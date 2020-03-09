// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Runtime.ExceptionServices;

    using Abc.Utilities;

    using Anexn = System.ArgumentNullException;
    using EF = Abc.Utilities.ExceptionFactory;

    // When the error is in fact an exception.
    // Warning, it is a really bad idea to try to replace the standard
    // exception system...

    public abstract class Exceptional<T>
    {
        public static readonly Exceptional<T> None = new Ok_();

        public abstract bool IsError { get; }

        [MaybeNull] public abstract T Value { get; }

        [MaybeNull] public abstract Exception InnerException { get; }

        internal static Exceptional<T> Ok(T value)
            => new Exceptional<T>.Ok_(value);

        internal static Exceptional<T> Threw(Exception exception)
            => new Exceptional<T>.Threw_(exception);

        [return: NotNull]
        public abstract T ValueOrRethrow();

        public abstract Maybe<T> ToMaybe();

        [Pure]
        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Visual Basic: use an escaped name")]
        public abstract Exceptional<T> OrElse(Exceptional<T> other);

        [Pure]
        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Query Expression Pattern")]
        public abstract Exceptional<TResult> Select<TResult>(Func<T, TResult> selector);

        [Pure]
        public abstract Exceptional<T> Where(Func<T, bool> predicate);

        [Pure]
        public abstract Exceptional<TResult> SelectMany<TMiddle, TResult>(
            Func<T, Exceptional<TMiddle>> selector,
            Func<T, TMiddle, TResult> resultSelector);

        [Pure]
        public abstract Exceptional<TResult> Join<TInner, TKey, TResult>(
            Exceptional<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector);

        [Pure]
        public abstract Exceptional<TResult> Join<TInner, TKey, TResult>(
            Exceptional<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector,
            IEqualityComparer<TKey>? comparer);

        private sealed class Ok_ : Exceptional<T>
        {
            public Ok_()
            {
                // FIXME: NULL_FORGIVING
                Value = default!;
            }

            public Ok_(T value)
            {
                Value = value ?? throw new Anexn(nameof(value));
            }

            public override bool IsError => false;

            public override T Value { get; }

            public override Exception InnerException
            { [DoesNotReturn] get => throw new InvalidOperationException(); }

            public override T ValueOrRethrow()
                => Value;

            public override Maybe<T> ToMaybe()
                // TODO: if moved to the main assembly, use the ctor.
                => Maybe.Of(Value);

            public override Exceptional<T> OrElse(Exceptional<T> other)
                => this;

            public override Exceptional<TResult> Select<TResult>(Func<T, TResult> selector)
            {
                if (selector is null) { throw new Anexn(nameof(selector)); }

                return new Exceptional<TResult>.Ok_(selector(Value));
            }

            public override Exceptional<T> Where(Func<T, bool> predicate)
            {
                if (predicate is null) { throw new Anexn(nameof(predicate)); }

                return predicate(Value) ? this : None;
            }

            public override Exceptional<TResult> SelectMany<TMiddle, TResult>(
                Func<T, Exceptional<TMiddle>> selector,
                Func<T, TMiddle, TResult> resultSelector)
            {
                if (selector is null) { throw new Anexn(nameof(selector)); }
                if (resultSelector is null) { throw new Anexn(nameof(resultSelector)); }

                Exceptional<TMiddle> middle = selector(Value);
                if (middle is Exceptional<TMiddle>.Threw_ err)
                {
                    return err.WithReturnType<TResult>();
                }

                return Exceptional.Ok(resultSelector(Value, middle.Value));
            }

            public override Exceptional<TResult> Join<TInner, TKey, TResult>(
                Exceptional<TInner> inner,
                Func<T, TKey> outerKeySelector,
                Func<TInner, TKey> innerKeySelector,
                Func<T, TInner, TResult> resultSelector)
            {
                if (inner is null) { throw new Anexn(nameof(inner)); }
                if (outerKeySelector is null) { throw new Anexn(nameof(outerKeySelector)); }
                if (innerKeySelector is null) { throw new Anexn(nameof(innerKeySelector)); }
                if (resultSelector is null) { throw new Anexn(nameof(resultSelector)); }

                return JoinImpl(
                    inner,
                    outerKeySelector,
                    innerKeySelector,
                    resultSelector,
                    EqualityComparer<TKey>.Default);
            }

            public override Exceptional<TResult> Join<TInner, TKey, TResult>(
                Exceptional<TInner> inner,
                Func<T, TKey> outerKeySelector,
                Func<TInner, TKey> innerKeySelector,
                Func<T, TInner, TResult> resultSelector,
                IEqualityComparer<TKey>? comparer)
            {
                if (inner is null) { throw new Anexn(nameof(inner)); }
                if (outerKeySelector is null) { throw new Anexn(nameof(outerKeySelector)); }
                if (innerKeySelector is null) { throw new Anexn(nameof(innerKeySelector)); }
                if (resultSelector is null) { throw new Anexn(nameof(resultSelector)); }

                return JoinImpl(
                    inner,
                    outerKeySelector,
                    innerKeySelector,
                    resultSelector,
                    comparer ?? EqualityComparer<TKey>.Default);
            }

            private Exceptional<TResult> JoinImpl<TInner, TKey, TResult>(
                Exceptional<TInner> inner,
                Func<T, TKey> outerKeySelector,
                Func<TInner, TKey> innerKeySelector,
                Func<T, TInner, TResult> resultSelector,
                IEqualityComparer<TKey> comparer)
            {
                if (!inner.IsError)
                {
                    var outerKey = outerKeySelector(Value);
                    var innerKey = innerKeySelector(inner.Value);

                    if (comparer.Equals(outerKey, innerKey))
                    {
                        return Exceptional.Ok(resultSelector(Value, inner.Value));
                    }
                }

                return Exceptional<TResult>.None;
            }
        }

        private sealed class Threw_ : Exceptional<T>
        {
            public Threw_(Exception exception)
            {
                InnerException = exception ?? throw new Anexn(nameof(exception));
            }

            public override bool IsError => true;

            public override T Value
            { [DoesNotReturn] get => throw EF.Exceptional_NoValue; }

            public override Exception InnerException { get; }

            public Exceptional<TOther> WithReturnType<TOther>()
                => new Exceptional<TOther>.Threw_(InnerException);

            [DoesNotReturn]
            public override T ValueOrRethrow()
            {
                ExceptionDispatchInfo.Capture(InnerException).Throw();
                return default;
            }

            public override Maybe<T> ToMaybe()
                => Maybe<T>.None;

            public override Exceptional<T> OrElse(Exceptional<T> other)
                => other;

            public override Exceptional<TResult> Select<TResult>(Func<T, TResult> selector)
                => new Exceptional<TResult>.Threw_(InnerException);

            public override Exceptional<T> Where(Func<T, bool> predicate)
                => this;

            public override Exceptional<TResult> SelectMany<TMiddle, TResult>(
                Func<T, Exceptional<TMiddle>> selector,
                Func<T, TMiddle, TResult> resultSelector)
            {
                return new Exceptional<TResult>.Threw_(InnerException);
            }

            public override Exceptional<TResult> Join<TInner, TKey, TResult>(
                Exceptional<TInner> inner,
                Func<T, TKey> outerKeySelector,
                Func<TInner, TKey> innerKeySelector,
                Func<T, TInner, TResult> resultSelector)
            {
                return new Exceptional<TResult>.Threw_(InnerException);
            }

            public override Exceptional<TResult> Join<TInner, TKey, TResult>(
                Exceptional<TInner> inner,
                Func<T, TKey> outerKeySelector,
                Func<TInner, TKey> innerKeySelector,
                Func<T, TInner, TResult> resultSelector,
                IEqualityComparer<TKey>? comparer)
            {
                return new Exceptional<TResult>.Threw_(InnerException);
            }
        }
    }

    public static class Exceptional
    {
        public static readonly Exceptional<Unit> Unit = Ok(default(Unit));

        [Pure]
        public static Exceptional<T> Ok<T>([AllowNull]T value)
            => value is null ? Exceptional<T>.None : Exceptional<T>.Ok(value);

        [Pure]
        public static Exceptional<T> Threw<T>(Exception exception)
            => Exceptional<T>.Threw(exception);

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
                return Unit;
            }
            catch (Exception ex)
            {
                return Threw<Unit>(ex);
            }
        }

        [Pure]
        [Obsolete("Do not use as it, catching general exception types is an antipattern.")]
        public static Exceptional<TResult> TryWith<TResult>(Func<TResult> func)
        {
            if (func is null) { throw new Anexn(nameof(func)); }

            try
            {
                return Ok(func());
            }
            catch (Exception ex)
            {
                return Threw<TResult>(ex);
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
                return Unit;
            }
            catch (Exception ex)
            {
                return Threw<Unit>(ex);
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
                return Ok(func());
            }
            catch (Exception ex)
            {
                return Threw<TResult>(ex);
            }
            finally
            {
                finallyAction();
            }
        }

#pragma warning restore CA1031
    }
}
