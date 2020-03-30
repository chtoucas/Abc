// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using System;
    using System.Linq;

    using Xunit;

    using Assert = AssertEx;

    public sealed partial class SelectAnyTests : QperatorsTests { }

    // Arg check.
    public partial class SelectAnyTests
    {
        [Fact]
        public static void NullSource() =>
            Assert.ThrowsAnexn("source", () => Null.SelectAny(Kunc<int, int>.Any));

        [Fact]
        public static void NullSelector() =>
            Assert.ThrowsAnexn("selector", () => NotNull.SelectAny(Kunc<int, int>.Null));
    }

    public partial class SelectAnyTests
    {
        [Fact]
        public static void IsDeferred()
        {
            bool notCalled = true;
            Func<Maybe<int>> fun = () => { notCalled = false; return Maybe.Of(1); };
            var q = Enumerable.Repeat(fun, 1).SelectAny(f => f());

            Assert.True(notCalled);
            Assert.CalledOnNext(q, ref notCalled);
        }
    }
}
