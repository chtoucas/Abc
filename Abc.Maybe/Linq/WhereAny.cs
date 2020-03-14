// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    // Filtering: WhereAny (deferred).
    public static partial class Qperators
    {
        [Pure]
        public static IEnumerable<TSource> WhereAny<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, Maybe<bool>> predicate)
        {
            return from x in source
                   let result = predicate(x)
                   where result.IsSome && result.Value
                   select x;
        }
    }
}
