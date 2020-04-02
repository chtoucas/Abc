// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Xunit;

    using Assert = AssertEx;

    public sealed partial class SelectAnyTests : QperatorsTests
    {
        private static Maybe<int> ReturnNone(int i) => Maybe<int>.None;
        private static Maybe<int> AddOne(int i) => Maybe.Some(i + 1);
        private static Maybe<int> AddOneUnless3(int i) =>
            i == 3 ? Maybe<int>.None : Maybe.Some(i + 1);
    }

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

    // Selector returns None, always.
    public partial class SelectAnyTests
    {
        [Fact]
        public void WithArray_OnlyNone()
        {
            // Arrange
            int[] source = new[] { 1, 2, 3, 4, 5 };
            // Act
            var q = source.SelectAny(ReturnNone);
            // Assert
            Assert.Empty(q);
        }

        [Fact]
        public void WithList_OnlyNone()
        {
            // Arrange
            var source = new List<int> { 1, 2, 3, 4, 5 };
            // Act
            var q = source.SelectAny(ReturnNone);
            // Assert
            Assert.Empty(q);
        }

        [Fact]
        public void WithReadOnlyCollection_OnlyNone()
        {
            // Arrange
            var source = new ReadOnlyCollection<int>(new List<int> { 1, 2, 3, 4, 5 });
            // Act
            var q = source.SelectAny(ReturnNone);
            // Assert
            Assert.Empty(q);
        }

        [Fact]
        public void WithCollection_OnlyNone()
        {
            // Arrange
            var source = new LinkedList<int>(new List<int> { 1, 2, 3, 4, 5 });
            // Act
            var q = source.SelectAny(ReturnNone);
            // Assert
            Assert.Empty(q);
        }

        [Fact]
        public void WithEnumerable_OnlyNone()
        {
            // Arrange
            var source = Enumerable.Range(1, 5);
            // Act
            var q = source.SelectAny(ReturnNone);
            // Assert
            Assert.Empty(q);
        }
    }

    // Selector returns Some, always.
    public partial class SelectAnyTests
    {
        [Fact]
        public void WithArray_OnlySome()
        {
            // Arrange
            int[] source = new[] { 1, 2, 3, 4, 5 };
            int[] expected = new[] { 2, 3, 4, 5, 6 };
            // Act
            var q = source.SelectAny(AddOne);
            // Assert
            Assert.Equal(expected, q);
        }

        [Fact]
        public void WithList_OnlySome()
        {
            // Arrange
            var source = new List<int> { 1, 2, 3, 4, 5 };
            var expected = new List<int> { 2, 3, 4, 5, 6 };
            // Act
            var q = source.SelectAny(AddOne);
            // Assert
            Assert.Equal(expected, q);
        }

        [Fact]
        public void WithReadOnlyCollection_OnlySome()
        {
            // Arrange
            var source = new ReadOnlyCollection<int>(new List<int> { 1, 2, 3, 4, 5 });
            var expected = new List<int> { 2, 3, 4, 5, 6 };
            // Act
            var q = source.SelectAny(AddOne);
            // Assert
            Assert.Equal(expected, q);
        }

        [Fact]
        public void WithCollection_OnlySome()
        {
            // Arrange
            var source = new LinkedList<int>(new List<int> { 1, 2, 3, 4, 5 });
            var expected = new List<int> { 2, 3, 4, 5, 6 };
            // Act
            var q = source.SelectAny(AddOne);
            // Assert
            Assert.Equal(expected, q);
        }

        [Fact]
        public void WithEnumerable_OnlySome()
        {
            // Arrange
            var source = Enumerable.Range(1, 5);
            var expected = new List<int> { 2, 3, 4, 5, 6 };
            // Act
            var q = source.SelectAny(AddOne);
            // Assert
            Assert.Equal(expected, q);
        }
    }

    // Selector returns Some, always.
    public partial class SelectAnyTests
    {
        [Fact]
        public void WithArray_Mixed()
        {
            // Arrange
            int[] source = new[] { 1, 2, 3, 4, 5 };
            int[] expected = new[] { 2, 3, 5, 6 };
            // Act
            var q = source.SelectAny(AddOneUnless3);
            // Assert
            Assert.Equal(expected, q);
        }

        [Fact]
        public void WithList_Mixed()
        {
            // Arrange
            var source = new List<int> { 1, 2, 3, 4, 5 };
            var expected = new List<int> { 2, 3, 5, 6 };
            // Act
            var q = source.SelectAny(AddOneUnless3);
            // Assert
            Assert.Equal(expected, q);
        }

        [Fact]
        public void WithReadOnlyCollection_Mixed()
        {
            // Arrange
            var source = new ReadOnlyCollection<int>(new List<int> { 1, 2, 3, 4, 5 });
            var expected = new List<int> { 2, 3, 5, 6 };
            // Act
            var q = source.SelectAny(AddOneUnless3);
            // Assert
            Assert.Equal(expected, q);
        }

        [Fact]
        public void WithCollection_Mixed()
        {
            // Arrange
            var source = new LinkedList<int>(new List<int> { 1, 2, 3, 4, 5 });
            var expected = new List<int> { 2, 3, 5, 6 };
            // Act
            var q = source.SelectAny(AddOneUnless3);
            // Assert
            Assert.Equal(expected, q);
        }

        [Fact]
        public void WithEnumerable_Mixed()
        {
            // Arrange
            var source = Enumerable.Range(1, 5);
            var expected = new List<int> { 2, 3, 5, 6 };
            // Act
            var q = source.SelectAny(AddOneUnless3);
            // Assert
            Assert.Equal(expected, q);
        }
    }

    // Returns default after enumeration.
    public partial class SelectAnyTests
    {
        [Fact]
        public void WithArray_CurrentIsDefault_AfterEnumeration()
        {
            // Arrange
            int[] source = new[] { 1 };
            // Act
            IEnumerable<int> q = source.SelectAny(AddOne);
            var enumerator = q.GetEnumerator();
            while (enumerator.MoveNext()) { }
            // Assert
            Assert.Equal(default, enumerator.Current);
        }

        [Fact]
        public void WithList_CurrentIsDefault_AfterEnumeration()
        {
            // Arrange
            var source = new List<int> { 1 };
            // Act
            IEnumerable<int> q = source.SelectAny(AddOne);
            var enumerator = q.GetEnumerator();
            while (enumerator.MoveNext()) { }
            // Assert
            Assert.Equal(default, enumerator.Current);
        }

        [Fact]
        public void WithReadOnlyCollection_CurrentIsDefault_AfterEnumeration()
        {
            // Arrange
            var source = new ReadOnlyCollection<int>(new List<int> { 1 });
            // Act
            IEnumerable<int> q = source.SelectAny(AddOne);
            var enumerator = q.GetEnumerator();
            while (enumerator.MoveNext()) { }
            // Assert
            Assert.Equal(default, enumerator.Current);
        }

        [Fact]
        public void WithCollection_CurrentIsDefault_AfterEnumeration()
        {
            // Arrange
            var source = new LinkedList<int>(new List<int> { 1 });
            // Act
            IEnumerable<int> q = source.SelectAny(AddOne);
            var enumerator = q.GetEnumerator();
            while (enumerator.MoveNext()) { }
            // Assert
            Assert.Equal(default, enumerator.Current);
        }

        [Fact]
        public void WithEnumerable_CurrentIsDefault_AfterEnumeration()
        {
            // Arrange
            var source = Enumerable.Range(1, 1);
            // Act
            IEnumerable<int> q = source.SelectAny(AddOne);
            var enumerator = q.GetEnumerator();
            while (enumerator.MoveNext()) { }
            // Assert
            Assert.Equal(default, enumerator.Current);
        }
    }

    // Call SelectAny() twice in a row.
    public partial class SelectAnyTests
    {
        [Fact]
        public void WithArray_Twice()
        {
            // Arrange
            int[] source = new[] { 1, 2, 3, 4, 5 };
            int[] expected = new[] { 3, 6, 7 };
            // Act
            var q = source.SelectAny(AddOneUnless3).SelectAny(AddOneUnless3);
            // Assert
            Assert.Equal(expected, q);
        }

        [Fact]
        public void WithList_Twice()
        {
            // Arrange
            var source = new List<int> { 1, 2, 3, 4, 5 };
            var expected = new List<int> { 3, 6, 7 };
            // Act
            var q = source.SelectAny(AddOneUnless3).SelectAny(AddOneUnless3);
            // Assert
            Assert.Equal(expected, q);
        }

        [Fact]
        public void WithReadOnlyCollection_Twice()
        {
            // Arrange
            var source = new ReadOnlyCollection<int>(new List<int> { 1, 2, 3, 4, 5 });
            var expected = new List<int> { 3, 6, 7 };
            // Act
            var q = source.SelectAny(AddOneUnless3).SelectAny(AddOneUnless3);
            // Assert
            Assert.Equal(expected, q);
        }

        [Fact]
        public void WithCollection_Twice()
        {
            // Arrange
            var source = new LinkedList<int>(new List<int> { 1, 2, 3, 4, 5 });
            var expected = new List<int> { 3, 6, 7 };
            // Act
            var q = source.SelectAny(AddOneUnless3).SelectAny(AddOneUnless3);
            // Assert
            Assert.Equal(expected, q);
        }

        [Fact]
        public void WithEnumerable_Twice()
        {
            // Arrange
            var source = Enumerable.Range(1, 5);
            var expected = new List<int> { 3, 6, 7 };
            // Act
            var q = source.SelectAny(AddOneUnless3).SelectAny(AddOneUnless3);
            // Assert
            Assert.Equal(expected, q);
        }
    }
}
