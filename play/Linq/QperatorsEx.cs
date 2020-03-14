// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    public static class QperatorsEx
    {
        [Pure]
        public static IEnumerable<TResult> SelectAny<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, Result<TResult>> selector)
        {
            return from x in source
                   let result = selector(x)
                   where !result.IsError
                   select result.Value;
        }

        [Pure]
        public static IEnumerable<TSource> WhereAny<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, Result<bool>> predicate)
        {
            return from x in source
                   let result = predicate(x)
                   where !result.IsError && result.Value
                   select x;
        }

        [Pure]
        public static IEnumerable<TResult> ZipAny<T1, T2, TResult>(
            this IEnumerable<T1> first,
            IEnumerable<T2> second,
            Func<T1, T2, Result<TResult>> resultSelector)
        {
            // Identical to Result.CollectAny(first.Zip(second, resultSelector)).
            return from x in first.Zip(second, resultSelector)
                   where !x.IsError
                   select x.Value;
        }
    }
}
