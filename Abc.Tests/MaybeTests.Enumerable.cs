// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System.Collections.Generic;
    using System.Linq;

    using Xunit;

    using Assert = AssertEx;

    // Helpers for Maybe<IEnumerable<T>>.
    public partial class MaybeTests
    {
        [Fact]
        public static void EmptyEnumerable()
        {
            // TODO: a better test whould not check the reference equality
            // but the equality of both sequences.
            Assert.Some(Enumerable.Empty<int>(), Maybe.EmptyEnumerable<int>());
        }

        [Fact]
        public static void CollectAny_NullSource() =>
            Assert.ThrowsAnexn("source", () =>
                Maybe.CollectAny(default(IEnumerable<Maybe<int>>)!));

        [Fact]
        public static void CollectAny_IsDeferred()
        {
            // Arrange
            IEnumerable<Maybe<int>> source = new ThrowingCollection<Maybe<int>>();
            // Act
            var q = Maybe.CollectAny(source);
            // Assert
            Assert.ThrowsOnNext(q);
        }

        // TODO: non-empty test.

        [Fact]
        public static void CollectAny_WithEmpty() =>
            Assert.Empty(Maybe.CollectAny(Enumerable.Empty<Maybe<int>>()));
    }
}
