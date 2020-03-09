// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    // Both an Option type and a Result type.
    public abstract class Result<T>
    {
        private protected Result() { }

        public abstract bool IsError { get; }

        [NotNull] public abstract T Value { get; }

        [Pure] public abstract Maybe<T> ToMaybe();

        [Pure]
        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Visual Basic: use an escaped name")]
        public abstract Result<T> OrElse(Result<T> other);

        [Pure]
        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Query Expression Pattern")]
        public abstract Result<TResult> Select<TResult>(Func<T, TResult> selector);

        [Pure] public abstract Result<T> Where(Func<T, bool> predicate);

        [Pure]
        public abstract Result<TResult> SelectMany<TMiddle, TResult>(
            Func<T, Result<TMiddle>> selector,
            Func<T, TMiddle, TResult> resultSelector);

        [Pure]
        public abstract Result<TResult> Join<TInner, TKey, TResult>(
            Result<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector);

        [Pure]
        public abstract Result<TResult> Join<TInner, TKey, TResult>(
            Result<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector,
            IEqualityComparer<TKey>? comparer);
    }
}
