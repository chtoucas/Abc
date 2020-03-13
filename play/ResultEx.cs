// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Anexn = System.ArgumentNullException;

    public static partial class ResultEx { }

    // Extension methods for Result<T> where T is enumerable.
    // LINQ extensions for IEnumerable<Result<T>>.
    public partial class ResultEx
    {
        [Pure]
        public static IEnumerable<T> ValueOrEmpty<T>(
            this Result<IEnumerable<T>> @this)
        {
            if (@this is null) { throw new Anexn(nameof(@this)); }

            return @this.IsError ? Enumerable.Empty<T>() : @this.Value;
        }

        [Pure]
        public static IEnumerable<T> CollectAny<T>(IEnumerable<Result<T>> source)
        {
            return from x in source where !x.IsError select x.Value;
        }

        // Any()
        [Pure]
        public static Result<T> FirstSuccess<T>(IEnumerable<Result<T>> source)
        {
            return source.FirstOrDefault(x => !x.IsError);
        }
    }
}
