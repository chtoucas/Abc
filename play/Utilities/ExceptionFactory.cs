// See LICENSE.txt in the project root for license information.

#pragma warning disable CA1303 // Do not pass literals as localized parameters.

namespace Abc.Utilities
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides static methods to create exceptions.
    /// <para>This class cannot be inherited.</para>
    /// </summary>
    internal static class ExceptionFactory
    {
        [Pure]
        public static InvalidOperationException Exceptional_NoValue
            => new InvalidOperationException(
                "The object does not contain any value."
                + $"{Environment.NewLine}You should have checked that the property IsError is not true.");
    }
}
