// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

namespace Abc.Edu.Fx
{
    using System;
    using System.Collections.Generic;

    using Abc.Utilities;

    // Query Expression Pattern.
    public static partial class Mayhap
    {
        public static Mayhap<TResult> SelectMany<T, TMiddle, TResult>(
            this Mayhap<T> @this,
            Func<T, Mayhap<TMiddle>> selector,
            Func<T, TMiddle, TResult> resultSelector)
        {
            Utilities.Guard.NotNull(selector, nameof(selector));
            Utilities.Guard.NotNull(resultSelector, nameof(resultSelector));

            return @this.Bind(
                x => selector(x).Select(
                    middle => resultSelector(x, middle)));
        }

        public static Mayhap<TResult> Join<T, TInner, TKey, TResult>(
            this Mayhap<T> @this,
            Mayhap<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector)
        {
            Utilities.Guard.NotNull(outerKeySelector, nameof(outerKeySelector));
            Utilities.Guard.NotNull(innerKeySelector, nameof(innerKeySelector));
            Utilities.Guard.NotNull(resultSelector, nameof(resultSelector));

            var comparer = EqualityComparer<TKey>.Default;

            var keyLookup = __getKeyLookup();

            return @this.SelectMany(__valueSelector, resultSelector);

            Mayhap<TInner> __valueSelector(T outer) => keyLookup(outerKeySelector(outer));

            Func<TKey, Mayhap<TInner>> __getKeyLookup()
            {
                return outerKey =>
                    inner.Select(innerKeySelector)
                        .Where(innerKey => comparer.Equals(innerKey, outerKey))
                        .ContinueWith(inner);
            }
        }

        //public static Mayhap<TResult> GroupJoin<T, TInner, TKey, TResult>(
        //    this Mayhap<T> @this,
        //    Mayhap<TInner> inner,
        //    Func<T, TKey> outerKeySelector,
        //    Func<TInner, TKey> innerKeySelector,
        //    Func<T, Mayhap<TInner>, TResult> resultSelector)
        //{
        //    Guard.NotNull(outerKeySelector, nameof(outerKeySelector));
        //    Guard.NotNull(innerKeySelector, nameof(innerKeySelector));
        //    Guard.NotNull(resultSelector, nameof(resultSelector));

        //    if (_isSome && inner._isSome)
        //    {
        //        var outerKey = outerKeySelector(_value);
        //        var innerKey = innerKeySelector(inner._value);

        //        if (EqualityComparer<TKey>.Default.Equals(outerKey, innerKey))
        //        {
        //            return Mayhap.Of(resultSelector(_value, inner));
        //        }
        //    }

        //    return Mayhap<TResult>.None;
        //}
    }
}
