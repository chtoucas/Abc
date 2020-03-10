// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Anexn = System.ArgumentNullException;
    using EF = Abc.Utilities.ExceptionFactory;

    public abstract partial class Faillible<T>
    {
        internal sealed partial class Success : Faillible<T>
        {
            public Success()
            {
                // NULL_FORGIVING
                Value = default!;
            }

            public Success([DisallowNull]T value)
            {
                Value = value ?? throw new Anexn(nameof(value));
            }

            public override bool Threw => false;

            [MaybeNull] public override T Value { get; }

            public override Exception InnerException
            {
                [DoesNotReturn]
                get => throw EF.Faillible_NoInnerException;
            }

            public override T ValueOrRethrow()
                => Value;

            public override Maybe<T> ToMaybe()
                // TODO: if moved to the main assembly, use the ctor.
                => Maybe.Of(Value);

            public override Faillible<T> OrElse(Faillible<T> other)
                => this;
        }

        // Query Expression Pattern aka LINQ.
        internal partial class Success
        {
            public override Faillible<TResult> Select<TResult>(Func<T, TResult> selector)
            {
                if (selector is null) { throw new Anexn(nameof(selector)); }

                return new Faillible<TResult>.Success(selector(Value));
            }

            public override Faillible<T> Where(Func<T, bool> predicate)
            {
                if (predicate is null) { throw new Anexn(nameof(predicate)); }

                return predicate(Value) ? this : None;
            }

            public override Faillible<TResult> SelectMany<TMiddle, TResult>(
                Func<T, Faillible<TMiddle>> selector,
                Func<T, TMiddle, TResult> resultSelector)
            {
                if (selector is null) { throw new Anexn(nameof(selector)); }
                if (resultSelector is null) { throw new Anexn(nameof(resultSelector)); }

                Faillible<TMiddle> middle = selector(Value);
                if (middle is Faillible<TMiddle>.Exceptional err)
                {
                    return err.WithGenericType<TResult>();
                }

                return Faillible.Succeed(resultSelector(Value, middle.Value));
            }

            public override Faillible<TResult> Join<TInner, TKey, TResult>(
                Faillible<TInner> inner,
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

            public override Faillible<TResult> Join<TInner, TKey, TResult>(
                Faillible<TInner> inner,
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

            private Faillible<TResult> JoinImpl<TInner, TKey, TResult>(
                Faillible<TInner> inner,
                Func<T, TKey> outerKeySelector,
                Func<TInner, TKey> innerKeySelector,
                Func<T, TInner, TResult> resultSelector,
                IEqualityComparer<TKey> comparer)
            {
                if (!inner.Threw)
                {
                    TKey outerKey = outerKeySelector(Value);
                    TKey innerKey = innerKeySelector(inner.Value);

                    if (comparer.Equals(outerKey, innerKey))
                    {
                        return Faillible.Succeed(resultSelector(Value, inner.Value));
                    }
                }

                return Faillible<TResult>.None;
            }
        }
    }
}
