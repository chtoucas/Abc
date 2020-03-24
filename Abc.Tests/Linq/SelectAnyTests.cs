// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using System;
    using System.Linq;

    using Xunit;

    using Assert = AssertEx;

    public sealed class SelectAnyTests : QperatorsTests
    {
        [Fact]
        public static void SelectAny_InvalidArg()
        {
            Assert.ThrowsArgNullEx("source", () => Qperators.SelectAny(Null, Kunc<int, int>.Any));
            Assert.ThrowsArgNullEx("selector", () => Qperators.SelectAny(NotNull, Kunc<int, int>.Null));
        }

        [Fact]
        public static void SelectAny_Deferred()
        {
            bool notCalled = true;
            Func<Maybe<int>> fun = () => { notCalled = false; return Maybe.Of(1); };
            var q = Enumerable.Repeat(fun, 1).SelectAny(f => f());

            Assert.True(notCalled);
            Assert.CalledOnNext(q, ref notCalled);
        }
    }
}
