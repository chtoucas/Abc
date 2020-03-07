// See LICENSE.txt in the project root for license information.

namespace Abc.Edu.Fx
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abc.Utilities;

    // Query Expression Pattern aka LINQ.
    public static partial class Mayhap
    {
        public static Mayhap<TResult> SelectMany<T, TMiddle, TResult>(
            this Mayhap<T> @this,
            Func<T, Mayhap<TMiddle>> selector,
            Func<T, TMiddle, TResult> resultSelector)
        {
            Require.NotNull(selector, nameof(selector));
            Require.NotNull(resultSelector, nameof(resultSelector));

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
            // NULL_FORGIVING
            return Join(@this, inner, outerKeySelector, innerKeySelector, resultSelector, null!);
        }

        public static Mayhap<TResult> Join<T, TInner, TKey, TResult>(
            this Mayhap<T> @this,
            Mayhap<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            Require.NotNull(outerKeySelector, nameof(outerKeySelector));
            Require.NotNull(innerKeySelector, nameof(innerKeySelector));
            Require.NotNull(resultSelector, nameof(resultSelector));

            var keyLookup = __getKeyLookup(inner, innerKeySelector, comparer);

            return @this.SelectMany(__valueSelector, resultSelector);

            Mayhap<TInner> __valueSelector(T outer) => keyLookup(outerKeySelector(outer));

            static Func<TKey, Mayhap<TInner>> __getKeyLookup(
               Mayhap<TInner> inner,
               Func<TInner, TKey> innerKeySelector,
               IEqualityComparer<TKey>? comparer)
            {
                return outerKey =>
                    inner.Select(innerKeySelector)
                        .Where(innerKey =>
                            (comparer ?? EqualityComparer<TKey>.Default)
                                .Equals(innerKey, outerKey))
                        .ContinueWith(inner);
            }
        }

        //
        // GroupJoin currently disabled.
        //

        //public static Mayhap<TResult> GroupJoin<T, TInner, TKey, TResult>(
        //    this Mayhap<T> @this,
        //    Mayhap<TInner> inner,
        //    Func<T, TKey> outerKeySelector,
        //    Func<TInner, TKey> innerKeySelector,
        //    Func<T, Mayhap<TInner>, TResult> resultSelector,
        //    IEqualityComparer<TKey> comparer)
        //{
        //    Require.NotNull(outerKeySelector, nameof(outerKeySelector));
        //    Require.NotNull(innerKeySelector, nameof(innerKeySelector));
        //    Require.NotNull(resultSelector, nameof(resultSelector));

        //    if (_isSome && inner._isSome)
        //    {
        //        var outerKey = outerKeySelector(_value);
        //        var innerKey = innerKeySelector(inner._value);

        //        if ((comparer ?? EqualityComparer<TKey>.Default).Equals(outerKey, innerKey))
        //        {
        //            return Mayhap.Of(resultSelector(_value, inner));
        //        }
        //    }

        //    return Mayhap<TResult>.None;
        //}
    }

    // Extension methods for Mayhap<T> where T is enumerable.
    // Operations on IEnumerable<Mayhap<T>>.
    // - Filtering: CollectAny (deferred).
    // - Aggregation: Any.
    public partial class Mayhap
    {
        public static Mayhap<IEnumerable<T>> Empty<T>()
            => MayhapEnumerable_<T>.Empty;

        public static Mayhap<IEnumerable<T>> CollectAny<T>(IEnumerable<Mayhap<T>> source)
        {
            var seed = MayhapEnumerable_<T>.Empty;
            return source.Aggregate(seed, (x, y) => x.ZipWith(y, Enumerable.Append));
        }

        private static class MayhapEnumerable_<T>
        {
            internal static readonly Mayhap<IEnumerable<T>> Empty
                = Mayhap<IEnumerable<T>>.η(Enumerable.Empty<T>());
        }
    }
}
