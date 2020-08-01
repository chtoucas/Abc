// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

#pragma warning disable CA1303 // Do not pass literals as localized parameters.

namespace Abc.Utilities
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;

    internal sealed partial class ExceptionFactoryEx : ExceptionFactory { }

    // Argument exceptions.
    internal partial class ExceptionFactoryEx
    {
        [Pure]
        [DebuggerStepThrough]
        public static ArgumentException InvalidBinaryInput(string paramName) =>
            new ArgumentException(
                "The binary data is not well-formed or is invalid.",
                paramName);
    }

    // Overflow exceptions.
    internal partial class ExceptionFactoryEx
    {
        public static OverflowException YearOverflow =>
            new OverflowException(
                "The computation would overflow the latest supported year.");

        public static OverflowException YearOverflowOrUnderflow =>
            new OverflowException(
                "The computation would overflow the range of supported years.");

        public static OverflowException YearUnderflow =>
            new OverflowException(
                "The computation would underflow the earliest supported year.");

        public static OverflowException DayNumberOverflow =>
            new OverflowException(
                "The computation would overflow the latest supported date.");

        public static OverflowException DayNumberOverflowOrUnderflow =>
            new OverflowException(
                "The computation would overflow the calendar boundaries.");

        public static OverflowException DayNumberUnderflow =>
            new OverflowException(
                "The computation would underflow the earliest supported date.");
    }
}
