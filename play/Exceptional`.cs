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

    // TODO: include in main proj or remove.
    // When the error is in fact an exception.
    // Warning, it is a really bad idea to try to replace the standard
    // exception system...

    public sealed class Exceptional<T> : Result<T>
    {
        public Exceptional([DisallowNull]Exception exception)
        {
            InnerException = exception ?? throw new Anexn(nameof(exception));
        }

        public override bool IsError => true;

        public override T Value { [DoesNotReturn] get => throw EF.Result_NoValue; }

        [NotNull] public Exception InnerException { get; }

        public override Maybe<T> ToMaybe()
            => Maybe<T>.None;

        public override Result<T> OrElse(Result<T> other)
            => other;

        [Pure]
        public override Result<TResult> Select<TResult>(Func<T, TResult> selector)
            => new Exceptional<TResult>(InnerException);

        [Pure]
        public override Result<T> Where(Func<T, bool> predicate)
            => this;

        public override Result<TResult> SelectMany<TMiddle, TResult>(
            Func<T, Result<TMiddle>> selector,
            Func<T, TMiddle, TResult> resultSelector)
        {
            return new Exceptional<TResult>(InnerException);
        }

        public override Result<TResult> Join<TInner, TKey, TResult>(
            Result<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector)
        {
            return new Exceptional<TResult>(InnerException);
        }

        public override Result<TResult> Join<TInner, TKey, TResult>(
            Result<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector,
            IEqualityComparer<TKey>? comparer)
        {
            return new Exceptional<TResult>(InnerException);
        }
    }

    public static class Exceptional
    {
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
        public static Result<Unit> TryWith(Action action)
        {
            if (action is null) { throw new Anexn(nameof(action)); }

            try
            {
                action();
                return Result.Unit;
            }
            catch (Exception ex)
            {
                return new Exceptional<Unit>(ex);
            }
        }

        [Pure]
        [Obsolete("Do not use as it, catching general exception types is an antipattern.")]
        public static Result<TResult> TryWith<TResult>(Func<TResult> func)
        {
            if (func is null) { throw new Anexn(nameof(func)); }

            try
            {
                return Result.Of(func());
            }
            catch (Exception ex)
            {
                return new Exceptional<TResult>(ex);
            }
        }

        [Pure]
        [Obsolete("Do not use as it, catching general exception types is an antipattern.")]
        public static Result<Unit> TryFinally(Action action, Action finallyAction)
        {
            if (action is null) { throw new Anexn(nameof(action)); }
            if (finallyAction is null) { throw new Anexn(nameof(finallyAction)); }

            try
            {
                action();
                return Result.Unit;
            }
            catch (Exception ex)
            {
                return new Exceptional<Unit>(ex);
            }
            finally
            {
                finallyAction();
            }
        }

        [Pure]
        [Obsolete("Do not use as it, catching general exception types is an antipattern.")]
        public static Result<TResult> TryFinally<TResult>(
            Func<TResult> func, Action finallyAction)
        {
            if (func is null) { throw new Anexn(nameof(func)); }
            if (finallyAction is null) { throw new Anexn(nameof(finallyAction)); }

            try
            {
                return Result.Of(func());
            }
            catch (Exception ex)
            {
                return new Exceptional<TResult>(ex);
            }
            finally
            {
                finallyAction();
            }
        }

#pragma warning restore CA1031
    }
}
