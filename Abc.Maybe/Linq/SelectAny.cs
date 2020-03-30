// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Anexn = System.ArgumentNullException;

    public static partial class Qperators
    {
        [Pure]
        public static IEnumerable<TResult> SelectAny<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, Maybe<TResult>> selector)
        {
            if (selector is null) { throw new Anexn(nameof(selector)); }

            return from x in source
                   let result = selector(x)
                   where result.IsSome
                   select result.Value;
        }
    }
}
