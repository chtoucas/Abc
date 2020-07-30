// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

namespace Abc.Utilities
{
    using Xunit;

    public static partial class MathOperationsTests { }

    // Euclidian division.
    public partial class MathOperationsTests
    {
        [Theory(DisplayName = "Divide() follows mathematical rules, it rounds towards minus infinity.")]
        [InlineData(4, 1)]
        [InlineData(3, 1)]
        [InlineData(2, 0)]
        [InlineData(1, 0)]
        [InlineData(0, 0)]
        [InlineData(-1, -1)]
        [InlineData(-2, -1)]
        [InlineData(-3, -1)]
        [InlineData(-4, -2)]
        public static void Divide(int m, int q) =>
            Assert.Equal(q, MathOperations.Divide(m, 3));

        [Theory(DisplayName = "Divide() w/ rest follows mathematical rules.")]
        [InlineData(4, 1, 1)]
        [InlineData(3, 1, 0)]
        [InlineData(2, 0, 2)]
        [InlineData(1, 0, 1)]
        [InlineData(0, 0, 0)]
        [InlineData(-1, -1, 2)]
        [InlineData(-2, -1, 1)]
        [InlineData(-3, -1, 0)]
        [InlineData(-4, -2, 2)]
        public static void Divide_Rest(int m, int q, int r)
        {
            // Arrange
            int qA = MathOperations.Divide(m, 3, out int rA);
            // Act & Assert
            Assert.Equal(q, qA);
            Assert.Equal(r, rA);
        }

        [Theory(DisplayName = "Divide(long) follows mathematical rules.")]
        [InlineData(4L, 1L)]
        [InlineData(3L, 1L)]
        [InlineData(2L, 0L)]
        [InlineData(1L, 0L)]
        [InlineData(0L, 0L)]
        [InlineData(-1L, -1L)]
        [InlineData(-2L, -1L)]
        [InlineData(-3L, -1L)]
        [InlineData(-4L, -2L)]
        public static void DivideLong(long m, long q) =>
            Assert.Equal(q, MathOperations.Divide(m, 3L));

        [Theory(DisplayName = "Divide(long) w/ rest follows mathematical rules.")]
        [InlineData(4L, 1L, 1L)]
        [InlineData(3L, 1L, 0L)]
        [InlineData(2L, 0L, 2L)]
        [InlineData(1L, 0L, 1L)]
        [InlineData(0L, 0L, 0L)]
        [InlineData(-1L, -1L, 2L)]
        [InlineData(-2L, -1L, 1L)]
        [InlineData(-3L, -1L, 0L)]
        [InlineData(-4L, -2L, 2L)]
        public static void DivideLong_Rest(long m, long q, long r)
        {
            // Arrange
            long qA = MathOperations.Divide(m, 3, out long rA);
            // Act & Assert
            Assert.Equal(q, qA);
            Assert.Equal(r, rA);
        }

        [Theory(DisplayName = "Modulo() follows mathematical rules.")]
        [InlineData(4, 1)]
        [InlineData(3, 0)]
        [InlineData(2, 2)]
        [InlineData(1, 1)]
        [InlineData(0, 0)]
        [InlineData(-1, 2)]
        [InlineData(-2, 1)]
        [InlineData(-3, 0)]
        [InlineData(-4, 2)]
        public static void Modulo(int m, int n) =>
            Assert.Equal(n, MathOperations.Modulo(m, 3));

        [Theory(DisplayName = "Modulo(long) follows mathematical rules.")]
        [InlineData(4L, 1L)]
        [InlineData(3L, 0L)]
        [InlineData(2L, 2L)]
        [InlineData(1L, 1L)]
        [InlineData(0L, 0L)]
        [InlineData(-1L, 2L)]
        [InlineData(-2L, 1L)]
        [InlineData(-3L, 0L)]
        [InlineData(-4L, 2L)]
        public static void ModuloLong(long m, long n) =>
            Assert.Equal(n, MathOperations.Modulo(m, 3L));

        [Theory]
        [InlineData(4, 1)]
        [InlineData(3, 3)]
        [InlineData(2, 2)]
        [InlineData(1, 1)]
        [InlineData(0, 3)]
        [InlineData(-1, 2)]
        [InlineData(-2, 1)]
        [InlineData(-3, 3)]
        [InlineData(-4, 2)]
        public static void AdjustedModulo(int m, int n) =>
            Assert.Equal(n, MathOperations.AdjustedModulo(m, 3));

        [Theory]
        [InlineData(4L, 1L)]
        [InlineData(3L, 3L)]
        [InlineData(2L, 2L)]
        [InlineData(1L, 1L)]
        [InlineData(0L, 3L)]
        [InlineData(-1L, 2L)]
        [InlineData(-2L, 1L)]
        [InlineData(-3L, 3L)]
        [InlineData(-4L, 2L)]
        public static void AdjustedModuloLong(long m, long n) =>
            Assert.Equal(n, MathOperations.AdjustedModulo(m, 3));
    }
}
