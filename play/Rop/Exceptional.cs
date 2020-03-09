// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Runtime.ExceptionServices;

    using Abc.Utilities;

    using Anexn = System.ArgumentNullException;
    using EF = Abc.Utilities.ExceptionFactory;

    // When the error is in fact an exception.
    // Quick and dirty code.
    // Just for demo, in general it is a really bad idea to try to replace
    // the standard exception system.

    public abstract class Exceptional<T>
    {
        private protected Exceptional() { }

        public abstract bool IsError { get; }

        [NotNull] public abstract T Value { get; }

        [Pure]
        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Query Expression Pattern")]
        public abstract Exceptional<TResult> Select<TResult>(Func<T, TResult> selector);

        [Pure]
        public abstract Exceptional<T> Where(Func<T, bool> predicate);
    }

    public sealed class Empty<T> : Exceptional<T>
    {
        public static readonly Empty<T> Instance = new Empty<T>();

        private Empty() { }

        public override bool IsError => true;

        public override T Value { [DoesNotReturn] get => throw EF.Result_NoValue; }

        [Pure]
        public override Exceptional<TResult> Select<TResult>(Func<T, TResult> selector)
            => Empty<TResult>.Instance;

        [Pure]
        public override Exceptional<T> Where(Func<T, bool> predicate)
            => this;
    }

    public sealed class Ok1<T> : Exceptional<T>
    {
        internal Ok1([DisallowNull]T value)
        {
            Value = value;
        }

        public override bool IsError => false;

        [NotNull] public override T Value { get; }

        [Pure]
        public override Exceptional<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            if (selector is null) { throw new Anexn(nameof(selector)); }
            return new Ok1<TResult>(selector(Value));
        }

        [Pure]
        public override Exceptional<T> Where(Func<T, bool> predicate)
        {
            if (predicate is null) { throw new Anexn(nameof(predicate)); }
            if (predicate(Value)) { return this; }
            else { return Empty<T>.Instance; }
        }
    }

    public sealed class Threw<T> : Exceptional<T>
    {
        public Threw([DisallowNull] Exception exception)
        {
            InnerException = exception ?? throw new Anexn(nameof(exception));
        }

        public override bool IsError => true;

        public override T Value { [DoesNotReturn] get => throw EF.Result_NoValue; }

        [NotNull] public Exception InnerException { get; }

        [Pure]
        public override Exceptional<TResult> Select<TResult>(Func<T, TResult> selector)
            => new Threw<TResult>(InnerException);

        [Pure]
        public override Exceptional<T> Where(Func<T, bool> predicate)
            => this;
    }

    public sealed class ExceptionalFactory<T>
    {
        private ExceptionalFactory() { }

        internal static readonly ExceptionalFactory<T> Uniq = new ExceptionalFactory<T>();

        [Pure]
        public Empty<T> Empty => Empty<T>.Instance;

        [Pure]
        public Exceptional<T> Threw([DisallowNull]Exception exception)
            => new Threw<T>(exception);
    }

    public static class Exceptional
    {
        public static readonly Exceptional<Unit> Ok = new Ok1<Unit>(default);

        [Pure]
        public static ExceptionalFactory<T> OfType<T>()
            => ExceptionalFactory<T>.Uniq;

        [Pure]
        // Not actually obsolete, but clearly states that we shouldn't use it.
        [Obsolete("Use SomeOrNone() instead.")]
        public static Exceptional<T> Of<T>(T? value) where T : struct
        {
            if (value.HasValue) { return new Ok1<T>(value.Value); }
            else { return Empty<T>.Instance; }
        }

        [Pure]
        public static Exceptional<T> Of<T>([AllowNull]T value)
        {
            if (value is null) { return Empty<T>.Instance; }
            else { return new Ok1<T>(value); }
        }

        [Pure]
        public static Exceptional<T> None<T>() where T : notnull
            => Empty<T>.Instance;

        [Pure]
        public static Ok1<T> Some<T>(T value) where T : struct
            => new Ok1<T>(value);

        [Pure]
        public static Exceptional<T> SomeOrNone<T>(T? value) where T : struct
        {
            if (value.HasValue) { return new Ok1<T>(value.Value); }
            else { return Empty<T>.Instance; }
        }

        [Pure]
        public static Exceptional<T> SomeOrNone<T>(T? value) where T : class
        {
            if (value is null) { return Empty<T>.Instance; }
            else { return new Ok1<T>(value); }
        }

        public static void Rethrow(Exception ex)
        {
            Require.NotNull(ex, nameof(ex));
            ExceptionDispatchInfo.Capture(ex).Throw();
        }

        public static TResult Rethrow<TResult>(Exception ex)
        {
            Require.NotNull(ex, nameof(ex));
            ExceptionDispatchInfo.Capture(ex).Throw();
            return default;
        }

#pragma warning disable CA1031 // Do not catch general exception types

        [Pure]
        [Obsolete("Do not use as it, catching general exception types is an antipattern.")]
        public static Exceptional<Unit> TryWith(Action action)
        {
            if (action is null) { throw new Anexn(nameof(action)); }

            try
            {
                action();
                return Ok;
            }
            catch (Exception ex)
            {
                return OfType<Unit>().Threw(ex);
            }
        }

        [Pure]
        [Obsolete("Do not use as it, catching general exception types is an antipattern.")]
        public static Exceptional<TResult> TryWith<TResult>(Func<TResult> func)
        {
            if (func is null) { throw new Anexn(nameof(func)); }

            try
            {
                return Of(func());
            }
            catch (Exception ex)
            {
                return OfType<TResult>().Threw(ex);
            }
        }

        [Pure]
        [Obsolete("Do not use as it, catching general exception types is an antipattern.")]
        public static Exceptional<Unit> TryFinally(Action action, Action finallyAction)
        {
            if (action is null) { throw new Anexn(nameof(action)); }
            if (finallyAction is null) { throw new Anexn(nameof(finallyAction)); }

            try
            {
                action();
                return Ok;
            }
            catch (Exception ex)
            {
                return OfType<Unit>().Threw(ex);
            }
            finally
            {
                finallyAction();
            }
        }

        [Pure]
        [Obsolete("Do not use as it, catching general exception types is an antipattern.")]
        public static Exceptional<TResult> TryFinally<TResult>(
            Func<TResult> func, Action finallyAction)
        {
            if (func is null) { throw new Anexn(nameof(func)); }
            if (finallyAction is null) { throw new Anexn(nameof(finallyAction)); }

            try
            {
                return Of(func());
            }
            catch (Exception ex)
            {
                return OfType<TResult>().Threw(ex);
            }
            finally
            {
                finallyAction();
            }
        }

#pragma warning restore CA1031
    }
}
