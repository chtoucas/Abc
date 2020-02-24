// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    // Concatenation: ZipAny.
    public partial class Qperators
    {
        public static IEnumerable<TResult> ZipAny<T1, T2, TResult>(
            this IEnumerable<T1> first,
            IEnumerable<T2> second,
            Func<T1, T2, Maybe<TResult>> resultSelector)
        {
            return Maybe.CollectAny(first.Zip(second, resultSelector));
        }
    }
}
