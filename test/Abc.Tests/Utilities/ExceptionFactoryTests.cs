// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

namespace Abc.Utilities
{
    using System;

    using Xunit;

    using Assert = AssertEx;
    using EF = ExceptionFactory;

    public static partial class ExceptionFactoryTests { }

    // Argument exceptions.
    public partial class ExceptionFactoryTests
    {
        [Fact]
        public static void InvalidBinaryInput()
        {
            // Act
            var ex = EF.InvalidBinaryInput("paramName");
            // Assert
            Assert.CheckArgumentException(ex);
        }
    }

    // Overflow exceptions.
    public partial class ExceptionFactoryTests
    {
        [Fact] public static void YearOverflow() =>
            Assert.CheckException(typeof(OverflowException), EF.YearOverflow);

        [Fact] public static void YearOverflowOrUnderflow() =>
            Assert.CheckException(typeof(OverflowException), EF.YearOverflowOrUnderflow);

        [Fact] public static void YearUnderflow() =>
            Assert.CheckException(typeof(OverflowException), EF.YearUnderflow);

        [Fact] public static void DayNumberOverflow() =>
            Assert.CheckException(typeof(OverflowException), EF.DayNumberOverflow);

        [Fact] public static void DayNumberOverflowOrUnderflow() =>
            Assert.CheckException(typeof(OverflowException), EF.DayNumberOverflowOrUnderflow);

        [Fact] public static void DayNumberUnderflow() =>
            Assert.CheckException(typeof(OverflowException), EF.DayNumberUnderflow);
    }
}
