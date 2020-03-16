﻿// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    // Both an Option type and a Result type, but only a simple one where the
    // error part is just a string. The main difference with a Maybe<T> is that
    // we get the opportunity to describe the error.
    //
    // It is possible to construct a general Result type, an `Either<T, TError>`,
    // but I find it to be difficult to use due to the extra generic parameter
    // (no sum type in C#). I tried too with both a `Result<T>` and a generic
    // error type `Error<T, TErr>`, but its usability was equally questionnable.
    //
    // I wish to keep this class simple: no method Bind(), public access to the
    // value if any, and a property IsError, very much like Nullable<T> offers
    // Value and HasValue (or rather its opposite).
    //
    // In situations where a Maybe<T> is not suitable, for instance when T is a
    // mutable reference type, one can use a Result<T> instead.
    //
    // It don't recommend to use this type in a public API.
    public abstract partial class Result<T>
    {
        public static readonly Err<T> None = new Err<T>();

        // Only two classes can extend this one (Err<T> and Ok<T>),
        // pattern matching is therefore unambiguous.
        private protected Result() { }

        public abstract bool IsError { get; }

        // Even w/ Err<T> this property never returns null, indeed in case of
        // error the getter throws.
        [NotNull] public abstract T Value { get; }

        [Pure] public abstract Maybe<T> ToMaybe();

        [Pure]
        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Visual Basic: use an escaped name")]
        public abstract Result<T> OrElse(Result<T> other);
    }

    // Query Expression Pattern.
    public partial class Result<T>
    {
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