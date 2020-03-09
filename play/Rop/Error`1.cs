// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    using Anexn = System.ArgumentNullException;
    using EF = Abc.Utilities.ExceptionFactory;

    // TODO: improvements? inner error, aggregate errors?

    [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Visual Basic: use an escaped name")]
    public sealed class Error<T> : Result<T>
    {
        public static readonly Error<T> None = new Error<T>("No value", true);

        public Error([DisallowNull] string message) : this(message, false) { }

        private Error([DisallowNull] string message, bool isNone)
        {
            Message = message ?? throw new Anexn(nameof(message));
            IsNone = isNone;
        }

        public override bool IsError => true;

        public override T Value { [DoesNotReturn] get => throw EF.Result_NoValue; }

        public bool IsNone { get; }

        [NotNull] public string Message { get; }

        public override string ToString()
            => Message;

        [Pure]
        public Error<TOther> WithReturnType<TOther>()
            => IsNone ? Error<TOther>.None : new Error<TOther>(Message);

        [Pure]
        public override Maybe<T> ToMaybe()
            => Maybe<T>.None;

        [Pure]
        public override Result<T> OrElse(Result<T> other)
            => other;

        [Pure]
        public override Result<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            return WithReturnType<TResult>();
        }

        [Pure]
        public override Result<T> Where(Func<T, bool> predicate)
            => this;

        [Pure]
        public override Result<TResult> SelectMany<TMiddle, TResult>(
            Func<T, Result<TMiddle>> selector,
            Func<T, TMiddle, TResult> resultSelector)
        {
            return WithReturnType<TResult>();
        }

        [Pure]
        public override Result<TResult> Join<TInner, TKey, TResult>(
            Result<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector)
        {
            return WithReturnType<TResult>();
        }

        [Pure]
        public override Result<TResult> Join<TInner, TKey, TResult>(
            Result<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector,
            IEqualityComparer<TKey>? comparer)
        {
            return WithReturnType<TResult>();
        }
    }
}
