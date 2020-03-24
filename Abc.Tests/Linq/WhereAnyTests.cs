// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using System;
    using System.Linq;

    using Xunit;

    using Assert = AssertEx;

    public sealed class WhereAnyTests : QperatorsTests
    {
        [Fact]
        public static void InvalidArg()
        {
            Assert.ThrowsArgNullEx("source", () => Null.WhereAny(Kunc<int, bool>.Any));
            Assert.ThrowsArgNullEx("predicate", () => NotNull.WhereAny(Kunc<int, bool>.Null));
        }

        [Fact]
        public static void Deferred()
        {
            bool notCalled = true;
            Func<Maybe<bool>> fun = () => { notCalled = false; return Maybe.Of(true); };
            var q = Enumerable.Repeat(fun, 1).WhereAny(f => f());

            Assert.True(notCalled);
            Assert.CalledOnNext(q, ref notCalled);
        }
    }
}
