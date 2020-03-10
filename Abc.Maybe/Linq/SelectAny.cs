// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    // Projection: SelectAny (deferred).
    public static partial class Qperators
    {
        [Pure]
        public static IEnumerable<TResult> SelectAny<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, Maybe<TResult>> selector)
        {
            return from x in source
                   let result = selector(x)
                   where result.IsSome
                   select result.Value;
        }

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
    }
}
