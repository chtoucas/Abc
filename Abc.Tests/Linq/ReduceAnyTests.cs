// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using Xunit;

    using Assert = AssertEx;

    public sealed class ReduceAnyTests : QperatorsTests
    {
        [Fact]
        public static void ReduceAny_InvalidArg()
        {
            Assert.ThrowsArgNullEx("source", () => Qperators.ReduceAny(Null, Kunc<int, int, int>.Any));
            Assert.ThrowsArgNullEx("accumulator", () => Qperators.ReduceAny(NotNull, Kunc<int, int, int>.Null));

            Assert.ThrowsArgNullEx("source", () => Qperators.ReduceAny(Null, Kunc<int, int, int>.Any, Funk<Maybe<int>, bool>.Any));
            Assert.ThrowsArgNullEx("accumulator", () => Qperators.ReduceAny(NotNull, Kunc<int, int, int>.Null, Funk<Maybe<int>, bool>.Any));
            Assert.ThrowsArgNullEx("predicate", () => Qperators.ReduceAny(NotNull, Kunc<int, int, int>.Any, Funk<Maybe<int>, bool>.Null));
        }
    }
}
