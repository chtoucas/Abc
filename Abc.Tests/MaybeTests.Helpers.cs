// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System.Collections.Generic;
    using System.Linq;

    using Xunit;

    using Assert = AssertEx;

    // Helpers for Maybe<T> where T is a struct.
    public partial class MaybeTests
    {
        [Fact]
        public static void Squash()
        {
            // Arrange
            var none = Ø.Select(x => (int?)x);
            var one = One.Select(x => (int?)x);
            // Act & Assert
            Assert.Equal(Ø, none.Squash());
            Assert.Equal(One, one.Squash());
        }

        [Fact]
        public static void ToNullable()
        {
            // Arrange
            var none = Ø.Select(x => (int?)x);
            var one = One.Select(x => (int?)x);
            // Act & Assert
            Assert.Null(Ø.ToNullable());
            Assert.Equal(1, One.ToNullable());
            Assert.Null(none.ToNullable());
            Assert.Equal(1, one.ToNullable());
        }
    }

    // Helpers for Maybe<Unit>.
    public partial class MaybeTests
    {
        [Fact]
        public static void Unit() => Assert.Some(Abc.Unit.Default, Maybe.Unit);

        [Fact]
        public static void Zero() => Assert.None(Maybe.Zero);
        [Fact]
        public static void Guard()
        {
            Assert.Equal(Maybe.Zero, Maybe.Guard(false));
            Assert.Equal(Maybe.Unit, Maybe.Guard(true));
        }
    }

    // Helpers for Maybe<bool>.
    public partial class MaybeTests
    {
        [Fact]
        public static void True() => Assert.Some(Maybe.True);

        [Fact]
        public static void False() => Assert.Some(Maybe.False);

        [Fact]
        public static void Unknown() => Assert.None(Maybe.Unknown);

        [Fact]
        public static void Negate()
        {
            Assert.Some(false, Maybe.True.Negate());
            Assert.Some(true, Maybe.False.Negate());
            Assert.None(Maybe.Unknown.Negate());
        }

        [Fact]
        public static void Or()
        {
            Assert.Some(true, Maybe.True.Or(Maybe.True));
            Assert.Some(true, Maybe.True.Or(Maybe.False));
            Assert.Some(true, Maybe.True.Or(Maybe.Unknown));

            Assert.Some(true, Maybe.False.Or(Maybe.True));
            Assert.Some(false, Maybe.False.Or(Maybe.False));
            Assert.None(Maybe.False.Or(Maybe.Unknown));

            Assert.Some(true, Maybe.Unknown.Or(Maybe.True));
            Assert.None(Maybe.Unknown.Or(Maybe.False));
            Assert.None(Maybe.Unknown.Or(Maybe.Unknown));
        }

        [Fact]
        public static void And()
        {
            Assert.Some(true, Maybe.True.And(Maybe.True));
            Assert.Some(false, Maybe.True.And(Maybe.False));
            Assert.None(Maybe.True.And(Maybe.Unknown));

            Assert.Some(false, Maybe.False.And(Maybe.True));
            Assert.Some(false, Maybe.False.And(Maybe.False));
            Assert.Some(false, Maybe.False.And(Maybe.Unknown));

            Assert.None(Maybe.Unknown.And(Maybe.True));
            Assert.Some(false, Maybe.Unknown.And(Maybe.False));
            Assert.None(Maybe.Unknown.And(Maybe.Unknown));
        }
    }

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
        public static void CollectAny_Deferred()
        {
            IEnumerable<Maybe<int>> source = new ThrowingEnumerable<Maybe<int>>();

            var q = Maybe.CollectAny(source);
            Assert.ThrowsOnNext(q);
        }
    }
}
