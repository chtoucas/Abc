// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using System;
    using System.Collections.Generic;

    // Filtering: WhereAny (deferred).
    public static partial class Qperators
    {
        // Maybe<IEnumerable<TSource>>?
        public static IEnumerable<TSource> WhereAny<TSource>(
            this IEnumerable<TSource> source, Func<TSource, Maybe<bool>> predicate)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            if (predicate is null) { throw new ArgumentNullException(nameof(predicate)); }

            return __iterator();

#if MONADS_PURE
            IEnumerable<TSource> __iterator()
            {
                var seed = Maybe.Empty<TSource>();

                return source.Aggregate(
                    seed,
                    (x, y) => predicate(y).ZipWith(x, __zipper(y)))
                    .ValueOrEmpty();
            }

            Func<bool, IEnumerable<TSource>, IEnumerable<TSource>> __zipper(TSource item)
                => (b, seq) => b ? seq.Append(item) : seq;
#else
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
