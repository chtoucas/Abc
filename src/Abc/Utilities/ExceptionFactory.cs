// See LICENSE in the project root for license information.

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
    internal static partial class ExceptionFactory { }

    // Argument exceptions.
    internal partial class ExceptionFactory
    {
        [Pure]
        [DebuggerStepThrough]
        public static ArgumentException InvalidType(
            string paramName, Type expected, object obj)
            => new ArgumentException(
                $"The object should be of type {expected} but it is of type {obj.GetType()}.",
                paramName);

        [Pure]
        [DebuggerStepThrough]
        public static ArgumentException InvalidBinaryInput(string paramName)
            => new ArgumentException(
                "The binary data is not well-formed or is invalid.",
                paramName);
    }

    // Overflow exceptions.
    internal partial class ExceptionFactory
    {
        public static OverflowException YearOverflow
            => new OverflowException(
                "The computation would overflow the latest supported year.");

        public static OverflowException YearOverflowOrUnderflow
            => new OverflowException(
                "The computation would overflow the range of supported years.");

        public static OverflowException YearUnderflow
            => new OverflowException(
                "The computation would underflow the earliest supported year.");

        public static OverflowException DayNumberOverflow
            => new OverflowException(
                "The computation would overflow the latest supported date.");

        public static OverflowException DayNumberOverflowOrUnderflow
            => new OverflowException(
                "The computation would overflow the calendar boundaries.");

        public static OverflowException DayNumberUnderflow
            => new OverflowException(
                "The computation would underflow the earliest supported date.");
    }
}
