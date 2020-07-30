// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

namespace Abc.Utilities
{
    using System;

    using Xunit;

    public static class RequireTests
    {
        [Fact]
        public static void DoesNotThrow() => Require.NotNull("", "paramName");

        [Fact]
        public static void Throws()
        {
            // Arrange
            object obj = null!;
            string paramName = "paramName";
            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => Require.NotNull(obj, paramName));
            // Assert
            Assert.NotNull(ex);
            Assert.Equal(paramName, ex.ParamName);
        }
    }
}
