﻿// See LICENSE in the project root for license information.

namespace Abc.Utilities
{
    using System;

    using Xunit;

    public static class GuardTests
    {
        [Fact]
#pragma warning disable CA1806 // Do not ignore method results
        public static void DoesNotThrow() => Guard.NotNull("", "paramName");
#pragma warning restore CA1806

        [Fact]
        public static void Throws()
        {
            // Arrange
            object obj = null!;
            string paramName = "paramName";
            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => Guard.NotNull(obj, paramName));
            // Assert
            Assert.NotNull(ex);
            Assert.Equal(paramName, ex.ParamName);
        }
    }
}