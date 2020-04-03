// See LICENSE in the project root for license information.

namespace Abc
{
    using System;

    using Xunit;

    using Assert = AssertEx;

    // Query Expression Pattern.

    // Select()
    public partial class MaybeTests
    {
        [Fact]
        public static void Select_None_NullSelector()
        {
            Assert.ThrowsAnexn("selector", () => Ø.Select(Funk<int, AnyResult>.Null));
            Assert.ThrowsAnexn("selector", () => NoText.Select(Funk<string, AnyResult>.Null));
            Assert.ThrowsAnexn("selector", () => NoUri.Select(Funk<Uri, AnyResult>.Null));
            Assert.ThrowsAnexn("selector", () => AnyT.None.Select(Funk<AnyT, AnyResult>.Null));
        }

        [Fact]
        public static void Select_Some_NullSelector()
        {
            Assert.ThrowsAnexn("selector", () => One.Select(Funk<int, AnyResult>.Null));
            Assert.ThrowsAnexn("selector", () => SomeText.Select(Funk<string, AnyResult>.Null));
            Assert.ThrowsAnexn("selector", () => SomeUri.Select(Funk<Uri, AnyResult>.Null));
            Assert.ThrowsAnexn("selector", () => AnyT.Some.Select(Funk<AnyT, AnyResult>.Null));
        }

        [Fact]
        public static void Select_None()
        {
            Assert.None(Ø.Select(Funk<int, int>.Any));
            Assert.None(from x in Ø select x);
        }

        [Fact]
        public static void Select_SomeInt32()
        {
            Assert.Some(6, Two.Select(Times3));
            Assert.Some(6, from x in Two select 3 * x);
        }

        [Fact]
        public static void Select_SomeInt64()
        {
            Assert.Some(8L, TwoL.Select(Times4));
            Assert.Some(8L, from x in TwoL select 4L * x);
        }

        [Fact]
        public static void Select_SomeUri()
        {
            Assert.Some(MyUri.AbsoluteUri, SomeUri.Select(GetAbsoluteUri));
            Assert.Some(MyUri.AbsoluteUri, from x in SomeUri select x.AbsoluteUri);
        }
    }

    // Where()
    public partial class MaybeTests
    {
        [Fact]
        public static void Where_None_NullPredicate()
        {
            Assert.ThrowsAnexn("predicate", () => Ø.Where(null!));
            Assert.ThrowsAnexn("predicate", () => NoText.Where(null!));
            Assert.ThrowsAnexn("predicate", () => NoUri.Where(null!));
            Assert.ThrowsAnexn("predicate", () => AnyT.None.Where(null!));
        }

        [Fact]
        public static void Where_Some_NullPredicate()
        {
            Assert.ThrowsAnexn("predicate", () => One.Where(null!));
            Assert.ThrowsAnexn("predicate", () => SomeText.Where(null!));
            Assert.ThrowsAnexn("predicate", () => SomeUri.Where(null!));
            Assert.ThrowsAnexn("predicate", () => AnyT.Some.Where(null!));
        }

        [Fact]
        public static void Where_None()
        {
            Assert.None(Ø.Where(Funk<int, bool>.Any));

            Assert.None(from x in Ø where true select x);
            Assert.None(from x in Ø where false select x);
        }

        [Fact]
        public static void Where_Some()
        {
            // Some.Where(false) -> None
            Assert.None(One.Where(x => x == 2));
            Assert.None(from x in One where x == 2 select x);

            // Some.Where(true) -> Some
            Assert.Some(1, One.Where(x => x == 1));
            Assert.Some(1, from x in One where x == 1 select x);
        }
    }

    // SelectMany()
    public partial class MaybeTests
    {
        [Fact]
        public static void SelectMany_None_NullSelector()
        {
            Assert.ThrowsAnexn("selector", () =>
                Ø.SelectMany(Kunc<int, AnyT1>.Null, Funk<int, AnyT1, AnyResult>.Any));
            Assert.ThrowsAnexn("selector", () =>
                NoText.SelectMany(Kunc<string, AnyT1>.Null, Funk<string, AnyT1, AnyResult>.Any));
            Assert.ThrowsAnexn("selector", () =>
                NoUri.SelectMany(Kunc<Uri, AnyT1>.Null, Funk<Uri, AnyT1, AnyResult>.Any));
            Assert.ThrowsAnexn("selector", () =>
                AnyT.None.SelectMany(Kunc<AnyT, AnyT1>.Null, Funk<AnyT, AnyT1, AnyResult>.Any));
        }

