// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Xunit;

    using static global::My;

    using Assert = AssertEx;

    // Tests at least w/ a value type and a reference type.

    public static partial class MaybeTests
    {
        private static readonly Maybe<int> NONE = Maybe<int>.None;

        private static readonly Maybe<int> Ø = Maybe<int>.None;
        private static readonly Maybe<long> ØL = Maybe<long>.None;

        private static readonly Maybe<int> One = Maybe.Some(1);
        private static readonly Maybe<int> Two = Maybe.Some(2);
        private static readonly Maybe<long> TwoL = Maybe.Some(2L);

        private static readonly string MyText;
        private static readonly Maybe<string> NoText = Maybe<string>.None;
        private static readonly Maybe<string> SomeText;

        private static readonly Uri MyUri;
        private static readonly Maybe<Uri> NoUri = Maybe<Uri>.None;
        private static readonly Maybe<Uri> SomeUri;

#pragma warning disable CA1810 // Initialize reference type static fields inline
        static MaybeTests()
        {
            MyText = "text";
            SomeText = Maybe.SomeOrNone(MyText);

            MyUri = new Uri("http://www.narvalo.org");
            SomeUri = Maybe.SomeOrNone(MyUri);
        }
#pragma warning restore CA1810
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
            Assert.None(default(Maybe<Uri>));
            Assert.None(default(Maybe<object>));
        }

        [Fact]
        public static void None()
        {
            Assert.None(Maybe<Unit>.None);
            Assert.None(Maybe<int>.None);
            Assert.None(Maybe<int?>.None);
            Assert.None(Maybe<string>.None);
            Assert.None(Maybe<Uri>.None);
            Assert.None(Maybe<object>.None);

            // None is the default value.
            Assert.Equal(default, Maybe<Unit>.None);
            Assert.Equal(default, Maybe<int>.None);
            Assert.Equal(default, Maybe<int?>.None);
            Assert.Equal(default, Maybe<string>.None);
            Assert.Equal(default, Maybe<Uri>.None);
            Assert.Equal(default, Maybe<object>.None);

            // The int? version is not allowed here.
            Assert.None(Maybe.None<Unit>());
            Assert.None(Maybe.None<int>());
            Assert.None(Maybe.None<string>());
            Assert.None(Maybe.None<Uri>());
            Assert.None(Maybe.None<object>());

            // Maybe.None<T>() simply returns Maybe<T>.None.
            Assert.Equal(Maybe<Unit>.None, Maybe.None<Unit>());
            Assert.Equal(Maybe<int>.None, Maybe.None<int>());
            Assert.Equal(Maybe<string>.None, Maybe.None<string>());
            Assert.Equal(Maybe<Uri>.None, Maybe.None<Uri>());
            Assert.Equal(Maybe<object>.None, Maybe.None<object>());
        }

        [Fact]
        public static void Of()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            Assert.None(Maybe.Of((int?)null));
            Assert.Some(1, Maybe.Of((int?)1));
