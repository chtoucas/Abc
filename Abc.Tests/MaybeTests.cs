// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Linq;

    using Xunit;

    using static global::My;

    using Assert = AssertEx;

    public static partial class MaybeTests
    {
        private static readonly Maybe<int> Ø = Maybe<int>.None;
        private static readonly Maybe<int> One = Maybe.Some(1);
        private static readonly Maybe<int> Two = Maybe.Some(2);
        private static readonly Maybe<long> ØL = Maybe<long>.None;
        private static readonly Maybe<long> TwoL = Maybe.Some(2L);
    }

    // Construction, properties.
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
        public static void Zero()
        {
            Assert.None(Maybe.Zero);
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
        public static void Of_Reference()
        {
            Assert.None(Maybe.Of((string?)null));
            Assert.None(Maybe.Of(NullString));

            Assert.Some(Maybe.Of("value"));
            Assert.Some(Maybe.Of((string?)"value"));
        }

        [Fact]
        public static void Of_Value()
        {
            Assert.Some(Maybe.Of(1));

#pragma warning disable CS0618 // Type or member is obsolete
            Assert.None(Maybe.Of((int?)null));
            Assert.Some(Maybe.Of((int?)1));
#pragma warning restore CS0618
        }

        [Fact]
        public static void Some()
        {
            Assert.Some(Maybe.Some(1));
        }
    }

    // Core methods.
    public partial class MaybeTests
    {
        [Fact]
        public static void Bind_InvalidBinder()
        {
            Assert.ThrowsArgNullEx("binder", () => Ø.Bind((Func<int, Maybe<string>>)null!));
            Assert.ThrowsArgNullEx("binder", () => One.Bind((Func<int, Maybe<string>>)null!));
        }

        [Fact]
        public static void OrElse()
        {
            // Some Some -> Some
            Assert.Some(1, One.OrElse(Two));
            // Some None -> Some
            Assert.Some(1, One.OrElse(Ø));
            // None Some -> Some
            Assert.Some(2, Ø.OrElse(Two));
            // None None -> None
            Assert.None(Ø.OrElse(Ø));
        }
    }

    // Query Expression Pattern.
    public partial class MaybeTests
    {
        [Fact]
        public static void Select_InvalidSelector()
        {
            Assert.ThrowsArgNullEx("selector", () => Ø.Select((Func<int, string>)null!));
            Assert.ThrowsArgNullEx("selector", () => One.Select((Func<int, string>)null!));
        }

        [Fact]
        public static void Select()
        {
            Assert.None(Ø.Select(x => x));
            Assert.None(from x in Ø select x);

            Assert.Some(2, One.Select(x => 2 * x));
            Assert.Some(2, from x in One select 2 * x);
        }

        [Fact]
        public static void Where_InvalidPredicate()
        {
            Assert.ThrowsArgNullEx("predicate", () => Ø.Where(null!));
            Assert.ThrowsArgNullEx("predicate", () => One.Where(null!));
        }

        [Fact]
        public static void Where()
        {
            // None.Where(false) -> None
            Assert.None(Ø.Where(_ => true));
            Assert.None(from x in Ø where true select x);
            // None.Where(true) -> None
            Assert.None(Ø.Where(_ => false));
            Assert.None(from x in Ø where false select x);

            // Some.Where(false) -> None
            Assert.None(One.Where(x => x == 2));
            Assert.None(from x in One where x == 2 select x);
            // Some.Where(true) -> Some
            Assert.Some(1, One.Where(x => x == 1));
            Assert.Some(1, from x in One where x == 1 select x);
        }

        [Fact]
        public static void SelectMany()
        {
            // None.SelectMany(None) -> None
            Assert.None(Ø.SelectMany(_ => Ø, (i, j) => i + j));
            Assert.None(from i in Ø from j in Ø select i + j);
            // None.SelectMany(Some) -> None
            Assert.None(Ø.SelectMany(i => Maybe.Some(2 * i), (i, j) => i + j));
            Assert.None(from i in Ø from j in Maybe.Some(2 * i) select i + j);
            // Some.SelectMany(None) -> None
            Assert.None(One.SelectMany(_ => Ø, (i, j) => i + j));
            Assert.None(from i in One from j in Ø select i + j);

            // Some.SelectMany(Some) -> Some
            Assert.Some(3, One.SelectMany(i => Maybe.Some(2 * i), (i, j) => i + j));
            Assert.Some(3, from i in One from j in Maybe.Some(2 * i) select i + j);
        }

        [Fact]
        public static void Join()
        {
            Assert.None(One.Join(Two, i => i, i => i, (i, j) => i + j));
            Assert.None(from i in One join j in Two on i equals j select i + j);
        }
    }

    // Misc methods.
    public partial class MaybeTests
    {
        [Fact]
        public static void ZipWith()
        {
            // Some Some -> Some
            Assert.Some(3L, One.ZipWith(TwoL, (i, j) => i + j));
            // Some None -> None
            Assert.None(One.ZipWith(ØL, (i, j) => i + j));
            // None Some -> None
            Assert.None(Ø.ZipWith(TwoL, (i, j) => i + j));
            // None None -> None
            Assert.None(Ø.ZipWith(ØL, (i, j) => i + j));
        }

        [Fact]
        public static void ContinueWith()
        {
            // Some Some -> Some
            Assert.Some(2L, One.ContinueWith(TwoL));
            // Some None -> None
            Assert.None(One.ContinueWith(ØL));
            // None Some -> None
            Assert.None(Ø.ContinueWith(TwoL));
            // None None -> None
            Assert.None(Ø.ContinueWith(ØL));
        }

        [Fact]
        public static void XorElse()
        {
            // Some Some -> None
            Assert.None(One.XorElse(Two));
            // Some None -> Some
            Assert.Some(1, One.XorElse(Ø));
            // None Some -> Some
            Assert.Some(2, Ø.XorElse(Two));
            // None None -> None
            Assert.None(Ø.XorElse(Ø));

            // XorElse() flip to itself.
            Assert.None(Two.XorElse(One));
            Assert.Some(1, Ø.XorElse(One));
            Assert.Some(2, Two.XorElse(Ø));
            Assert.None(Ø.XorElse(Ø));
        }

        [Fact]
        public static void Skip()
        {
            Assert.Equal(Maybe.Zero, Ø.Skip());
            Assert.Equal(Maybe.Unit, One.Skip());
        }
    }

    // Iterable.
    public partial class MaybeTests
    {
        [Fact]
        public static void ToEnumerable()
        {
            // Arrange
            var some = Maybe.Of("value");
            var none = Maybe<string>.None;
            // Act & Assert
            Assert.Equal(Enumerable.Repeat("value", 1), some.ToEnumerable());
            Assert.Empty(none.ToEnumerable());
        }

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
}