        [Fact]
        public static void SelectMany_Some_NullSelector()
        {
            Assert.ThrowsAnexn("selector", () =>
                One.SelectMany(Kunc<int, AnyT1>.Null, Funk<int, AnyT1, AnyResult>.Any));
            Assert.ThrowsAnexn("selector", () =>
                SomeText.SelectMany(Kunc<string, AnyT1>.Null, Funk<string, AnyT1, AnyResult>.Any));
            Assert.ThrowsAnexn("selector", () =>
                SomeUri.SelectMany(Kunc<Uri, AnyT1>.Null, Funk<Uri, AnyT1, AnyResult>.Any));
            Assert.ThrowsAnexn("selector", () =>
                AnyT.Some.SelectMany(Kunc<AnyT, AnyT1>.Null, Funk<AnyT, AnyT1, AnyResult>.Any));
        }

        [Fact]
        public static void SelectMany_None_NullResultSelector()
        {
            Assert.ThrowsAnexn("resultSelector", () =>
                Ø.SelectMany(Kunc<int, AnyT1>.Any, Funk<int, AnyT1, AnyResult>.Null));
            Assert.ThrowsAnexn("resultSelector", () =>
                NoText.SelectMany(Kunc<string, AnyT1>.Any, Funk<string, AnyT1, AnyResult>.Null));
            Assert.ThrowsAnexn("resultSelector", () =>
                NoUri.SelectMany(Kunc<Uri, AnyT1>.Any, Funk<Uri, AnyT1, AnyResult>.Null));
            Assert.ThrowsAnexn("resultSelector", () =>
                AnyT.None.SelectMany(Kunc<AnyT, AnyT1>.Any, Funk<AnyT, AnyT1, AnyResult>.Null));
        }

        [Fact]
        public static void SelectMany_Some_NullResultSelector()
        {
            Assert.ThrowsAnexn("resultSelector", () =>
                One.SelectMany(Kunc<int, AnyT1>.Any, Funk<int, AnyT1, AnyResult>.Null));
            Assert.ThrowsAnexn("resultSelector", () =>
                SomeText.SelectMany(Kunc<string, AnyT1>.Any, Funk<string, AnyT1, AnyResult>.Null));
            Assert.ThrowsAnexn("resultSelector", () =>
                SomeUri.SelectMany(Kunc<Uri, AnyT1>.Any, Funk<Uri, AnyT1, AnyResult>.Null));
            Assert.ThrowsAnexn("resultSelector", () =>
                AnyT.Some.SelectMany(Kunc<AnyT, AnyT1>.Any, Funk<AnyT, AnyT1, AnyResult>.Null));
        }

        [Fact]
        public static void SelectMany_None()
        {
            // None.SelectMany(None) -> None
            Assert.None(Ø.SelectMany(_ => Ø, (i, j) => i + j));
            Assert.None(from i in Ø from j in Ø select i + j);

            // None.SelectMany(Some) -> None
            Assert.None(Ø.SelectMany(i => Maybe.Some(2 * i), (i, j) => i + j));
            Assert.None(from i in Ø from j in Maybe.Some(2 * i) select i + j);
        }

        [Fact]
        public static void SelectMany_Some()
        {
            // Some.SelectMany(None) -> None
            Assert.None(One.SelectMany(_ => Ø, (i, j) => i + j));
            Assert.None(from i in One from j in Ø select i + j);

            // Some.SelectMany(Some) -> Some
            Assert.Some(3, One.SelectMany(i => Maybe.Some(2 * i), (i, j) => i + j));
            Assert.Some(3, from i in One from j in Maybe.Some(2 * i) select i + j);
        }
    }

    // Join()
    public partial class MaybeTests
    {
        [Fact]
        public static void Join_None_NullOuterKeySelector()
        {
            Assert.ThrowsAnexn("outerKeySelector", () =>
                Ø.Join(AnyT1.Some,
                    Funk<int, AnyT2>.Null, Funk<AnyT1, AnyT2>.Null, Funk<int, AnyT1, AnyResult>.Any));
            // With a comparer.
            Assert.ThrowsAnexn("outerKeySelector", () =>
                Ø.Join(AnyT1.Some,
                    Funk<int, AnyT2>.Null, Funk<AnyT1, AnyT2>.Null, Funk<int, AnyT1, AnyResult>.Any, null));
        }

        [Fact]
        public static void Join_Some_NullOuterKeySelector()
        {
            Assert.ThrowsAnexn("outerKeySelector", () =>
                One.Join(TwoL,
                    Funk<int, AnyT2>.Null, Funk<long, AnyT2>.Any, Funk<int, long, AnyResult>.Any));
            // With a comparer.
            Assert.ThrowsAnexn("outerKeySelector", () =>
                One.Join(TwoL,
                    Funk<int, AnyT2>.Null, Funk<long, AnyT2>.Any, Funk<int, long, AnyResult>.Any, null));
        }

