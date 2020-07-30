// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

namespace Abc.Utilities
{
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
        [Fact] public static void YearOverflow() => Assert.CheckException(EF.YearOverflow);

        [Fact] public static void YearOverflowOrUnderflow() => Assert.CheckException(EF.YearOverflowOrUnderflow);

        [Fact] public static void YearUnderflow() => Assert.CheckException(EF.YearUnderflow);

        [Fact] public static void DayNumberOverflow() => Assert.CheckException(EF.DayNumberOverflow);

        [Fact] public static void DayNumberOverflowOrUnderflow() => Assert.CheckException(EF.DayNumberOverflowOrUnderflow);

        [Fact] public static void DayNumberUnderflow() => Assert.CheckException(EF.DayNumberUnderflow);
    }
}
