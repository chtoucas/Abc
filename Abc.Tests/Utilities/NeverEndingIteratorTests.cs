// See LICENSE.txt in the project root for license information.

namespace Abc.Utilities
{
    using System.Collections;
    using System.Collections.Generic;

    using Xunit;

    using Assert = AssertEx;

    public static class NeverEndingIteratorTests
    {
        private static readonly AnyT Value;
        private static readonly IEnumerable<AnyT> Iter;

#pragma warning disable CA1810 // Initialize reference type static fields inline
        static NeverEndingIteratorTests()
#pragma warning restore CA1810
        {
            var anyT = AnyT.New();
            Value = anyT.Value;
            Iter = anyT.Some.Yield();
        }

        [Fact]
        public static void GetEnumerator()
        {
            // Arrange
            IEnumerator<AnyT> enumerator = Iter.GetEnumerator();
            // Act & Assert
            Assert.Same(Iter, enumerator);
        }

        [Fact]
        public static void GetEnumerator_Untyped()
        {
            // Arrange
            IEnumerator enumerator = Iter.GetEnumerator();
            // Act & Assert
            Assert.Same(Iter, enumerator);
        }

        [Fact]
        public static void Current()
        {
            // Arrange
            IEnumerator<AnyT> enumerator = Iter.GetEnumerator();

            // Act & Assert
            // Even before MoveNext(), Current already returns Value.
            Assert.Same(Value, enumerator.Current);

            for (int i = 0; i < 100; i++)
            {
                Assert.True(enumerator.MoveNext());
                Assert.Same(Value, enumerator.Current);
            }
        }

        [Fact]
        public static void Current_Untyped()
        {
            // Arrange
            IEnumerator enumerator = Iter.GetEnumerator();

            // Act & Assert
            // Even before MoveNext(), Current already returns Value.
            Assert.Same(Value, enumerator.Current);

            for (int i = 0; i < 100; i++)
            {
                Assert.True(enumerator.MoveNext());
                Assert.Same(Value, enumerator.Current);
            }
        }

        [Fact]
        public static void MoveNext()
        {
            // Arrange
            IEnumerator enumerator = Iter.GetEnumerator();
            // Act & Assert
            Assert.True(enumerator.MoveNext());
        }

        [Fact]
        public static void Reset()
        {
            // Arrange
            IEnumerator enumerator = Iter.GetEnumerator();

            // Act & Assert
            for (int i = 0; i < 100; i++)
            {
                Assert.True(enumerator.MoveNext());
                Assert.Same(Value, enumerator.Current);
            }

            // Reset() does nothing.
            enumerator.Reset();

            for (int i = 0; i < 100; i++)
            {
                Assert.True(enumerator.MoveNext());
                Assert.Same(Value, enumerator.Current);
            }
        }

        [Fact]
        public static void Dispose()
        {
            // Arrange
            IEnumerator<AnyT> enumerator = Iter.GetEnumerator();

            // Act & Assert
            for (int i = 0; i < 100; i++)
            {
                Assert.True(enumerator.MoveNext());
                Assert.Same(Value, enumerator.Current);
            }

            // Dispose() does nothing.
            enumerator.Dispose();

            for (int i = 0; i < 100; i++)
            {
                Assert.True(enumerator.MoveNext());
                Assert.Same(Value, enumerator.Current);
            }
        }
    }
}
