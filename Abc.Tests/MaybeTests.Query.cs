// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using Xunit;

    using Assert = AssertEx;

    // Query Expression Pattern.
    public partial class MaybeTests
    {
        #region Select()

        [Fact]
        public static void Select_None_NullSelector()
        {
            Assert.ThrowsArgNullEx("selector", () => Ø.Select(Funk<int, AnyT>.Null));
        }

        [Fact]
        public static void Select_Some_NullSelector()
        {
            Assert.ThrowsArgNullEx("selector", () => One.Select(Funk<int, AnyT>.Null));
        }

        [Fact]
        public static void Select_None()
        {
            Assert.None(Ø.Select(x => x));
            Assert.None(from x in Ø select x);
        }

        [Fact]
        public static void Select_Some()
        {
            Assert.Some(2L, One.Select(x => 2L * x));
            Assert.Some(2L, from x in One select 2L * x);

            Assert.Some(6L, Two.Select(x => 3L * x));
            Assert.Some(6L, from x in Two select 3L * x);

            Assert.Some(MyUri.AbsoluteUri, SomeUri.Select(x => x.AbsoluteUri));
            Assert.Some(MyUri.AbsoluteUri, from x in SomeUri select x.AbsoluteUri);
        }

        #endregion

        #region Where()

        [Fact]
        public static void Where_None_NullPredicate()
        {
            Assert.ThrowsArgNullEx("predicate", () => Ø.Where(null!));
        }

        [Fact]
        public static void Where_Some_NullPredicate()
        {
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

        #endregion

        #region SelectMany()

        [Fact]
        public static void SelectMany_None_NullSelector()
        {
            Assert.ThrowsArgNullEx("selector", () =>
                Ø.SelectMany(Kunc<int, AnyT1>.Null, Funk<int, AnyT1, AnyT2>.Any));
        }

        [Fact]
        public static void SelectMany_Some_NullSelector()
        {
            Assert.ThrowsArgNullEx("selector", () =>
                One.SelectMany(Kunc<int, AnyT1>.Null, Funk<int, AnyT1, AnyT2>.Any));
        }

        [Fact]
        public static void SelectMany_None_NullResultSelector()
        {
            Assert.ThrowsArgNullEx("resultSelector", () =>
                Ø.SelectMany(Kunc<int, AnyT1>.Any, Funk<int, AnyT1, AnyT2>.Null));
        }

        [Fact]
        public static void SelectMany_Some_NullResultSelector()
        {
            Assert.ThrowsArgNullEx("resultSelector", () =>
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
            Assert.ThrowsArgNullEx("outerKeySelector", () =>
                Ø.Join(TwoL, Funk<int, AnyT1>.Null, Funk<long, AnyT1>.Null, Funk<int, long, AnyT2>.Any));
            // With a comparer.
            Assert.ThrowsArgNullEx("outerKeySelector", () =>
                Ø.Join(TwoL, Funk<int, AnyT1>.Null, Funk<long, AnyT1>.Null, Funk<int, long, AnyT2>.Any, null));
        }

        [Fact]
        public static void Join_Some_NullOuterKeySelector()
        {
            Assert.ThrowsArgNullEx("outerKeySelector", () =>
                One.Join(TwoL, Funk<int, AnyT1>.Null, Funk<long, AnyT1>.Any, Funk<int, long, AnyT2>.Any));
            // With a comparer.
            Assert.ThrowsArgNullEx("outerKeySelector", () =>
                One.Join(TwoL, Funk<int, AnyT1>.Null, Funk<long, AnyT1>.Any, Funk<int, long, AnyT2>.Any, null));
        }

        [Fact]
        public static void Join_None_NullInnerKeySelector()
        {
            Assert.ThrowsArgNullEx("innerKeySelector", () =>
                Ø.Join(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Null, Funk<int, long, AnyT2>.Any));
            // With a comparer.
            Assert.ThrowsArgNullEx("innerKeySelector", () =>
                Ø.Join(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Null, Funk<int, long, AnyT2>.Any, null));
        }

        [Fact]
        public static void Join_Some_NullInnerKeySelector()
        {
            Assert.ThrowsArgNullEx("innerKeySelector", () =>
                One.Join(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Null, Funk<int, long, AnyT2>.Any));
            // With a comparer.
            Assert.ThrowsArgNullEx("innerKeySelector", () =>
                One.Join(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Null, Funk<int, long, AnyT2>.Any, null));
        }

        [Fact]
        public static void Join_None_NullResultSelector()
        {
            Assert.ThrowsArgNullEx("resultSelector", () =>
                Ø.Join(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Any, Funk<int, long, AnyT2>.Null));
            // With a comparer.
            Assert.ThrowsArgNullEx("resultSelector", () =>
                Ø.Join(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Any, Funk<int, long, AnyT2>.Null, null));
        }

        [Fact]
        public static void Join_Some_NullResultSelector()
        {
            Assert.ThrowsArgNullEx("resultSelector", () =>
                One.Join(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Any, Funk<int, long, AnyT2>.Null));
            // With a comparer.
            Assert.ThrowsArgNullEx("resultSelector", () =>
                One.Join(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Any, Funk<int, long, AnyT2>.Null, null));
        }

        [Fact]
        public static void Join()
        {
            Assert.None(One.Join(Two, i => i, i => i, (i, j) => i + j));
            Assert.None(from i in One join j in Two on i equals j select i + j);
        }

        #endregion

        #region GroupJoin()

        [Fact]
        public static void GroupJoin_None_NullOuterKeySelector()
        {
            Assert.ThrowsArgNullEx("outerKeySelector", () =>
                Ø.GroupJoin(TwoL, Funk<int, AnyT1>.Null, Funk<long, AnyT1>.Null, Funk<int, Maybe<long>, AnyT2>.Any));
            // With a comparer.
            Assert.ThrowsArgNullEx("outerKeySelector", () =>
                Ø.GroupJoin(TwoL, Funk<int, AnyT1>.Null, Funk<long, AnyT1>.Null, Funk<int, Maybe<long>, AnyT2>.Any, null));
        }

        [Fact]
        public static void GroupJoin_Some_NullOuterKeySelector()
        {
            Assert.ThrowsArgNullEx("outerKeySelector", () =>
                One.GroupJoin(TwoL, Funk<int, AnyT1>.Null, Funk<long, AnyT1>.Any, Funk<int, Maybe<long>, AnyT2>.Any));
            // With a comparer.
            Assert.ThrowsArgNullEx("outerKeySelector", () =>
                One.GroupJoin(TwoL, Funk<int, AnyT1>.Null, Funk<long, AnyT1>.Any, Funk<int, Maybe<long>, AnyT2>.Any, null));
        }

        [Fact]
        public static void GroupJoin_None_NullInnerKeySelector()
        {
            Assert.ThrowsArgNullEx("innerKeySelector", () =>
                Ø.GroupJoin(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Null, Funk<int, Maybe<long>, AnyT2>.Any));
            // With a comparer.
            Assert.ThrowsArgNullEx("innerKeySelector", () =>
                Ø.GroupJoin(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Null, Funk<int, Maybe<long>, AnyT2>.Any, null));
        }

        [Fact]
        public static void GroupJoin_Some_NullInnerKeySelector()
        {
            Assert.ThrowsArgNullEx("innerKeySelector", () =>
                One.GroupJoin(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Null, Funk<int, Maybe<long>, AnyT2>.Any));
            // With a comparer.
            Assert.ThrowsArgNullEx("innerKeySelector", () =>
                One.GroupJoin(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Null, Funk<int, Maybe<long>, AnyT2>.Any, null));
        }

        [Fact]
        public static void GroupJoin_None_NullResultSelector()
        {
            Assert.ThrowsArgNullEx("resultSelector", () =>
                Ø.GroupJoin(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Any, Funk<int, Maybe<long>, AnyT2>.Null));
            // With a comparer.
            Assert.ThrowsArgNullEx("resultSelector", () => Ø.GroupJoin(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Any, Funk<int, Maybe<long>, AnyT2>.Null, null));
        }

        [Fact]
        public static void GroupJoin_Some_NullResultSelector()
        {
            Assert.ThrowsArgNullEx("resultSelector", () =>
                One.GroupJoin(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Any, Funk<int, Maybe<long>, AnyT2>.Null));
            // With a comparer.
            Assert.ThrowsArgNullEx("resultSelector", () =>
                One.GroupJoin(TwoL, Funk<int, AnyT1>.Any, Funk<long, AnyT1>.Any, Funk<int, Maybe<long>, AnyT2>.Null, null));
        }

        #endregion
    }
}
