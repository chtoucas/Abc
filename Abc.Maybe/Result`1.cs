// See LICENSE.txt in the project root for license information.

// A Result type.
//
// It should
// 1. Be both a Result type and an Option type.
// 2. Be a reference type.
// 3. Work fine with pattern matching.
// 4. Offer basic support for the Query Expression Syntax.
//
// Solutions?
// - Generic `Result<T, TErr>` type using Either.
//   Usability is questionnable and it is only a Result type (not an Option type).
// - Base `Result<T>` and generic error type `Error<T, TErr>`.
//   Usability is questionnable.
//
// Here we choose to only cover the simplest situation, the one where the error
// is just a string.

namespace Abc
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    // Both an Option type and a Result type.
    public abstract class Result<T>
    {
        public static readonly Err<T> None = new Err<T>("No value.", true);

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
