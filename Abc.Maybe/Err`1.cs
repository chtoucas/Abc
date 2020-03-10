// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    using Anexn = System.ArgumentNullException;
    using EF = Abc.Utilities.ExceptionFactory;

    // TODO: improvements (?) inner error, aggregate errors?

    public sealed partial class Err<T> : Result<T>
    {
        internal Err()
        {
            Message = "No value";
            IsNone = true;
        }

        public Err(string message)
        {
            Message = message ?? throw new Anexn(nameof(message));
            IsNone = false;
        }

        public override bool IsError => true;

        public override T Value { [DoesNotReturn] get => throw EF.Result_NoValue; }

        public bool IsNone { get; }

        [NotNull] public string Message { get; }

        public override string ToString()
            => $"Err({Message})";

        [Pure]
        public Err<TOther> WithGenericType<TOther>()
            => IsNone ? Err<TOther>.None : new Err<TOther>(Message);

        [Pure]
        public override Maybe<T> ToMaybe()
            => Maybe<T>.None;

        [Pure]
        public override Result<T> OrElse(Result<T> other)
            => other;
    }

    // Query Expression Pattern aka LINQ.
    public partial class Err<T>
    {
        [Pure]
        public override Result<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            return WithGenericType<TResult>();
        }

        [Pure]
        public override Result<T> Where(Func<T, bool> predicate)
            => this;

        [Pure]
        public override Result<TResult> SelectMany<TMiddle, TResult>(
            Func<T, Result<TMiddle>> selector,
            Func<T, TMiddle, TResult> resultSelector)
        {
            return WithGenericType<TResult>();
        }

        [Pure]
        public override Result<TResult> Join<TInner, TKey, TResult>(
            Result<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector)
        {
            return WithGenericType<TResult>();
        }

        [Pure]
        public override Result<TResult> Join<TInner, TKey, TResult>(
            Result<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector,
            IEqualityComparer<TKey>? comparer)
        {
            return WithGenericType<TResult>();
        }
    }
}
