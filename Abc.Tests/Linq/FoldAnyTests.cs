// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using Xunit;

    using Assert = AssertEx;

    public sealed partial class FoldAnyTests : QperatorsTests { }

    // Arg check.
    public partial class FoldAnyTests
    {
        [Fact]
        public static void NullSource() =>
            Assert.ThrowsArgNullEx("source", () =>
                Null.FoldAny(1, Kunc<int, int, int>.Any));

        [Fact]
        public static void NullAccumulator() =>
            Assert.ThrowsArgNullEx("accumulator", () =>
                NotNull.FoldAny(1, Kunc<int, int, int>.Null));

        [Fact]
        public static void NullSource_WithPredicate() =>
            Assert.ThrowsArgNullEx("source", () =>
                Null.FoldAny(1, Kunc<int, int, int>.Any, Funk<Maybe<int>, bool>.Any));

        [Fact]
        public static void NullAccumulator_WithPredicate() =>
            Assert.ThrowsArgNullEx("accumulator", () =>
                NotNull.FoldAny(1, Kunc<int, int, int>.Null, Funk<Maybe<int>, bool>.Any));

        [Fact]
        public static void NullPredicate() =>
            Assert.ThrowsArgNullEx("predicate", () =>
                NotNull.FoldAny(1, Kunc<int, int, int>.Any, Funk<Maybe<int>, bool>.Null));
    }
}
