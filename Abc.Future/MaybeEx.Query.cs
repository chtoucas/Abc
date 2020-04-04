// See LICENSE in the project root for license information.

namespace Abc
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using Anexn = System.ArgumentNullException;

    // We will need compelling examples to consider this seriously.

    public partial class MaybeEx
    {
        /// <example>
        /// Query expression syntax:
        /// <code><![CDATA[
        ///   from x in outer
        ///   join y in inner
        ///     on outerKeySelector(x) equals innerKeySelector(y)
        ///     into Z
        ///   select resultSelector(x, Z)
        /// ]]></code>
        /// </example>
        [Pure]
        public static Maybe<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(
            this Maybe<TOuter> outer,
            Maybe<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, Maybe<TInner>, TResult> resultSelector)
        {
            if (outerKeySelector is null) { throw new Anexn(nameof(outerKeySelector)); }
            if (innerKeySelector is null) { throw new Anexn(nameof(innerKeySelector)); }
            if (resultSelector is null) { throw new Anexn(nameof(resultSelector)); }

            return GroupJoinImpl(
                outer,
                inner,
                outerKeySelector,
                innerKeySelector,
                resultSelector,
                EqualityComparer<TKey>.Default);
        }

        // No query expression syntax.
        // If comparer is null, the default equality comparer is used instead.
        [Pure]
        public static Maybe<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(
            this Maybe<TOuter> outer,
            Maybe<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, Maybe<TInner>, TResult> resultSelector,
            IEqualityComparer<TKey>? comparer)
        {
            if (outerKeySelector is null) { throw new Anexn(nameof(outerKeySelector)); }
            if (innerKeySelector is null) { throw new Anexn(nameof(innerKeySelector)); }
            if (resultSelector is null) { throw new Anexn(nameof(resultSelector)); }

            return GroupJoinImpl(
                outer,
                inner,
                outerKeySelector,
                innerKeySelector,
                resultSelector,
                comparer ?? EqualityComparer<TKey>.Default);
        }

        [Pure]
        private static Maybe<TResult> GroupJoinImpl<T, TInner, TKey, TResult>(
            Maybe<T> outer,
            Maybe<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, Maybe<TInner>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            if (outer.TryGetValue(out T oval)
                && inner.TryGetValue(out TInner ival))
            {
                TKey outerKey = outerKeySelector(oval);
                TKey innerKey = innerKeySelector(ival);

                if (comparer.Equals(outerKey, innerKey))
                {
                    return Maybe.Of(resultSelector(oval, inner));
                }
            }

            return Maybe<TResult>.None;
        }
    }
}
