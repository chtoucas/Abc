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
        private static readonly IEnumerable<AnyT> AsEnumerableT;

#pragma warning disable CA1810 // Initialize reference type static fields inline
        static NeverEndingIteratorTests()
#pragma warning restore CA1810
        {
            var anyT = AnyT.New();
            Value = anyT.Value;
            AsEnumerableT = anyT.Some.Yield();
        }

        private static IEnumerable AsEnumerable => AsEnumerableT;
        private static IEnumerator<AnyT> AsEnumeratorT => AsEnumerableT.GetEnumerator();
        private static IEnumerator AsEnumerator => AsEnumerableT.GetEnumerator();

        [Fact]
        public static void GetEnumerator()
            => Assert.Same(AsEnumerableT, AsEnumerableT.GetEnumerator());

        [Fact]
        public static void GetEnumerator_Untyped()
            => Assert.Same(AsEnumerableT, AsEnumerable.GetEnumerator());

        [Fact]
        public static void Current()
        {
            var enumeratorT = AsEnumeratorT;

            // Even before MoveNext(), Current already returns Value.
            Assert.Same(Value, enumeratorT.Current);

            for (int i = 0; i < 100; i++)
            {
                Assert.True(enumeratorT.MoveNext());
                Assert.Same(Value, enumeratorT.Current);
            }
        }

        [Fact]
        public static void Dispose()
        {
            var enumeratorT = AsEnumeratorT;

            for (int i = 0; i < 100; i++)
            {
                Assert.True(enumeratorT.MoveNext());
                Assert.Same(Value, enumeratorT.Current);
            }

            // Dispose() does nothing.
            enumeratorT.Dispose();

            for (int i = 0; i < 100; i++)
            {
                Assert.True(enumeratorT.MoveNext());
                Assert.Same(Value, enumeratorT.Current);
            }
        }

        [Fact]
        public static void Current_Untyped()
        {
            var enumerator = AsEnumerator;

            // Even before MoveNext(), Current already returns Value.
            Assert.Same(Value, enumerator.Current);

            for (int i = 0; i < 100; i++)
            {
                Assert.True(enumerator.MoveNext());
                Assert.Same(Value, enumerator.Current);
            }
        }

        [Fact]
        public static void MoveNext() => Assert.True(AsEnumerator.MoveNext());

        [Fact]
        public static void Reset()
        {
            var enumerator = AsEnumerator;

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
    }
}
