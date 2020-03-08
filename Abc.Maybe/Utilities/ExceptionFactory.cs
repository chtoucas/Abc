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
        public static InvalidOperationException ControlFlow
            => new InvalidOperationException(
                "The flow of execution just reached a section of the code that should have been unreachable."
                + $"{Environment.NewLine}Most certainly signals a coding error. Please report.");

        [Pure]
        public static InvalidOperationException NoValue
            => new InvalidOperationException(
                "The object does not contain any value."
                + $"{Environment.NewLine}You should have checked that the property IsSome is true.");

        [Pure]
        public static InvalidOperationException EmptySequence
            => new InvalidOperationException("The sequence was empty.");

        [Pure]
        [DebuggerStepThrough]
        public static ArgumentException InvalidType(
            string paramName, Type expected, object obj)
            => new ArgumentException(
                $"The object should be of type {expected} but it is of type {obj.GetType()}.",
                paramName);
    }
}
