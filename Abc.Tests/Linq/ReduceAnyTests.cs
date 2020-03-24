// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using Xunit;

    using Assert = AssertEx;

    public sealed class ReduceAnyTests : QperatorsTests
    {
        [Fact]
        public static void InvalidArg()
        {
            Assert.ThrowsArgNullEx("source", () => Null.ReduceAny(Kunc<int, int, int>.Any));
            Assert.ThrowsArgNullEx("accumulator", () => NotNull.ReduceAny(Kunc<int, int, int>.Null));

            Assert.ThrowsArgNullEx("source", () => Null.ReduceAny(Kunc<int, int, int>.Any, Funk<Maybe<int>, bool>.Any));
            Assert.ThrowsArgNullEx("accumulator", () => NotNull.ReduceAny(Kunc<int, int, int>.Null, Funk<Maybe<int>, bool>.Any));
            Assert.ThrowsArgNullEx("predicate", () => NotNull.ReduceAny(Kunc<int, int, int>.Any, Funk<Maybe<int>, bool>.Null));
        }
    }
}
