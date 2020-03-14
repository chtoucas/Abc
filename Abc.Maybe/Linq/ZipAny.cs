// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    // Concatenation: ZipAny.
    public partial class Qperators
    {
        [Pure]
        public static IEnumerable<TResult> ZipAny<T1, T2, TResult>(
            this IEnumerable<T1> first,
            IEnumerable<T2> second,
            Func<T1, T2, Maybe<TResult>> resultSelector)
        {
            // Identical to Maybe.CollectAny(first.Zip(second, resultSelector)).
            return from x in first.Zip(second, resultSelector)
                   where x.IsSome
                   select x.Value;
        }
    }
}
