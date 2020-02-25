// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
#if MONADS_PURE
    using System.Linq;
#endif

    // Filtering: WhereAny (deferred).
    public static partial class Qperators
    {
        // Maybe<IEnumerable<TSource>>?
        [Pure]
        public static IEnumerable<TSource> WhereAny<TSource>(
            this IEnumerable<TSource> source, Func<TSource, Maybe<bool>> predicate)
        {
#if MONADS_PURE
            var seed = Maybe.Empty<TSource>();
            var seq = source.Aggregate(seed, (x, y) => predicate(y).ZipWith(x, __zipper(y)));
            return seq.ValueOrEmpty();

            Func<bool, IEnumerable<TSource>, IEnumerable<TSource>> __zipper(TSource item)
                => (b, seq) => b ? seq.Append(item) : seq;
#else
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
#endif
        }
    }
}
