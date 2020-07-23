// See LICENSE in the project root for license information.

namespace Abc.Utilities
{
    using System.Collections;
    using System.Collections.Generic;

    using Xunit;

    using Assert = AssertEx;

    public static class NeverEndingSequenceTests
    {
        [Fact(DisplayName = "GetEnumerator() always returns the same iterator.")]
        public static void GetEnumerator()
        {
            // Arrange
            using var iter = new NeverEndingSequence<AnyT>(new AnyT());
            // Act
            IEnumerator<AnyT> it = iter.GetEnumerator();
            // Assert
            Assert.Same(iter, it);
        }

        [Fact(DisplayName = "GetEnumerator() (untyped) always returns the same iterator.")]
        public static void GetEnumerator_Untyped()
        {
            // Arrange
            using var iter = new NeverEndingSequence<AnyT>(new AnyT());
            // Act
            IEnumerable enumerable = iter;
            IEnumerator it = enumerable.GetEnumerator();
            // Assert
            Assert.Same(iter, it);
        }

        [Fact]
        public static void Current()
        {
            // Arrange
            var value = new AnyT();
            using var iter = new NeverEndingSequence<AnyT>(value);

            // Act
            IEnumerator<AnyT> it = iter.GetEnumerator();

            // Assert
            // Even before the first MoveNext(), Current already returns Value.
            Assert.Same(value, it.Current);

            for (int i = 0; i < 100; i++)
            {
                Assert.True(it.MoveNext());
                Assert.Same(value, it.Current);
            }
        }

        [Fact]
        public static void Current_Untyped()
        {
            // Arrange
            var value = new AnyT();
            using var iter = new NeverEndingSequence<AnyT>(value);

            // Act
            IEnumerator it = iter.GetEnumerator();

            // Assert
            // Even before the first MoveNext(), Current already returns Value.
            Assert.Same(value, it.Current);

            for (int i = 0; i < 100; i++)
            {
                Assert.True(it.MoveNext());
                Assert.Same(value, it.Current);
            }
        }

        [Fact]
        public static void MoveNext()
        {
            // Arrange
            using var iter = new NeverEndingSequence<AnyT>(new AnyT());
            // Act
            IEnumerator it = iter.GetEnumerator();
            // Assert
            Assert.True(it.MoveNext());
        }

        [Fact]
        public static void Reset()
        {
            // Arrange
            var value = new AnyT();
            using var iter = new NeverEndingSequence<AnyT>(value);

            // Act
            IEnumerator it = iter.GetEnumerator();

            // Assert
            for (int i = 0; i < 100; i++)
            {
                Assert.True(it.MoveNext());
                Assert.Same(value, it.Current);
            }

            // Reset() does nothing.
            it.Reset();

            for (int i = 0; i < 100; i++)
            {
                Assert.True(it.MoveNext());
                Assert.Same(value, it.Current);
            }
        }

        [Fact]
        public static void Dispose()
        {
            // Arrange
            var value = new AnyT();
            using var iter = new NeverEndingSequence<AnyT>(value);

            // Act
            IEnumerator<AnyT> it = iter.GetEnumerator();

            // Assert
            for (int i = 0; i < 100; i++)
            {
                Assert.True(it.MoveNext());
                Assert.Same(value, it.Current);
            }

            // Dispose() does nothing.
            it.Dispose();

            for (int i = 0; i < 100; i++)
            {
                Assert.True(it.MoveNext());
                Assert.Same(value, it.Current);
            }
        }
    }
}
