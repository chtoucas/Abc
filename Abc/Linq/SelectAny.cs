// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using System;
    using System.Collections.Generic;

    // Projection: SelectAny (deferred).
    public static partial class Qperators
    {
        public static IEnumerable<TResult> SelectAny<TSource, TResult>(
            this IEnumerable<TSource> source, Func<TSource, Maybe<TResult>> selector)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            if (selector is null) { throw new ArgumentNullException(nameof(selector)); }

            return __iterator();

#if MONADS_PURE
            IEnumerable<TResult> __iterator()
            {
                return CollectAny(source.Select(selector));
            }
#else
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
