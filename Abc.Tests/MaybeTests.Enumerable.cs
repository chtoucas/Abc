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
        public static void EmptyEnumerable() =>
            Assert.Some(Enumerable.Empty<int>(), Maybe.EmptyEnumerable<int>());

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

        [Fact]
        public static void CollectAny_WithEmpty() =>
            Assert.Empty(Maybe.CollectAny(Enumerable.Empty<Maybe<int>>()));

        // Start w/ None, end w/ None.
        private static IEnumerable<Maybe<int>> Seq_None2None
        {
            get
            {
                yield return Maybe<int>.None;
                yield return Maybe<int>.None;
                yield return Maybe.Some(1);
                yield return Maybe.Some(2);
                yield return Maybe<int>.None;
            }
        }

        // Start w/ None, end w/ Some.
        private static IEnumerable<Maybe<int>> Seq_None2Some
        {
            get
            {
                yield return Maybe<int>.None;
                yield return Maybe.Some(1);
                yield return Maybe<int>.None;
                yield return Maybe.Some(1);
                yield return Maybe<int>.None;
                yield return Maybe<int>.None;
                yield return Maybe.Some(3);
            }
        }

        // Start w/ Some, end w/ None.
        private static IEnumerable<Maybe<int>> Seq_Some2None
        {
            get
            {
                yield return Maybe.Some(1);
                yield return Maybe<int>.None;
                yield return Maybe<int>.None;
                yield return Maybe.Some(2);
                yield return Maybe<int>.None;
            }
        }

        // Start w/ Some, end w/ Some.
        private static IEnumerable<Maybe<int>> Seq_Some2Some
        {
            get
            {
                yield return Maybe.Some(1);
                yield return Maybe.Some(2);
                yield return Maybe<int>.None;
                yield return Maybe<int>.None;
                yield return Maybe.Some(3);
            }
        }

        // Only Some.
        private static IEnumerable<Maybe<int>> Seq_OnlySome
        {
            get
            {
                yield return Maybe.Some(1);
                yield return Maybe.Some(2);
                yield return Maybe.Some(3);
                yield return Maybe.Some(314);
                yield return Maybe.Some(413);
                yield return Maybe.Some(7);
                yield return Maybe.Some(5);
                yield return Maybe.Some(3);
            }
        }

        // Only None.
        private static IEnumerable<Maybe<int>> Seq_OnlyNone
        {
            get
            {
                yield return Maybe<int>.None;
                yield return Maybe<int>.None;
                yield return Maybe<int>.None;
                yield return Maybe<int>.None;
                yield return Maybe<int>.None;
                yield return Maybe<int>.None;
                yield return Maybe<int>.None;
            }
        }

        [Fact]
        public static void CollectAny_None2None()
        {
            // Arrange
            var expected = new List<int> { 1, 2 };
            // Act & Assert
            Assert.Equal(expected, Maybe.CollectAny(Seq_None2None));
        }

        [Fact]
        public static void CollectAny_None2Some()
        {
            // Arrange
            var expected = new List<int> { 1, 1, 3 };
            // Act & Assert
            Assert.Equal(expected, Maybe.CollectAny(Seq_None2Some));
        }

        [Fact]
        public static void CollectAny_Some2None()
        {
            // Arrange
            var expected = new List<int> { 1, 2 };
            // Act & Assert
            Assert.Equal(expected, Maybe.CollectAny(Seq_Some2None));
        }

        [Fact]
        public static void CollectAny_Some2Some()
        {
            // Arrange
            var expected = new List<int> { 1, 2, 3 };
            // Act & Assert
            Assert.Equal(expected, Maybe.CollectAny(Seq_Some2Some));
        }

        [Fact]
        public static void CollectAny_OnlySome()
        {
            // Arrange
            var expected = new List<int> { 1, 2, 3, 314, 413, 7, 5, 3 };
            // Act & Assert
            Assert.Equal(expected, Maybe.CollectAny(Seq_OnlySome));
        }

        [Fact]
        public static void CollectAny_OnlyNone() =>
            Assert.Empty(Maybe.CollectAny(Seq_OnlyNone));
    }
}
