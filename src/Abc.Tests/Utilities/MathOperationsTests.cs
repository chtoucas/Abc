// See LICENSE in the project root for license information.

namespace Abc.Utilities
{
    using System;

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

    // Égalité / inégalité entre doubles.
    public partial class MathOperationsTests
    {

    }

    public partial class MathOperationsTests
    {
        public static readonly TheoryData<int, decimal> AddHalfOneData =
            new TheoryData<int, decimal>
            {
                { Int32.MinValue, Int32.MinValue + .5m },
                { MathOperations.HalfOneMin - 1, (MathOperations.HalfOneMin - 1) + .5m },
                { MathOperations.HalfOneMin, MathOperations.HalfOneMin + .5m },
                { MathOperations.HalfOneMin + 1, (MathOperations.HalfOneMin + 1) + .5m },
                { -3, -2.5m },
                { -2, -1.5m },
                { -1,  -.5m },
                {  0,   .5m },
                {  1,  1.5m },
                {  2,  2.5m },
                {  3,  3.5m },
                { MathOperations.HalfOneMax - 1, (MathOperations.HalfOneMax - 1) + .5m },
                { MathOperations.HalfOneMax, MathOperations.HalfOneMax + .5m },
                { MathOperations.HalfOneMax + 1, (MathOperations.HalfOneMax + 1) + .5m },
                { Int32.MaxValue, Int32.MaxValue + .5m },
            };

        public static readonly TheoryData<int, decimal> SubtractHalfOneData =
            new TheoryData<int, decimal>
            {
                { Int32.MinValue, Int32.MinValue - .5m },
                { MathOperations.HalfOneMin - 1, (MathOperations.HalfOneMin - 1) - .5m },
                { MathOperations.HalfOneMin, MathOperations.HalfOneMin - .5m },
                { MathOperations.HalfOneMin + 1, (MathOperations.HalfOneMin + 1) - .5m },
                { -3, -3.5m },
                { -2, -2.5m },
                { -1, -1.5m },
                {  0,  -.5m },
                {  1,   .5m },
                {  2,  1.5m },
                {  3,  2.5m },
                { MathOperations.HalfOneMax - 1, (MathOperations.HalfOneMax - 1) - .5m },
                { MathOperations.HalfOneMax, MathOperations.HalfOneMax - .5m },
                { MathOperations.HalfOneMax + 1, (MathOperations.HalfOneMax + 1) - .5m },
                { Int32.MaxValue, Int32.MaxValue - .5m },
            };

        [Theory, MemberData(nameof(AddHalfOneData))]
        public static void AddHalfOne(int num, decimal exp) =>
            Assert.Equal(exp, MathOperations.AddHalfOne(num));

        [Theory, MemberData(nameof(SubtractHalfOneData))]
        public static void SubtractHalfOne(int num, decimal exp) =>
            Assert.Equal(exp, MathOperations.SubtractHalfOne(num));
    }