#pragma warning restore CS0618

            Assert.Some(1, Maybe.Of(1));

            Assert.None(Maybe.Of((string)null!));
            Assert.Some(MyText, Maybe.Of(MyText));

            Assert.None(Maybe.Of((Uri)null!));
            Assert.Some(MyUri, Maybe.Of(MyUri));
        }

        [Fact]
        public static void Some()
        {
            Assert.Some(1, Maybe.Some(1));
        }

        [Fact]
        public static void SomeOrNone()
        {
            Assert.None(Maybe.SomeOrNone((int?)null));
            Assert.Some(1, Maybe.SomeOrNone((int?)1));

            Assert.None(Maybe.SomeOrNone((string)null!));
            Assert.Some(MyText, Maybe.SomeOrNone(MyText));

            Assert.None(Maybe.SomeOrNone((Uri)null!));
            Assert.Some(MyUri, Maybe.SomeOrNone(MyUri));
        }

        [Fact]
        public static void Square()
        {
            Assert.Some(One, Maybe.Square(1));
        }

        [Fact]
        public static void SquareOrNone()
        {
            Assert.None(Maybe.SquareOrNone((int?)null));
            Assert.Some(One, Maybe.SquareOrNone((int?)1));

            Assert.None(Maybe.SquareOrNone((string)null!));
            Assert.Some(Maybe.Of(MyText), Maybe.SquareOrNone(MyText));

            Assert.None(Maybe.SquareOrNone((Uri)null!));
            Assert.Some(Maybe.Of(MyUri), Maybe.SquareOrNone(MyUri));
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
            Assert.Equal(MyText, (string)SomeText);
            Assert.Equal(MyUri, (Uri)SomeUri);

            Assert.Throws<InvalidCastException>(() => (int)Maybe<int>.None);
            Assert.Throws<InvalidCastException>(() => (string)Maybe<string>.None);
            Assert.Throws<InvalidCastException>(() => (Uri)Maybe<Uri>.None);
        }
    }

    // Core methods.
    public partial class MaybeTests
    {
        [Fact]
        public static void Bind_InvalidArg()
        {
            Assert.ThrowsArgNullEx("binder", () => Ø.Bind((Func<int, Maybe<string>>)null!));
            Assert.ThrowsArgNullEx("binder", () => NoText.Bind((Func<string, Maybe<string>>)null!));
            Assert.ThrowsArgNullEx("binder", () => NoUri.Bind((Func<Uri, Maybe<string>>)null!));

            Assert.ThrowsArgNullEx("binder", () => One.Bind((Func<int, Maybe<string>>)null!));
            Assert.ThrowsArgNullEx("binder", () => SomeText.Bind((Func<string, Maybe<string>>)null!));
            Assert.ThrowsArgNullEx("binder", () => SomeUri.Bind((Func<Uri, Maybe<string>>)null!));
        }

        [Fact]
        public static void Flatten()
        {
            Assert.Equal(Ø, Maybe.Some(Ø).Flatten());
            Assert.Equal(NoText, Maybe.Some(NoText).Flatten());
            Assert.Equal(NoUri, Maybe.Some(NoUri).Flatten());

            Assert.Equal(One, Maybe.Some(One).Flatten());
            Assert.Equal(SomeText, Maybe.Some(SomeText).Flatten());
            Assert.Equal(SomeUri, Maybe.Some(SomeUri).Flatten());

            Assert.Equal(Ø, Maybe<Maybe<int>>.None.Flatten());
            Assert.Equal(NoText, Maybe<Maybe<string>>.None.Flatten());
            Assert.Equal(NoUri, Maybe<Maybe<Uri>>.None.Flatten());
        }
    }

    // Safe escapes.
    public partial class MaybeTests
    {
        [Fact]
        public static void Switch_InvalidArg()
        {
            Assert.ThrowsArgNullEx("caseNone", () => Ø.Switch(x => x, null!));
            Assert.ThrowsArgNullEx("caseNone", () => NoText.Switch(x => x, (Func<string>)null!));
            Assert.ThrowsArgNullEx("caseNone", () => NoUri.Switch(x => x, (Func<Uri>)null!));

            Assert.ThrowsArgNullEx("caseSome", () => One.Switch(null!, () => 1));
            Assert.ThrowsArgNullEx("caseSome", () => SomeText.Switch(null!, () => 1));
            Assert.ThrowsArgNullEx("caseSome", () => SomeUri.Switch(null!, () => 1));

            Assert.ThrowsArgNullEx("caseSome", () => One.Switch(null!, 1));
            Assert.ThrowsArgNullEx("caseSome", () => SomeText.Switch(null!, 1));
            Assert.ThrowsArgNullEx("caseSome", () => SomeUri.Switch(null!, 1));
        }

        [Fact]
        public static void TryGetValue()
        {
            Assert.False(Ø.TryGetValue(out int _));
            Assert.False(NoText.TryGetValue(out string _));
            Assert.False(NoUri.TryGetValue(out Uri _));

            Assert.True(One.TryGetValue(out int one));
            Assert.Equal(1, one);

            Assert.True(SomeText.TryGetValue(out string? text));
            Assert.Equal(MyText, text);

            Assert.True(SomeUri.TryGetValue(out Uri? uri));
            Assert.Equal(MyUri, uri);
        }

        [Fact]
        public static void ValueOrDefault()
        {
            Assert.Equal(0, Ø.ValueOrDefault());
            Assert.Null(NoText.ValueOrDefault());
            Assert.Null(NoUri.ValueOrDefault());

            Assert.Equal(1, One.ValueOrDefault());
            Assert.Equal(MyText, SomeText.ValueOrDefault());
            Assert.Equal(MyUri, SomeUri.ValueOrDefault());
        }

        [Fact]
        public static void ValueOrElse_InvalidArg()
        {
            Assert.ThrowsArgNullEx("valueFactory", () => Ø.ValueOrElse(null!));
            Assert.ThrowsArgNullEx("valueFactory", () => NoText.ValueOrElse((Func<string>)null!));
            Assert.ThrowsArgNullEx("valueFactory", () => NoUri.ValueOrElse((Func<Uri>)null!));

            Assert.Equal(1, One.ValueOrElse(null!));
            Assert.Equal(MyText, SomeText.ValueOrElse((Func<string>)null!));
            Assert.Equal(MyUri, SomeUri.ValueOrElse((Func<Uri>)null!));
        }

        [Fact]
        public static void ValueOrElse()
        {
            // Arrange
            var otherUri = new Uri("https://source.dot.net/");

            // Act & Assert
            Assert.Equal(3, Ø.ValueOrElse(3));
            Assert.Equal("other", NoText.ValueOrElse("other"));
            Assert.Equal(otherUri, NoUri.ValueOrElse(otherUri));

            Assert.Equal(1, One.ValueOrElse(3));
            Assert.Equal(MyText, SomeText.ValueOrElse("other"));
            Assert.Equal(MyUri, SomeUri.ValueOrElse(otherUri));

            // With a factory.

            Assert.Equal(3, Ø.ValueOrElse(() => 3));
            Assert.Equal("other", NoText.ValueOrElse(() => "other"));
            Assert.Equal(otherUri, NoUri.ValueOrElse(() => otherUri));

            Assert.Equal(1, One.ValueOrElse(() => 3));
            Assert.Equal(MyText, SomeText.ValueOrElse(() => "other"));
            Assert.Equal(MyUri, SomeUri.ValueOrElse(() => otherUri));
        }

        [Fact]
        public static void ValueOrThrow_InvalidArg()
        {
            Assert.ThrowsArgNullEx("exception", () => Ø.ValueOrThrow(null!));
            Assert.ThrowsArgNullEx("exception", () => NoText.ValueOrThrow(null!));
            Assert.ThrowsArgNullEx("exception", () => NoUri.ValueOrThrow(null!));
        }

        [Fact]
        public static void ValueOrThrow()
        {
            Assert.Throws<InvalidOperationException>(() => Ø.ValueOrThrow());
            Assert.Throws<InvalidOperationException>(() => NoText.ValueOrThrow());
            Assert.Throws<InvalidOperationException>(() => NoUri.ValueOrThrow());

            Assert.Equal(1, One.ValueOrThrow());
            Assert.Equal(MyText, SomeText.ValueOrThrow());
            Assert.Equal(MyUri, SomeUri.ValueOrThrow());

            // With a custom exception.

            Assert.Throws<NotSupportedException>(() => Ø.ValueOrThrow(new NotSupportedException()));
            Assert.Throws<NotSupportedException>(() => NoText.ValueOrThrow(new NotSupportedException()));
            Assert.Throws<NotSupportedException>(() => NoUri.ValueOrThrow(new NotSupportedException()));

            Assert.Equal(1, One.ValueOrThrow(new NotSupportedException()));
            Assert.Equal(MyText, SomeText.ValueOrThrow(new NotSupportedException()));
            Assert.Equal(MyUri, SomeUri.ValueOrThrow(new NotSupportedException()));
        }
    }

    // Side effects.
    public partial class MaybeTests
    {
        [Fact]
        public static void Do_InvalidArg()
        {
            Assert.ThrowsArgNullEx("onNone", () => Ø.Do(_ => { }, null!));
            Assert.ThrowsArgNullEx("onNone", () => NoText.Do(_ => { }, null!));
            Assert.ThrowsArgNullEx("onNone", () => NoUri.Do(_ => { }, null!));

            Assert.ThrowsArgNullEx("onSome", () => One.Do(null!, () => { }));
            Assert.ThrowsArgNullEx("onSome", () => SomeText.Do(null!, () => { }));
            Assert.ThrowsArgNullEx("onSome", () => SomeUri.Do(null!, () => { }));
        }

        [Fact]
        public static void OnSome_InvalidArg()
        {
            Assert.ThrowsArgNullEx("action", () => One.OnSome(null!));
            Assert.ThrowsArgNullEx("action", () => SomeText.OnSome(null!));
            Assert.ThrowsArgNullEx("action", () => SomeUri.OnSome(null!));
        }
    }

    // Query Expression Pattern.
    public partial class MaybeTests
    {
        [Fact]
        public static void Select_InvalidArg()
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
        public static void Where_InvalidArg()
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
        public static void SelectMany_InvalidArg()
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

    // Async methods.
    public partial class MaybeTests
    {
        [Fact]
        public static void BindAsync_InvalidArg()
        {
            Assert.ThrowsAsyncArgNullEx("binder", () => Ø.BindAsync((Func<int, Task<Maybe<string>>>)null!));
            Assert.ThrowsAsyncArgNullEx("binder", () => NoText.BindAsync((Func<string, Task<Maybe<string>>>)null!));
            Assert.ThrowsAsyncArgNullEx("binder", () => NoUri.BindAsync((Func<Uri, Task<Maybe<string>>>)null!));

            Assert.ThrowsAsyncArgNullEx("binder", () => One.BindAsync((Func<int, Task<Maybe<string>>>)null!));
            Assert.ThrowsAsyncArgNullEx("binder", () => SomeText.BindAsync((Func<string, Task<Maybe<string>>>)null!));
            Assert.ThrowsAsyncArgNullEx("binder", () => SomeUri.BindAsync((Func<Uri, Task<Maybe<string>>>)null!));
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
        public static void ZipWith_InvalidArg()
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

    // Equality.
    public partial class MaybeTests
    {
    }
}