        [Fact]
        public static void Join_None_NullInnerKeySelector()
        {
            Assert.ThrowsAnexn("innerKeySelector", () =>
                Ø.Join(TwoL,
                    Funk<int, AnyT2>.Any, Funk<long, AnyT2>.Null, Funk<int, long, AnyResult>.Any));
            // With a comparer.
            Assert.ThrowsAnexn("innerKeySelector", () =>
                Ø.Join(TwoL,
                    Funk<int, AnyT2>.Any, Funk<long, AnyT2>.Null, Funk<int, long, AnyResult>.Any, null));
        }

        [Fact]
        public static void Join_Some_NullInnerKeySelector()
        {
            Assert.ThrowsAnexn("innerKeySelector", () =>
                One.Join(TwoL,
                    Funk<int, AnyT2>.Any, Funk<long, AnyT2>.Null, Funk<int, long, AnyResult>.Any));
            // With a comparer.
            Assert.ThrowsAnexn("innerKeySelector", () =>
                One.Join(TwoL,
                    Funk<int, AnyT2>.Any, Funk<long, AnyT2>.Null, Funk<int, long, AnyResult>.Any, null));
        }

        [Fact]
        public static void Join_None_NullResultSelector()
        {
            Assert.ThrowsAnexn("resultSelector", () =>
                Ø.Join(TwoL,
                    Funk<int, AnyT2>.Any, Funk<long, AnyT2>.Any, Funk<int, long, AnyResult>.Null));
            // With a comparer.
            Assert.ThrowsAnexn("resultSelector", () =>
                Ø.Join(TwoL,
                    Funk<int, AnyT2>.Any, Funk<long, AnyT2>.Any, Funk<int, long, AnyResult>.Null, null));
        }

        [Fact]
        public static void Join_Some_NullResultSelector()
        {
            Assert.ThrowsAnexn("resultSelector", () =>
                One.Join(TwoL,
                    Funk<int, AnyT2>.Any, Funk<long, AnyT2>.Any, Funk<int, long, AnyResult>.Null));
            // With a comparer.
            Assert.ThrowsAnexn("resultSelector", () =>
                One.Join(TwoL,
                    Funk<int, AnyT2>.Any, Funk<long, AnyT2>.Any, Funk<int, long, AnyResult>.Null, null));
        }

        [Fact]
        public static void Join_None()
        {
            Assert.None(Ø.Join(One, Ident, Ident, (i, j) => i + j));
            Assert.None(from i in Ø join j in One on i equals j select i + j);
        }

        [Fact]
        public static void Join_Some_WithNone()
        {
            Assert.None(One.Join(Ø, Ident, Ident, (i, j) => i + j));
            Assert.None(from i in One join j in Ø on i equals j select i + j);
        }

        [Fact]
        public static void Join_Some_WithSome_Unmatched()
        {
            Assert.None(One.Join(Two, Ident, Ident, (i, j) => i + j));
            Assert.None(from i in One join j in Two on i equals j select i + j);
        }

        [Fact]
        public static void Join_Some_WithSome_Matched()
        {
            Assert.Some(2, One.Join(One, Ident, Ident, (i, j) => i + j));
            Assert.Some(2, from i in One join j in One on i equals j select i + j);

            Assert.Some(3, One.Join(Two, x => 2 * x, Ident, (i, j) => i + j));
            Assert.Some(3, from i in One join j in Two on 2 * i equals j select i + j);

            var outer = Maybe.Some(3);
            var inner = Maybe.Some(5);
            Assert.Some(8, outer.Join(inner, x => 5 * x, x => 3 * x, (i, j) => i + j));
            Assert.Some(8, from i in outer join j in inner on 5 * i equals 3 * j select i + j);
        }

        [Fact]
        public static void Join_WithNullComparer()
        {
            // Arrange
            var outer = Maybe.SomeOrNone("XXX");
            var inner = Maybe.SomeOrNone("YYY");
            // Act
            var q = outer.Join(inner, Ident, Ident, (x, y) => $"{x} = {y}", null);
            // Assert
            Assert.None(q);
        }

        [Fact]
        public static void Join_WithComparer()
        {
            // Arrange
            var outer = Maybe.SomeOrNone(Anagram);
            var inner = Maybe.SomeOrNone(Margana);
            string expected = $"{Anagram} est un anagramme de {Margana}";
            // Act
            var q = outer.Join(inner, Ident, Ident,
                (x, y) => $"{x} est un anagramme de {y}",
                new AnagramEqualityComparer());
            // Assert
            Assert.Some(expected, q);
        }
    }

    // GroupJoin()
    public partial class MaybeTests
    {
        [Fact]
        public static void GroupJoin_None_NullOuterKeySelector()
        {
            Assert.ThrowsAnexn("outerKeySelector", () =>
                Ø.GroupJoin(AnyT1.Some,
                    Funk<int, AnyT2>.Null, Funk<AnyT1, AnyT2>.Null, Funk<int, Maybe<AnyT1>, AnyResult>.Any));
            // With a comparer.
            Assert.ThrowsAnexn("outerKeySelector", () =>
                Ø.GroupJoin(AnyT1.Some,
                    Funk<int, AnyT2>.Null, Funk<AnyT1, AnyT2>.Null, Funk<int, Maybe<AnyT1>, AnyResult>.Any, null));
        }

