// See LICENSE in the project root for license information.

namespace Abc.Utilities
{
    using System;
    using System.Diagnostics;

#if !COVER_ABC_UTILITIES
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
    internal static partial class Require
    {
        [DebuggerStepThrough]
        public static void NotNull<T>([ValidatedNotNull] T value, string paramName)
            where T : class
        {
            if (value is null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
