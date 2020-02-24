// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using System.Collections.Generic;
    using System.Linq;

    // Generation: RepeatAny.
    public partial class Qperators
    {
        // Maybe<IEnumerable<TSource>>?
        public static IEnumerable<TSource> RepeatAny<TSource>(Maybe<TSource> value, int count)
        {
#if MONADS_PURE
            return value.Select(x => Enumerable.Repeat(x, count)).ValueOrEmpty();
#else
            return value.IsSome ? Enumerable.Repeat(value.Value, count)
                : Enumerable.Empty<TSource>();
#endif
        }
    }
}