        [Fact]
        public static void GroupJoin_Some_NullOuterKeySelector()
        {
            Assert.ThrowsAnexn("outerKeySelector", () =>
                One.GroupJoin(AnyT1.Some,
                    Funk<int, AnyT2>.Null, Funk<AnyT1, AnyT2>.Any, Funk<int, Maybe<AnyT1>, AnyResult>.Any));
            // With a comparer.
            Assert.ThrowsAnexn("outerKeySelector", () =>
                One.GroupJoin(AnyT1.Some,
                    Funk<int, AnyT2>.Null, Funk<AnyT1, AnyT2>.Any, Funk<int, Maybe<AnyT1>, AnyResult>.Any, null));
        }

        [Fact]
        public static void GroupJoin_None_NullInnerKeySelector()
        {
            Assert.ThrowsAnexn("innerKeySelector", () =>
                Ø.GroupJoin(AnyT1.Some,
                    Funk<int, AnyT2>.Any, Funk<AnyT1, AnyT2>.Null, Funk<int, Maybe<AnyT1>, AnyResult>.Any));
            // With a comparer.
            Assert.ThrowsAnexn("innerKeySelector", () =>
                Ø.GroupJoin(AnyT1.Some,
                    Funk<int, AnyT2>.Any, Funk<AnyT1, AnyT2>.Null, Funk<int, Maybe<AnyT1>, AnyResult>.Any, null));
        }

        [Fact]
        public static void GroupJoin_Some_NullInnerKeySelector()
        {
            Assert.ThrowsAnexn("innerKeySelector", () =>
                One.GroupJoin(AnyT1.Some,
                    Funk<int, AnyT2>.Any, Funk<AnyT1, AnyT2>.Null, Funk<int, Maybe<AnyT1>, AnyResult>.Any));
            // With a comparer.
            Assert.ThrowsAnexn("innerKeySelector", () =>
                One.GroupJoin(AnyT1.Some,
                    Funk<int, AnyT2>.Any, Funk<AnyT1, AnyT2>.Null, Funk<int, Maybe<AnyT1>, AnyResult>.Any, null));
        }

        [Fact]
        public static void GroupJoin_None_NullResultSelector()
        {
            Assert.ThrowsAnexn("resultSelector", () =>
                Ø.GroupJoin(AnyT1.Some,
                    Funk<int, AnyT2>.Any, Funk<AnyT1, AnyT2>.Any, Funk<int, Maybe<AnyT1>, AnyResult>.Null));
            // With a comparer.
            Assert.ThrowsAnexn("resultSelector", () =>
                Ø.GroupJoin(AnyT1.Some,
                    Funk<int, AnyT2>.Any, Funk<AnyT1, AnyT2>.Any, Funk<int, Maybe<AnyT1>, AnyResult>.Null, null));
        }

        [Fact]
        public static void GroupJoin_Some_NullResultSelector()
        {
            Assert.ThrowsAnexn("resultSelector", () =>
                One.GroupJoin(AnyT1.Some,
                    Funk<int, AnyT2>.Any, Funk<AnyT1, AnyT2>.Any, Funk<int, Maybe<AnyT1>, AnyResult>.Null));
            // With a comparer.
            Assert.ThrowsAnexn("resultSelector", () =>
                One.GroupJoin(AnyT1.Some,
                    Funk<int, AnyT2>.Any, Funk<AnyT1, AnyT2>.Any, Funk<int, Maybe<AnyT1>, AnyResult>.Null, null));
        }

        [Fact]
        public static void GroupJoin_WithNullComparer()
        {
            // Arrange
            var outer = Maybe.SomeOrNone("XXX");
            var inner = Maybe.SomeOrNone("YYY");
            // Act
            var q = outer.GroupJoin(inner, Ident, Ident,
                (x, y) => y.Switch(s => $"{x} = {s}",
                Funk<string>.Any),
                null);
            // Assert
            Assert.None(q);
        }

        [Fact]
        public static void GroupJoin_WithComparer()
        {
            // Arrange
            var outer = Maybe.SomeOrNone(Anagram);
            var inner = Maybe.SomeOrNone(Margana);
            string expected = $"{Anagram} est un anagramme de {Margana}";
            // Act
            var q = outer.GroupJoin(inner, Ident, Ident,
                (x, y) => y.Switch(s => $"{x} est un anagramme de {s}",
                Funk<string>.Any),
                new AnagramEqualityComparer());
            // Assert
            Assert.Some(expected, q);
        }
    }
}
