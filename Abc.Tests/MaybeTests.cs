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
        private static readonly Maybe<int> NONE = Maybe<int>.None;

        private static readonly Maybe<int> Ø = Maybe<int>.None;
        private static readonly Maybe<long> ØL = Maybe<long>.None;
        private static readonly Maybe<string> ØT = Maybe<string>.None;

        private static readonly Maybe<int> One = Maybe.Some(1);
        private static readonly Maybe<int> Two = Maybe.Some(2);
        private static readonly Maybe<long> TwoL = Maybe.Some(2L);

        private static readonly Maybe<string> ValueT = Maybe.SomeOrNone("value");
    }

    // Construction & factories.
    public partial class MaybeTests
    {
        [Fact]
        public static void Default()
        {
            // The default value is empty.
            Assert.None(default(Maybe<Unit>));
            Assert.None(default(Maybe<int>));
            Assert.None(default(Maybe<string>));
            Assert.None(default(Maybe<object>));
        }

        [Fact]
        public static void None()
        {
            Assert.None(Maybe<Unit>.None);
            Assert.None(Maybe<int>.None);
            Assert.None(Maybe<int?>.None);
            Assert.None(Maybe<string>.None);
            Assert.None(Maybe<object>.None);

            // None is the default value.
            Assert.Equal(default, Maybe<Unit>.None);
            Assert.Equal(default, Maybe<int>.None);
            Assert.Equal(default, Maybe<int?>.None);
            Assert.Equal(default, Maybe<string>.None);
            Assert.Equal(default, Maybe<object>.None);

            Assert.None(Maybe.None<Unit>());
            Assert.None(Maybe.None<int>());
            Assert.None(Maybe.None<string>());
            Assert.None(Maybe.None<object>());

            // Maybe.None<T>() simply returns Maybe<T>.None.
            Assert.Equal(Maybe<Unit>.None, Maybe.None<Unit>());
            Assert.Equal(Maybe<int>.None, Maybe.None<int>());
            Assert.Equal(Maybe<string>.None, Maybe.None<string>());
            Assert.Equal(Maybe<object>.None, Maybe.None<object>());
        }

        [Fact]
        public static void Of_Reference()
        {
            Assert.None(Maybe.Of((string?)null));
            Assert.None(Maybe.Of((string)null!));

            Assert.Some("value", Maybe.Of("value"));
            Assert.Some("value", Maybe.Of((string?)"value"));

            Assert.None(Maybe.Of((object?)null));
            Assert.None(Maybe.Of((object)null!));

            var @ref = new object();
            Assert.Some(@ref, Maybe.Of(@ref));
            Assert.Some(@ref, Maybe.Of((object?)@ref));
        }

        [Fact]
        public static void Of_Value()
        {
            Assert.Some(1, Maybe.Of(1));

#pragma warning disable CS0618 // Type or member is obsolete
            Assert.None(Maybe.Of((int?)null));
            Assert.Some(1, Maybe.Of((int?)1));
#pragma warning restore CS0618
        }

        [Fact]
        public static void Some()
        {
            Assert.Some(1, Maybe.Some(1));
        }

        [Fact]
        public static void SomeOrNone()
        {
            // Value type.
            Assert.None(Maybe.SomeOrNone((int?)null));
            Assert.Some(1, Maybe.SomeOrNone((int?)1));

            // Reference type.
            var @ref = new object();
            Assert.None(Maybe.SomeOrNone((object)null!));
            Assert.Some(@ref, Maybe.SomeOrNone(@ref));
        }

        [Fact]
        public static void Square()
        {
            Assert.Some(One, Maybe.Square(1));
        }

        [Fact]
        public static void SquareOrNone()
        {
            // Value type.
            Assert.None(Maybe.SquareOrNone((int?)null));
            Assert.Some(One, Maybe.SquareOrNone((int?)1));

            // Reference type.
            var @ref = new object();
            Assert.None(Maybe.SquareOrNone((object)null!));
            Assert.Some(Maybe.Of(@ref), Maybe.SquareOrNone(@ref));
        }

        [Fact]
        public static void ToString_CurrentCulture()
        {
            var none = Maybe<int>.None;
            Assert.Contains("None", none.ToString(), StringComparison.OrdinalIgnoreCase);

            string value = "My Value";
            var some = Maybe.Of(value);
            Assert.Contains(value, some.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void ImplicitToMaybe()
        {
            // Arrange
            Maybe<string> none = NullString; // implicit cast of a null-string

            // Act & Assert
            Assert.Some(1, 1);      // the second 1 is implicit casted to Maybe<int>
            Assert.None(none);

            Assert.True(1 == One);
            Assert.True(One == 1);
        }

        [Fact]
        public static void ExplicitFromMaybe()
        {
            Assert.Equal(1, (int)One);
            Assert.Equal(2L, (long)TwoL);

            Assert.Throws<InvalidCastException>(() => (string)Maybe<string>.None);
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
        public static void Flatten()
        {
            Assert.Equal(Ø, Maybe.Some(Ø).Flatten());
            Assert.Equal(One, Maybe.Some(One).Flatten());

            Assert.Equal(Ø, Maybe<Maybe<int>>.None.Flatten());
        }
    }

    // Safe escapes.
    public partial class MaybeTests
    {
        [Fact]
        public static void TryGetValue()
        {
            Assert.False(Ø.TryGetValue(out int _));
            Assert.False(ØL.TryGetValue(out long _));
            Assert.False(ØT.TryGetValue(out string _));

            Assert.True(One.TryGetValue(out int one));
            Assert.Equal(1, one);

            Assert.True(Two.TryGetValue(out int two));
            Assert.Equal(2, two);

            Assert.True(TwoL.TryGetValue(out long twoL));
            Assert.Equal(2L, twoL);

            Assert.True(ValueT.TryGetValue(out string? value));
            Assert.Equal("value", value);

            var @ref = new object();
            var some = Maybe.SomeOrNone(@ref);
            Assert.True(some.TryGetValue(out object? obj));
            Assert.Equal(@ref, obj);
        }

        [Fact]
        public static void ValueOrDefault()
        {
            Assert.Equal(0, Ø.ValueOrDefault());
            Assert.Equal(0L, ØL.ValueOrDefault());
            Assert.Null(ØT.ValueOrDefault());

            Assert.Equal(1, One.ValueOrDefault());
            Assert.Equal(2, Two.ValueOrDefault());
            Assert.Equal(2L, TwoL.ValueOrDefault());
            Assert.Equal("value", ValueT.ValueOrDefault());

            var @ref = new object();
            var some = Maybe.SomeOrNone(@ref);
            Assert.Equal(@ref, some.ValueOrDefault());
        }

        [Fact]
        public static void ValueOrElse()
        {
            Assert.Equal(3, Ø.ValueOrElse(3));
            Assert.Equal(3L, ØL.ValueOrElse(3L));
            Assert.Equal("other", ØT.ValueOrElse("other"));

            Assert.Equal(1, One.ValueOrElse(3));
            Assert.Equal(2, Two.ValueOrElse(3));
            Assert.Equal(2L, TwoL.ValueOrElse(3));
            Assert.Equal("value", ValueT.ValueOrElse("other"));

            var @ref = new object();
            var some = Maybe.SomeOrNone(@ref);
            var other = new object();
            Assert.NotSame(@ref, other); // sanity check
            Assert.Equal(@ref, some.ValueOrElse(other));
        }

        [Fact]
        public static void ValueOrThrow()
        {
            Assert.Throws<InvalidOperationException>(() => Ø.ValueOrThrow());
            Assert.Throws<InvalidOperationException>(() => ØL.ValueOrThrow());
            Assert.Throws<InvalidOperationException>(() => ØT.ValueOrThrow());

            Assert.Equal(1, One.ValueOrThrow());
            Assert.Equal(2, Two.ValueOrThrow());
            Assert.Equal(2L, TwoL.ValueOrThrow());
            Assert.Equal("value", ValueT.ValueOrThrow());

            var @ref = new object();
            var some = Maybe.SomeOrNone(@ref);
            Assert.Equal(@ref, some.ValueOrThrow());

            Assert.Throws<NotSupportedException>(() => Ø.ValueOrThrow(new NotSupportedException()));
            Assert.Throws<NotSupportedException>(() => ØL.ValueOrThrow(new NotSupportedException()));
            Assert.Throws<NotSupportedException>(() => ØT.ValueOrThrow(new NotSupportedException()));

            Assert.Equal(1, One.ValueOrThrow(new NotSupportedException()));
            Assert.Equal(2, Two.ValueOrThrow(new NotSupportedException()));
            Assert.Equal(2L, TwoL.ValueOrThrow(new NotSupportedException()));
            Assert.Equal("value", ValueT.ValueOrThrow(new NotSupportedException()));

            Assert.Equal(@ref, some.ValueOrThrow(new NotSupportedException()));
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
        public static void SelectMany_InvalidSelector()
        {
            Assert.ThrowsArgNullEx("selector",
                () => Ø.SelectMany((Func<int, Maybe<int>>)null!, (i, j) => i + j));
            Assert.ThrowsArgNullEx("selector",
                () => One.SelectMany((Func<int, Maybe<int>>)null!, (i, j) => i + j));

            Assert.ThrowsArgNullEx("resultSelector",
                () => Ø.SelectMany(_ => Ø, (Func<int, int, int>)null!));
            Assert.ThrowsArgNullEx("resultSelector",
                () => One.SelectMany(_ => One, (Func<int, int, int>)null!));
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

    // "Bitwise" logical operations.
    public partial class MaybeTests
    {
        [Fact]
        public static void OrElse()
        {
            // Some Some -> Some
            Assert.Equal(One, One.OrElse(Two));
            // Some None -> Some
            Assert.Equal(One, One.OrElse(Ø));
            // None Some -> Some
            Assert.Equal(Two, Ø.OrElse(Two));
            // None None -> None
            Assert.Equal(Ø, Ø.OrElse(Ø));
        }

        [Fact]
        public static void AndThen()
        {
            // Some Some -> Some
            Assert.Equal(TwoL, One.AndThen(TwoL));
            // Some None -> None
            Assert.Equal(ØL, One.AndThen(ØL));
            // None Some -> None
            Assert.Equal(ØL, Ø.AndThen(TwoL));
            // None None -> None
            Assert.Equal(ØL, Ø.AndThen(ØL));
        }

        [Fact]
        public static void XorElse()
        {
            // Some Some -> None
            Assert.Equal(Ø, One.XorElse(Two));
            // Some None -> Some
            Assert.Equal(One, One.XorElse(Ø));
            // None Some -> Some
            Assert.Equal(Two, Ø.XorElse(Two));
            // None None -> None
            Assert.Equal(Ø, Ø.XorElse(Ø));

            // XorElse() flips to itself.
            Assert.Equal(Ø, Two.XorElse(One));
            Assert.Equal(One, Ø.XorElse(One));
            Assert.Equal(Two, Two.XorElse(Ø));
            Assert.Equal(Ø, Ø.XorElse(Ø));
        }

        [Fact]
        public static void BitwiseOr()
        {
            // Some Some -> Some
            Assert.Equal(One, One | Two);
            Assert.Equal(Two, Two | One);   // non-abelian!
            // Some None -> Some
            Assert.Equal(One, One | Ø);
            // None Some -> Some
            Assert.Equal(Two, Ø | Two);
            // None None -> None
            Assert.Equal(Ø, Ø | Ø);

            Assert.LogicalTrue(One | Two);
            Assert.LogicalTrue(One | Ø);
            Assert.LogicalTrue(Ø | Two);
            Assert.LogicalFalse(Ø | Ø);
        }

        [Fact]
        public static void BitwiseAnd()
        {
            // Some Some -> Some
            Assert.Equal(Two, One & Two);
            Assert.Equal(One, Two & One);   // non-abelian!
            // Some None -> None
            Assert.Equal(Ø, One & Ø);
            // None Some -> None
            Assert.Equal(Ø, Ø & Two);
            // None None -> None
            Assert.Equal(Ø, Ø & Ø);

            Assert.LogicalTrue(One & Two);
            Assert.LogicalFalse(One & Ø);
            Assert.LogicalFalse(Ø & Two);
            Assert.LogicalFalse(Ø & Ø);
        }

        [Fact]
        public static void ExclusiveOr()
        {
            // Some Some -> None
            Assert.Equal(Ø, One ^ Two);
            Assert.Equal(Ø, Two ^ One);     // abelian
            // Some None -> Some
            Assert.Equal(One, One ^ Ø);
            // None Some -> Some
            Assert.Equal(Two, Ø ^ Two);
            // None None -> None
            Assert.Equal(Ø, Ø ^ Ø);

            Assert.LogicalFalse(One ^ Two);
            Assert.LogicalTrue(One ^ Ø);
            Assert.LogicalTrue(Ø ^ Two);
            Assert.LogicalFalse(Ø ^ Ø);
        }
    }

    // Misc methods.
    public partial class MaybeTests
    {
        [Fact]
        public static void ZipWith_InvalidZipper()
        {
            Assert.ThrowsArgNullEx("zipper",
                () => Ø.ZipWith(TwoL, (Func<int, long, long>)null!));
            Assert.ThrowsArgNullEx("zipper",
                () => One.ZipWith(TwoL, (Func<int, long, long>)null!));
        }

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
            // Arrange
            var some = Maybe.Of("value");
            int count = 0;

            // Act & Assert

            // First loop.
            foreach (string x in some) { count++; Assert.Equal("value", x); }
            Assert.Equal(1, count);
            // Second loop (new iterator).
            count = 0;
            foreach (string x in some) { count++; Assert.Equal("value", x); }
            Assert.Equal(1, count);

            // Using an explicit iterator.
            var iter = some.GetEnumerator();

            // First loop.
            count = 0;
            while (iter.MoveNext()) { count++; }
            Assert.Equal(1, count);
            // Second loop: no call to Reset().
            count = 0;
            while (iter.MoveNext()) { count++; }
            Assert.Equal(0, count);
            // Third loop: call to Reset().
            count = 0;
            iter.Reset();
            while (iter.MoveNext()) { count++; }
            Assert.Equal(1, count);
        }

        [Fact]
        public static void GetEnumerator_None()
        {
            // Arrange
            var none = Maybe<string>.None;
            int count = 0;

            // Act & Assert

            // First loop.
            foreach (string x in none) { count++; }
            Assert.Equal(0, count);
            // Second loop (new iterator).
            foreach (string x in none) { count++; }
            Assert.Equal(0, count);

            // Using an explicit iterator.
            var iter = none.GetEnumerator();

            // First loop.
            while (iter.MoveNext()) { count++; }
            Assert.Equal(0, count);
            // Second loop: no call to Reset().
            while (iter.MoveNext()) { count++; }
            Assert.Equal(0, count);
            // Third loop: call to Reset().
            iter.Reset();
            while (iter.MoveNext()) { count++; }
            Assert.Equal(0, count);
        }

        [Fact]
        public static void Yield()
        {
            // Arrange
            var some = Maybe.Of("value");
            var none = Maybe<string>.None;

            // Act & Assert
            Assert.Equal(Enumerable.Repeat("value", 0), some.Yield(0));
            Assert.Equal(Enumerable.Repeat("value", 1), some.Yield(1));
            Assert.Equal(Enumerable.Repeat("value", 10), some.Yield(10));
            Assert.Equal(Enumerable.Repeat("value", 100), some.Yield(100));
            Assert.Equal(Enumerable.Repeat("value", 1000), some.Yield(1000));
            Assert.Empty(none.Yield(0));
            Assert.Empty(none.Yield(10));
            Assert.Empty(none.Yield(100));
            Assert.Empty(none.Yield(1000));

            Assert.Equal(Enumerable.Repeat("value", 0), some.Yield().Take(0));
            Assert.Equal(Enumerable.Repeat("value", 1), some.Yield().Take(1));
            Assert.Equal(Enumerable.Repeat("value", 10), some.Yield().Take(10));
            Assert.Equal(Enumerable.Repeat("value", 100), some.Yield().Take(100));
            Assert.Equal(Enumerable.Repeat("value", 1000), some.Yield().Take(1000));
            Assert.Empty(none.Yield());
        }

        [Fact]
        public static void Contains()
        {
            Assert.False(One.Contains(0));
            Assert.True(One.Contains(1));
            Assert.False(One.Contains(2));

            Assert.False(Ø.Contains(0));
            Assert.False(Ø.Contains(1));
            Assert.False(Ø.Contains(2));

            Assert.True(Maybe.Of("XXX").Contains("XXX"));
            // Default comparison does NOT ignore case.
            Assert.False(Maybe.Of("XXX").Contains("xxx"));
        }
    }

    // Comparison.
    public partial class MaybeTests
    {
        [Fact]
        public static void Comparison_WithNone_ReturnFalse()
        {
            Assert.False(One < Ø);
            Assert.False(One > Ø);
            Assert.False(One <= Ø);
            Assert.False(One >= Ø);

            // The other way around.
            Assert.False(Ø < One);
            Assert.False(Ø > One);
            Assert.False(Ø <= One);
            Assert.False(Ø >= One);

#pragma warning disable CS1718 // Comparison made to same variable
            Assert.False(Ø < Ø);
            Assert.False(Ø > Ø);
            Assert.False(Ø <= Ø);
            Assert.False(Ø >= Ø);
#pragma warning restore CS1718
        }

        [Fact]
        public static void CompareTo_WithNone()
        {
            Assert.Equal(1, One.CompareTo(Ø));
            Assert.Equal(-1, Ø.CompareTo(One));
            Assert.Equal(0, Ø.CompareTo(Ø));
        }
    }
}
