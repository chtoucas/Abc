// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Anexn = System.ArgumentNullException;

    // Filtering: WhereAny (deferred).
    public static partial class Qperators
    {
        [Pure]
        public static IEnumerable<TSource> WhereAny<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, Maybe<bool>> predicate)
        {
            if (source is null) { throw new Anexn(nameof(source)); }
            if (predicate is null) { throw new Anexn(nameof(predicate)); }

            return from x in source
                   let result = predicate(x)
                   where result.IsSome && result.Value
                   select x;
        }
    }
}
