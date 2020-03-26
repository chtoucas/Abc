// See LICENSE.txt in the project root for license information.

namespace Abc.Utilities
{
    using System;
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
            AsEnumerableT = anyT.Some.ToEnumerable();
        }

        private static IDisposable AsDisposable => (IDisposable)AsEnumerableT;
        private static IEnumerator AsEnumerator => AsEnumerableT.GetEnumerator();
        private static IEnumerator<AnyT> AsEnumeratorT => AsEnumerableT.GetEnumerator();
        private static IEnumerable AsEnumerable => AsEnumerableT;

        [Fact]
        public static void GetEnumerator() => Assert.Same(AsEnumerableT, AsEnumerableT.GetEnumerator());

        [Fact]
        public static void GetEnumerator_Untyped() => Assert.Same(AsEnumerableT, AsEnumerable.GetEnumerator());

        // TODO: to be improved.
        // Current is in fact constant...
        [Fact]
        public static void Current() => Assert.Same(Value, AsEnumeratorT.Current);

        [Fact]
        public static void Current_Untyped() => Assert.Same(Value, AsEnumerator.Current);

        [Fact]
        public static void MoveNext() => Assert.True(AsEnumerator.MoveNext());

        // Reset() does nothing, in particular it does not throw.
        [Fact]
        public static void Reset() => AsEnumerator.Reset();

        // Dispose() does nothing, in particular it does not throw.
        [Fact]
        public static void Dispose() => AsDisposable.Dispose();
    }
}
