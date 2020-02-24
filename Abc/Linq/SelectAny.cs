// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using System;
    using System.Collections.Generic;
#if MONADS_PURE
    using System.Linq;
#endif

    // Projection: SelectAny (deferred).
    public static partial class Qperators
    {
        public static IEnumerable<TResult> SelectAny<TSource, TResult>(
            this IEnumerable<TSource> source, Func<TSource, Maybe<TResult>> selector)
        {
#if MONADS_PURE
            return Maybe.CollectAny(source.Select(selector));
#else
            // Check args eagerly.
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            if (selector is null) { throw new ArgumentNullException(nameof(selector)); }

            return __iterator();

            IEnumerable<TResult> __iterator()
            {
                foreach (var item in source)
                {
                    var result = selector(item);

                    if (result.IsSome) { yield return result.Value; }
                }
            }
#endif
        }
    }
}
