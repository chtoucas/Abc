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
        public static InvalidOperationException Maybe_IsNone
            => new InvalidOperationException("The object does not contain any value.");

        [Pure]
        public static InvalidOperationException Result_NoValue
            => new InvalidOperationException(
                "The object does not contain any value."
                + $"{Environment.NewLine}You should have checked that the property IsError is not true.");

        [Pure]
        public static InvalidOperationException EmptySequence
            => new InvalidOperationException("The sequence was empty.");

        [Pure]
        public static NotSupportedException ReadOnlyCollection
            => new NotSupportedException("The collection is read-only.");

        [Pure]
        [DebuggerStepThrough]
        public static ArgumentException InvalidType(
            string paramName, Type expected, object obj)
            => new ArgumentException(
                $"The object should be of type {expected} but it is of type {obj.GetType()}.",
                paramName);
    }
}
