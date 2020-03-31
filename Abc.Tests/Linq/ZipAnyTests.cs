// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using Xunit;

    using Assert = AssertEx;

    public sealed partial class ZipAnyTests : QperatorsTests { }

    // Arg check.
    public partial class ZipAnyTests
    {
        [Fact]
        public static void NullFirst() =>
            Assert.ThrowsAnexn("first", () =>
                NullSeq.ZipAny(AnySeq, Kunc<int, int, int>.Any));

        [Fact]
        public static void NullSecond() =>
            Assert.ThrowsAnexn("second", () =>
                AnySeq.ZipAny(NullSeq, Kunc<int, int, int>.Any));

        [Fact]
        public static void NullResultSelector() =>
            Assert.ThrowsAnexn("resultSelector", () =>
                AnySeq.ZipAny(AnySeq, Kunc<int, int, int>.Null));
    }
}
