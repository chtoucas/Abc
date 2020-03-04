// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    // Filtering: WhereAny (deferred).
    public static partial class Qperators
    {
        // Maybe<IEnumerable<TSource>>?
        [Pure]
        public static IEnumerable<TSource> WhereAny<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, Maybe<bool>> predicate)
        {
            // Check args eagerly.
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            if (predicate is null) { throw new ArgumentNullException(nameof(predicate)); }

            return __iterator();

            IEnumerable<TSource> __iterator()
            {
                foreach (var item in source)
                {
                    var result = predicate(item);

                    if (result.IsSome && result.Value) { yield return item; }
                }
            }
        }
    }
}
