// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Xunit;

    using Assert = AssertEx;

    public sealed partial class SelectAnyTests : QperatorsTests { }

    // Arg check.
    public partial class SelectAnyTests
    {
        [Fact]
        public static void NullSource() =>
            Assert.ThrowsAnexn("source", () => NullSeq.SelectAny(Kunc<int, int>.Any));

        [Fact]
        public static void NullSelector() =>
            Assert.ThrowsAnexn("selector", () => AnySeq.SelectAny(Kunc<int, int>.Null));
    }

    // Deferred execution.
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

    public partial class SelectAnyTests
    {
        [Fact]
        public void Array_OnlySome()
        {
            // Arrange
            int[] source = new[] { 1, 2, 3, 4, 5 };
            int[] expected = new[] { 2, 3, 4, 5, 6 };
            // Act
            var q = source.SelectAny(i => Maybe.Some(i + 1));
            // Assert
            Assert.Equal(expected, q);
        }

        //[Fact]
        //public void List_OnlySome()
        //{
        //    // Arrange
        //    var source = new List<int> { 1, 2, 3, 4, 5 };
        //    var expected = new List<int> { 2, 3, 4, 5, 6 };
        //    // Act
        //    var q = source.SelectAny(i => Maybe.Some(i + 1));
        //    // Assert
        //    Assert.Equal(source, q);
        //}

        //[Fact]
        //public void ReadOnlyCollection_OnlySome()
        //{
        //    // Arrange
        //    var source = new ReadOnlyCollection<int>(new List<int> { 1, 2, 3, 4, 5 });
        //    var expected = new List<int> { 2, 3, 4, 5, 6 };
        //    // Act
        //    var q = source.SelectAny(i => Maybe.Some(i + 1));
        //    // Assert
        //    Assert.Equal(source, q);
        //}
    }
}
