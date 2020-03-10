﻿// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.ExceptionServices;

    using Anexn = System.ArgumentNullException;
    using EF = Abc.Utilities.ExceptionFactory;

    public abstract partial class Faillible<T>
    {
        internal sealed partial class Exceptional : Faillible<T>
        {
            public Exceptional(Exception exception)
            {
                InnerException = exception ?? throw new Anexn(nameof(exception));
            }

            public override bool IsError => true;

            public override T Value
            {
                [DoesNotReturn]
                get => throw EF.Faillible_NoValue;
            }

            public override Exception InnerException { get; }

            public Faillible<TOther> WithReturnType<TOther>()
                => new Faillible<TOther>.Exceptional(InnerException);

            [DoesNotReturn]
            public override T ValueOrRethrow()
            {
                ExceptionDispatchInfo.Capture(InnerException).Throw();
                return default;
            }

            public override Maybe<T> ToMaybe()
                => Maybe<T>.None;

            public override Faillible<T> OrElse(Faillible<T> other)
                => other;
        }

        // Query Expression Pattern aka LINQ.
        internal partial class Exceptional
        {
            public override Faillible<TResult> Select<TResult>(Func<T, TResult> selector)
                => new Faillible<TResult>.Exceptional(InnerException);

            public override Faillible<T> Where(Func<T, bool> predicate)
                => this;

            public override Faillible<TResult> SelectMany<TMiddle, TResult>(
                Func<T, Faillible<TMiddle>> selector,
                Func<T, TMiddle, TResult> resultSelector)
            {
                return new Faillible<TResult>.Exceptional(InnerException);
            }

            public override Faillible<TResult> Join<TInner, TKey, TResult>(
                Faillible<TInner> inner,
                Func<T, TKey> outerKeySelector,
                Func<TInner, TKey> innerKeySelector,
                Func<T, TInner, TResult> resultSelector)
            {
                return new Faillible<TResult>.Exceptional(InnerException);
            }

            public override Faillible<TResult> Join<TInner, TKey, TResult>(
                Faillible<TInner> inner,
                Func<T, TKey> outerKeySelector,
                Func<TInner, TKey> innerKeySelector,
                Func<T, TInner, TResult> resultSelector,
                IEqualityComparer<TKey>? comparer)
            {
                return new Faillible<TResult>.Exceptional(InnerException);
            }
        }
    }
}
