// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

namespace Abc.Utilities
{
    using System;

    using Xunit;

    using Assert = AssertEx;

    public static class GuardTests
    {
        [Fact]
        public static void NotNull_DoesNotThrow() =>
            Guard.NotNull(String.Empty, "paramName");

        [Fact]
        public static void NotNull_Throws()
        {
            // Arrange
            object obj = null!;
            string paramName = "paramName";
            // Act & Assert
            Assert.ThrowsAnexn(paramName, () => Guard.NotNull(obj, paramName));
        }

        [Fact]
#pragma warning disable CA1806 // Do not ignore method results
        public static void NotNullPassThru_DoesNotThrow() =>
            Guard.NotNullPassThru(String.Empty, "paramName");
#pragma warning restore CA1806

        [Fact]
        public static void NotNullPassThru_Throws()
        {
            // Arrange
            object obj = null!;
            string paramName = "paramName";
            // Act & Assert
            Assert.ThrowsAnexn(paramName, () => Guard.NotNullPassThru(obj, paramName));
        }
    }
}
