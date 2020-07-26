// See LICENSE in the project root for license information.

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
