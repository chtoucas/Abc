// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using Xunit;

    using Assert = AssertEx;

    public sealed class ZipAnyTests : QperatorsTests
    {
        [Fact]
        public static void ZipAny_InvalidArg()
        {
            Assert.ThrowsArgNullEx("first", () => Qperators.ZipAny(Null, NotNull, Kunc<int, int, int>.Any));
            Assert.ThrowsArgNullEx("second", () => Qperators.ZipAny(NotNull, Null, Kunc<int, int, int>.Any));
            Assert.ThrowsArgNullEx("resultSelector", () => Qperators.ZipAny(NotNull, NotNull, Kunc<int, int, int>.Null));
        }
    }
}
