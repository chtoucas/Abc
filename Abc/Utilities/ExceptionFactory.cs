// See LICENSE.txt in the project root for license information.

#pragma warning disable CA1303 // Do not pass literals as localized parameters.

namespace Abc.Utilities
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides static methods to create exceptions.
    /// <para>This class cannot be inherited.</para>
    /// </summary>
    internal static class ExceptionFactory
    {
        [Pure]
        public static InvalidOperationException EmptySequence
            => new InvalidOperationException("The sequence was empty.");

        [Pure]
        public static ArgumentException InvalidComparaison(string paramName)
            // TODO: exn message.
            => new ArgumentException("XXX", paramName);
    }
}
