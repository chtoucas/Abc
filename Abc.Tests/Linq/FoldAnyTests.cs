// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using Xunit;

    using Assert = AssertEx;

    public sealed class FoldAnyTests : QperatorsTests
    {
        [Fact]
        public static void FoldAny_InvalidArg()
        {
            Assert.ThrowsArgNullEx("source", () => Qperators.FoldAny(Null, 1, Kunc<int, int, int>.Any));
            Assert.ThrowsArgNullEx("accumulator", () => Qperators.FoldAny(NotNull, 1, Kunc<int, int, int>.Null));

            Assert.ThrowsArgNullEx("source", () => Qperators.FoldAny(Null, 1, Kunc<int, int, int>.Any, Funk<Maybe<int>, bool>.Any));
            Assert.ThrowsArgNullEx("accumulator", () => Qperators.FoldAny(NotNull, 1, Kunc<int, int, int>.Null, Funk<Maybe<int>, bool>.Any));
            Assert.ThrowsArgNullEx("predicate", () => Qperators.FoldAny(NotNull, 1, Kunc<int, int, int>.Any, Funk<Maybe<int>, bool>.Null));
        }
    }
}
