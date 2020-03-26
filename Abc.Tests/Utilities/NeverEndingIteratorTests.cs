// See LICENSE.txt in the project root for license information.

namespace Abc.Utilities
{
    using System.Collections.Generic;

    using Xunit;

    using Assert = AssertEx;

    public static class NeverEndingIteratorTests
    {
        private static readonly AnyT Value;
        private static readonly IEnumerator<AnyT> AsEnumerator;
        private static readonly IEnumerable<AnyT> AsEnumerable;

#pragma warning disable CA1810 // Initialize reference type static fields inline
        static NeverEndingIteratorTests()
#pragma warning restore CA1810
        {
            var any = AnyT.New();
            Value = any.Value;
            AsEnumerator = any.Some.GetEnumerator();
            AsEnumerable = any.Some.ToEnumerable();
        }

        // TODO: to be improved.
        // Current is in fact constant...
        [Fact]
        public static void Current() => Assert.Same(Value, AsEnumerator.Current);

        [Fact]
        public static void GetEnumerator() => Assert.Same(AsEnumerable, AsEnumerable.GetEnumerator());

        [Fact]
        public static void Reset() => AsEnumerator.Reset();
    }
}
