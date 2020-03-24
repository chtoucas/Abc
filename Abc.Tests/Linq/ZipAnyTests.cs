// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using Xunit;

    using Assert = AssertEx;

    public sealed class ZipAnyTests : QperatorsTests
    {
        [Fact]
        public static void InvalidArg()
        {
            Assert.ThrowsArgNullEx("first", () => Null.ZipAny(NotNull, Kunc<int, int, int>.Any));
            Assert.ThrowsArgNullEx("second", () => NotNull.ZipAny(Null, Kunc<int, int, int>.Any));
            Assert.ThrowsArgNullEx("resultSelector", () => NotNull.ZipAny(NotNull, Kunc<int, int, int>.Null));
        }
    }
}
