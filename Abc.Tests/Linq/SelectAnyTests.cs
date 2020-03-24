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
        public static void InvalidArg()
        {
            Assert.ThrowsArgNullEx("source", () => Null.SelectAny(Kunc<int, int>.Any));
            Assert.ThrowsArgNullEx("selector", () => NotNull.SelectAny(Kunc<int, int>.Null));
        }

        [Fact]
        public static void Deferred()
        {
            bool notCalled = true;
            Func<Maybe<int>> fun = () => { notCalled = false; return Maybe.Of(1); };
            var q = Enumerable.Repeat(fun, 1).SelectAny(f => f());

            Assert.True(notCalled);
            Assert.CalledOnNext(q, ref notCalled);
        }
    }
}
