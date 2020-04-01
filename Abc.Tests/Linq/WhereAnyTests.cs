// See LICENSE.dotnet.txt in the project root for license information.
//
// Largely inspired by
// https://github.com/dotnet/runtime/blob/master/src/libraries/System.Linq/tests/WhereTests.cs

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
            Assert.ThrowsAnexn("source", () => NullSeq.WhereAny(Kunc<int, bool>.Any));

        [Fact]
        public static void NullPredicate() =>
            Assert.ThrowsAnexn("predicate", () => AnySeq.WhereAny(Kunc<int, bool>.Null));
    }

    // Deferred execution.
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

    public partial class WhereAnyTests
    {
        [Fact]
        public void Array_TruePredicate()
        {
            // Arrange
            int[] source = new[] { 1, 2, 3, 4, 5 };
            // Act
            var q = source.WhereAny(_ => Maybe.True);
            // Assert
            Assert.Equal(source, q);
        }

        [Fact]
        public void Array_FalsePredicate()
        {
            // Arrange
            int[] source = new[] { 1, 2, 3, 4, 5 };
            // Act
            var q = source.WhereAny(_ => Maybe.False);
            // Assert
            Assert.Empty(q);
        }
    }
}
