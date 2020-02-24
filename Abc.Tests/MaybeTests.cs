// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using Xunit;

    using static global::My;

    using Assert = AssertEx;

    public static partial class MaybeTests
    {
        private static readonly Maybe<int> Ø = Maybe<int>.None;
        private static readonly Maybe<int> One = Maybe.Some(1);
        private static readonly Maybe<int> Two = Maybe.Some(2);
    }

    // Construction, properties
    public partial class MaybeTests
    {
        [Fact]
        public static void Default()
        {
            Assert.None(default(Maybe<Unit>));
            Assert.None(default(Maybe<int>));
            Assert.None(default(Maybe<string>));
        }

        [Fact]
        public static void Unit()
        {
            Assert.Some(Maybe.Unit);
        }

        [Fact]
        public static void None()
        {
            Assert.None(Maybe.None);
        }

        [Fact]
        public static void IsNone()
        {
            Assert.True(Maybe<int>.None.IsNone);
            Assert.True(Maybe<int?>.None.IsNone);
            Assert.True(Maybe<string>.None.IsNone);
            Assert.True(Maybe<string?>.None.IsNone);
        }

        [Fact]
        public static void Some()
        {
            Assert.Some(Maybe.Some(1));
        }

        [Fact]
        public static void Of()
        {
            Assert.None(Maybe.Of((int?)null));
            Assert.None(Maybe.Of((string?)null));
            Assert.None(Maybe.Of(NullString));

            Assert.Some(Maybe.Of(1));
            Assert.Some(Maybe.Of((int?)1));
            Assert.Some(Maybe.Of("value"));
            Assert.Some(Maybe.Of((string?)"value"));
        }
    }

    // Iterable.
    public partial class MaybeTests
    {
        //[Fact]
        //public static void ToEnumerable()
        //{
        //    // Arrange
        //    var some = Maybe.Of("value");
        //    var none = Maybe<string>.None;
        //    // Act & Assert
        //    Assert.Equal(Enumerable.Repeat("value", 1), some.ToEnumerable());
        //    Assert.Empty(none.ToEnumerable());
        //}

        [Fact]
        public static void GetEnumerator_Some()
        {
            var some = Maybe.Of("value");
            int count = 0;

            foreach (string x in some) { count++; Assert.Equal("value", x); }

            Assert.Equal(1, count);
        }

        [Fact]
        public static void GetEnumerator_None()
        {
            var none = Maybe<string>.None;
            int count = 0;

            foreach (string x in none) { count++; }

            Assert.Equal(0, count);
        }
    }

    // LINQ
    public partial class MaybeTests
    {
        [Fact]
        public static void Select()
        {
            Assert.Some(2, One.Select(x => 2 * x));
            Assert.Some(2, from x in One select 2 * x);

            Assert.None(Ø.Select(x => 2 * x));
            Assert.None(from x in Ø select 2 * x);
        }

        [Fact]
        public static void Where()
        {
            Assert.None(One.Where(x => x == 2));
            Assert.None(from x in One where x == 2 select x);

            Assert.Some(1, One.Where(x => x == 1));
            Assert.Some(1, from x in One where x == 1 select x);

            Assert.None(Ø.Where(x => x == 2));
            Assert.None(from x in Ø where x == 2 select x);

            Assert.None(Ø.Where(x => x == 1));
            Assert.None(from x in Ø where x == 1 select x);
        }

        [Fact]
        public static void SelectMany()
        {
            Assert.None(Ø.SelectMany(i => Maybe.Some(2 * i), (i, j) => i + j));
            Assert.None(from i in Ø from j in Maybe.Some(2 * i) select i + j);

            Assert.None(Ø.SelectMany(_ => Ø, (i, j) => i + j));
            Assert.None(from i in Ø from j in Ø select i + j);

            Assert.None(One.SelectMany(_ => Ø, (i, j) => i + j));
            Assert.None(from i in One from j in Ø select i + j);
        }

        [Fact]
        public static void Join()
        {
            Assert.None(One.Join(Two, i => i, i => i, (i, j) => i + j));
            Assert.None(from i in One join j in Two on i equals j select i + j);
        }
    }

    public partial class MaybeTests
    {
        [Fact]
        public static void ReplaceWith()
        {
            // Arrange
            var some = Maybe.Unit;

            // Act & Assert
            Assert.Some("value", some.ReplaceWith("value"));
            Assert.None(Ø.ReplaceWith("value"));

            Assert.None(some.ReplaceWith(NullString));
            Assert.None(Ø.ReplaceWith(NullString));

#nullable disable
            Assert.Some(2, some.ReplaceWith((int?)2));
            Assert.None(Ø.ReplaceWith((int?)2));

            Assert.None(some.ReplaceWith(NullNullString));
            Assert.None(Ø.ReplaceWith(NullNullString));
#nullable restore
        }
    }
}
