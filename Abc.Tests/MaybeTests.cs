// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Linq;

    using Xunit;

    using static global::My;

    using Assert = AssertEx;

    public static partial class MaybeTests { }

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

    // LINQ
    public partial class MaybeTests
    {
        [Fact]
        public static void Select()
        {
            // Arrange
            var some = Maybe.Of(1);
            var none = Maybe<int>.None;

            // Act & Assert
            Assert.Some(2, some.Select(__selector));
            Assert.Some(2, from x in some select __selector(x));

            Assert.None(none.Select(__selector));
            Assert.None(from x in none select __selector(x));

            static int __selector(int x) => 2 * x;
        }

        [Fact]
        public static void Where()
        {
            // Arrange
            var some = Maybe.Of(1);

            // Act & Assert
            Assert.None(some.Where(Returns<int>.False));
            Assert.None(from x in some where Returns<int>.False(x) select x);

            Assert.Some(1, some.Where(Returns<int>.True));
            Assert.Some(1, from x in some where Returns<int>.True(x) select x);
        }

        [Fact]
        public static void SelectMany1()
        {
            var none = Maybe<int>.None;
            var some = Maybe.Of(2);
            Func<int, Maybe<int>> valueSelector = _ => some;
            Func<int, int, int> resultSelector = (i, j) => i + j;

            var m1 = none.SelectMany(valueSelector, resultSelector);
            Assert.True(m1.IsNone);

            var q = from i in none
                    from j in some
                    select resultSelector(i, j);
            Assert.True(q.IsNone);
        }

        [Fact]
        public static void SelectMany2()
        {
            var none1 = Maybe<int>.None;
            var none2 = Maybe<int>.None;
            Func<int, Maybe<int>> valueSelector = _ => none2;
            Func<int, int, int> resultSelector = (i, j) => i + j;

            var m1 = none1.SelectMany(valueSelector, resultSelector);
            Assert.True(m1.IsNone);

            var q = from i in none1
                    from j in none2
                    select resultSelector(i, j);
            Assert.True(q.IsNone);
        }

        [Fact]
        public static void SelectMany3()
        {
            var some = Maybe.Of(1);
            var none = Maybe<int>.None;
            Func<int, Maybe<int>> valueSelector = _ => none;
            Func<int, int, int> resultSelector = (i, j) => i + j;

            var m1 = some.SelectMany(valueSelector, resultSelector);
            Assert.True(m1.IsNone);

            var q = from i in some
                    from j in none
                    select resultSelector(i, j);
            Assert.True(q.IsNone);
        }

        [Fact]
        public static void Join1()
        {
            var some1 = Maybe.Of(1);
            var some2 = Maybe.Of(2);

            var m1 = some1.Join(some2, i => i, i => i, (i, j) => i + j);
            Assert.True(m1.IsNone);

            var q = from i in some1
                    join j in some2 on i equals j
                    select i + j;
            Assert.True(q.IsNone);
        }
    }

    public partial class MaybeTests
    {
        [Fact]
        public static void ReplaceWith()
        {
            // Arrange
            var some = Maybe.Unit;
            var none = Maybe.None;

            // Act & Assert
            Assert.Some("value", some.ReplaceWith("value"));
            Assert.None(none.ReplaceWith("value"));

            Assert.None(some.ReplaceWith(NullString));
            Assert.None(none.ReplaceWith(NullString));

#nullable disable
            Assert.Some(2, some.ReplaceWith((int?)2));
            Assert.None(none.ReplaceWith((int?)2));

            Assert.None(some.ReplaceWith(NullNullString));
            Assert.None(none.ReplaceWith(NullNullString));
#nullable restore
        }
    }
}
