// See LICENSE.txt in the project root for license information.

namespace Abc.Utilities
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;

    internal static class Guard
    {
        [Pure]
        [DebuggerStepThrough]
        public static T NotNull<T>(T? value, string paramName) where T : class
            => value ?? throw new ArgumentNullException(paramName);
    }
}
