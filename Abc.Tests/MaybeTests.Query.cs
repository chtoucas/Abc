// See LICENSE in the project root for license information.

namespace Abc
{
    using Xunit;

    using Assert = AssertEx;

    // Query Expression Pattern.
    public partial class MaybeTests
    {
        #region Select()

        [Fact]
        public static void Select_None_NullSelector() =>
            Assert.ThrowsAnexn("selector", () => Ø.Select(Funk<int, AnyT>.Null));

        [Fact]
        public static void Select_Some_NullSelector() =>
            Assert.ThrowsAnexn("selector", () => One.Select(Funk<int, AnyT>.Null));

        [Fact]
        public static void Select_None()
        {
            Assert.None(Ø.Select(Ident));
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

        #endregion

        #region Where()

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

        #endregion

        #region SelectMany()

        [Fact]
        public static void SelectMany_None_NullSelector()
        {
            Assert.ThrowsAnexn("selector", () =>
                Ø.SelectMany(Kunc<int, AnyT1>.Null, Funk<int, AnyT1, AnyT2>.Any));
        }

        [Fact]
        public static void SelectMany_Some_NullSelector()
        {
            Assert.ThrowsAnexn("selector", () =>
                One.SelectMany(Kunc<int, AnyT1>.Null, Funk<int, AnyT1, AnyT2>.Any));
        }

        [Fact]
        public static void SelectMany_None_NullResultSelector()
        {
            Assert.ThrowsAnexn("resultSelector", () =>
                Ø.SelectMany(Kunc<int, AnyT1>.Any, Funk<int, AnyT1, AnyT2>.Null));
        }

        [Fact]
        public static void SelectMany_Some_NullResultSelector()
        {
            Assert.ThrowsAnexn("resultSelector", () =>
                One.SelectMany(Kunc<int, AnyT1>.Any, Funk<int, AnyT1, AnyT2>.Null));
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

        #endregion

        #region Join()

        [Fact]
        public static void Join_None_NullOuterKeySelector()
        {
            Assert.ThrowsAnexn("outerKeySelector", () =>
                Ø.Join(TwoL, Funk<int, AnyT1>.Null, Funk<long, AnyT1>.Null, Funk<int, long, AnyT2>.Any));
            // With a comparer.
            Assert.ThrowsAnexn("outerKeySelector", () =>
                Ø.Join(TwoL, Funk<int, AnyT1>.Null, Funk<long, AnyT1>.Null, Funk<int, long, AnyT2>.Any, null));
        }

        [Fact]
        public static void Join_Some_NullOuterKeySelector()
        {
            Assert.ThrowsAnexn("outerKeySelector", () =>
                One.Join(TwoL, Funk<int, AnyT1>.Null, Funk<long, AnyT1>.Any, Funk<int, long, AnyT2>.Any));
            // With a comparer.
            Assert.ThrowsAnexn("outerKeySelector", () =>
                One.Join(TwoL, Funk<int, AnyT1>.Null, Funk<long, AnyT1>.Any, Funk<int, long, AnyT2>.Any, null));
        }

        [Fact]
        public static void Join_None_NullInnerKeySelector()
        {
            Assert.ThrowsAnexn("innerKeySelector", () =>
                Ø.Join(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Null, Funk<int, long, AnyT2>.Any));
            // With a comparer.
            Assert.ThrowsAnexn("innerKeySelector", () =>
                Ø.Join(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Null, Funk<int, long, AnyT2>.Any, null));
        }

        [Fact]
        public static void Join_Some_NullInnerKeySelector()
        {
            Assert.ThrowsAnexn("innerKeySelector", () =>
                One.Join(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Null, Funk<int, long, AnyT2>.Any));
            // With a comparer.
            Assert.ThrowsAnexn("innerKeySelector", () =>
                One.Join(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Null, Funk<int, long, AnyT2>.Any, null));
        }

        [Fact]
        public static void Join_None_NullResultSelector()
        {
            Assert.ThrowsAnexn("resultSelector", () =>
                Ø.Join(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Any, Funk<int, long, AnyT2>.Null));
            // With a comparer.
            Assert.ThrowsAnexn("resultSelector", () =>
                Ø.Join(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Any, Funk<int, long, AnyT2>.Null, null));
        }

        [Fact]
        public static void Join_Some_NullResultSelector()
        {
            Assert.ThrowsAnexn("resultSelector", () =>
                One.Join(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Any, Funk<int, long, AnyT2>.Null));
            // With a comparer.
            Assert.ThrowsAnexn("resultSelector", () =>
                One.Join(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Any, Funk<int, long, AnyT2>.Null, null));
        }

        [Fact]
        public static void Join()
        {
            Assert.None(One.Join(Two, Ident, Ident, (i, j) => i + j));
            Assert.None(from i in One join j in Two on i equals j select i + j);
        }

        [Fact]
        public static void Join_WithComparer()
        {
            // Arrange
            var outer = Maybe.SomeOrNone("chicane");
            var inner = Maybe.SomeOrNone("caniche");
            string expected = "chicane est un anagramme de caniche";
            // Act
            var q = outer.Join(inner, Ident, Ident,
                (x, y) => $"{x} est un anagramme de {y}",
                new AnagramEqualityComparer());
            // Assert
            Assert.Some(expected, q);
        }

        #endregion

        #region GroupJoin()

        [Fact]
        public static void GroupJoin_None_NullOuterKeySelector()
        {
            Assert.ThrowsAnexn("outerKeySelector", () =>
                Ø.GroupJoin(TwoL, Funk<int, AnyT1>.Null, Funk<long, AnyT1>.Null, Funk<int, Maybe<long>, AnyT2>.Any));
            // With a comparer.
            Assert.ThrowsAnexn("outerKeySelector", () =>
                Ø.GroupJoin(TwoL, Funk<int, AnyT1>.Null, Funk<long, AnyT1>.Null, Funk<int, Maybe<long>, AnyT2>.Any, null));
        }

        [Fact]
        public static void GroupJoin_Some_NullOuterKeySelector()
        {
            Assert.ThrowsAnexn("outerKeySelector", () =>
                One.GroupJoin(TwoL, Funk<int, AnyT1>.Null, Funk<long, AnyT1>.Any, Funk<int, Maybe<long>, AnyT2>.Any));
            // With a comparer.
            Assert.ThrowsAnexn("outerKeySelector", () =>
                One.GroupJoin(TwoL, Funk<int, AnyT1>.Null, Funk<long, AnyT1>.Any, Funk<int, Maybe<long>, AnyT2>.Any, null));
        }

        [Fact]
        public static void GroupJoin_None_NullInnerKeySelector()
        {
            Assert.ThrowsAnexn("innerKeySelector", () =>
                Ø.GroupJoin(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Null, Funk<int, Maybe<long>, AnyT2>.Any));
            // With a comparer.
            Assert.ThrowsAnexn("innerKeySelector", () =>
                Ø.GroupJoin(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Null, Funk<int, Maybe<long>, AnyT2>.Any, null));
        }

        [Fact]
        public static void GroupJoin_Some_NullInnerKeySelector()
        {
            Assert.ThrowsAnexn("innerKeySelector", () =>
                One.GroupJoin(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Null, Funk<int, Maybe<long>, AnyT2>.Any));
            // With a comparer.
            Assert.ThrowsAnexn("innerKeySelector", () =>
                One.GroupJoin(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Null, Funk<int, Maybe<long>, AnyT2>.Any, null));
        }

        [Fact]
        public static void GroupJoin_None_NullResultSelector()
        {
            Assert.ThrowsAnexn("resultSelector", () =>
                Ø.GroupJoin(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Any, Funk<int, Maybe<long>, AnyT2>.Null));
            // With a comparer.
            Assert.ThrowsAnexn("resultSelector", () =>
                Ø.GroupJoin(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Any, Funk<int, Maybe<long>, AnyT2>.Null, null));
        }

        [Fact]
        public static void GroupJoin_Some_NullResultSelector()
        {
            Assert.ThrowsAnexn("resultSelector", () =>
                One.GroupJoin(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Any, Funk<int, Maybe<long>, AnyT2>.Null));
            // With a comparer.
            Assert.ThrowsAnexn("resultSelector", () =>
                One.GroupJoin(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Any, Funk<int, Maybe<long>, AnyT2>.Null, null));
        }

        [Fact]
        public static void GroupJoin_WithComparer()
        {
            // Arrange
            var outer = Maybe.SomeOrNone("chicane");
            var inner = Maybe.SomeOrNone("caniche");
            string expected = "chicane est un anagramme de caniche";
            // Act
            var q = outer.GroupJoin(inner, Ident, Ident,
                (x, y) => y.Switch(s => $"{x} est un anagramme de {s}", Funk<string>.Any),
                new AnagramEqualityComparer());
            // Assert
            Assert.Some(expected, q);
        }

        #endregion
    }
}