    // Logarithme décimal.
    public partial class MathOperationsTests
    {
        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 0)]
        [InlineData(3, 0)]
        [InlineData(4, 0)]
        [InlineData(5, 0)]
        [InlineData(6, 0)]
        [InlineData(7, 0)]
        [InlineData(8, 0)]
        [InlineData(9, 0)]
        [InlineData(10, 1)]
        [InlineData(11, 1)]
        [InlineData(19, 1)]
        [InlineData(20, 1)]
        [InlineData(99, 1)]
        [InlineData(100, 2)]
        [InlineData(999, 2)]
        [InlineData(1000, 3)]
        [InlineData(9999, 3)]
        [InlineData(10_000, 4)]
        [InlineData(100_000, 5)]
        [InlineData(1_000_000, 6)]
        [InlineData(10_000_000, 7)]
        [InlineData(100_000_000, 8)]
        [InlineData(1_000_000_000, 9)]
        [InlineData(Int32.MaxValue, 9)]
        public static void Log10(int num, int logE)
        {
            // Act
            int log = MathOperations.Log10(num);
            // Assert
            Assert.Equal(logE, log);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(3, 1)]
        [InlineData(4, 1)]
        [InlineData(5, 1)]
        [InlineData(6, 1)]
        [InlineData(7, 1)]
        [InlineData(8, 1)]
        [InlineData(9, 1)]
        [InlineData(10, 2)]
        [InlineData(11, 2)]
        [InlineData(19, 2)]
        [InlineData(20, 2)]
        [InlineData(99, 2)]
        [InlineData(100, 3)]
        [InlineData(999, 3)]
        [InlineData(1000, 4)]
        [InlineData(9999, 4)]
        [InlineData(10_000, 5)]
        [InlineData(100_000, 6)]
        [InlineData(1_000_000, 7)]
        [InlineData(10_000_000, 8)]
        [InlineData(100_000_000, 9)]
        [InlineData(1_000_000_000, 10)]
        [InlineData(Int32.MaxValue, 10)]
        public static void AdjustedLog10(int num, int alogE)
        {
            // Act
            int alog = MathOperations.AdjustedLog10(num);
            // Assert
            Assert.Equal(alogE, alog);
        }

        [Fact]
        public static void AdjustedLog10_Faster()
        {
            // Act
            int alog1 = MathOperations.AdjustedLog10(10_001, 1, 1);
            int alog2 = MathOperations.AdjustedLog10(10_001, 10, 2);
            int alog3 = MathOperations.AdjustedLog10(10_001, 100, 3);
            int alog4 = MathOperations.AdjustedLog10(10_001, 1000, 4);
            int alog5 = MathOperations.AdjustedLog10(10_001, 10_000, 5);
            // Assert
            Assert.Equal(5, alog1);
            Assert.Equal(5, alog2);
            Assert.Equal(5, alog3);
            Assert.Equal(5, alog4);
            Assert.Equal(5, alog5);
        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(2, 1, 1)]
        [InlineData(3, 1, 1)]
        [InlineData(4, 1, 1)]
        [InlineData(5, 1, 1)]
        [InlineData(6, 1, 1)]
        [InlineData(7, 1, 1)]
        [InlineData(8, 1, 1)]
        [InlineData(9, 1, 1)]
        [InlineData(10, 2, 10)]
        [InlineData(11, 2, 10)]
        [InlineData(19, 2, 10)]
        [InlineData(20, 2, 10)]
        [InlineData(99, 2, 10)]
        [InlineData(100, 3, 100)]
        [InlineData(999, 3, 100)]
        [InlineData(1000, 4, 1000)]
        [InlineData(9999, 4, 1000)]
        [InlineData(10_000, 5, 10_000)]
        [InlineData(100_000, 6, 100_000)]
        [InlineData(1_000_000, 7, 1_000_000)]
        [InlineData(10_000_000, 8, 10_000_000)]
        [InlineData(100_000_000, 9, 100_000_000)]
        [InlineData(1_000_000_000, 10, 1_000_000_000)]
        [InlineData(Int32.MaxValue, 10, 1_000_000_000)]
        public static void AdjustedLog10_Pow(int num, int logE, int powE)
        {
            // Act
            int log = MathOperations.AdjustedLog10(num, out int pow);
            // Assert
            Assert.Equal(logE, log);
            Assert.Equal(powE, pow);
        }

        [Fact]
        public static void AdjustedLog10_Pow_Faster()
        {
            // Act
            int alog1 = MathOperations.AdjustedLog10(10_001, 1, 1, out int pow1);
            int alog2 = MathOperations.AdjustedLog10(10_001, 10, 2, out int pow2);
            int alog3 = MathOperations.AdjustedLog10(10_001, 100, 3, out int pow3);
            int alog4 = MathOperations.AdjustedLog10(10_001, 1000, 4, out int pow4);
            int alog5 = MathOperations.AdjustedLog10(10_001, 10_000, 5, out int pow5);
            // Assert
            Assert.Equal(5, alog1);
            Assert.Equal(5, alog2);
            Assert.Equal(5, alog3);
            Assert.Equal(5, alog4);
            Assert.Equal(5, alog5);
            Assert.Equal(10_000, pow1);
            Assert.Equal(10_000, pow2);
            Assert.Equal(10_000, pow3);
            Assert.Equal(10_000, pow4);
            Assert.Equal(10_000, pow5);
        }
    }
}
