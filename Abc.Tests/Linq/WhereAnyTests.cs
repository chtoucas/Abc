// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using System;
    using System.Linq;

    using Xunit;

    using Assert = AssertEx;

    public sealed partial class WhereAnyTests : QperatorsTests { }

    // Arg check.
    public partial class WhereAnyTests
    {
        [Fact]
        public static void NullSource() =>
            Assert.ThrowsArgNullEx("source", () => Null.WhereAny(Kunc<int, bool>.Any));

        [Fact]
        public static void NullPredicate() =>
            Assert.ThrowsArgNullEx("predicate", () => NotNull.WhereAny(Kunc<int, bool>.Null));
    }

    public partial class WhereAnyTests
    {
        [Fact]
        public static void IsDeferred()
        {
            bool notCalled = true;
            Func<Maybe<bool>> fun = () => { notCalled = false; return Maybe.Of(true); };
            var q = Enumerable.Repeat(fun, 1).WhereAny(f => f());

            Assert.True(notCalled);
            Assert.CalledOnNext(q, ref notCalled);
        }
    }
}
