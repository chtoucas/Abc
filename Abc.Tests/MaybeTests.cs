// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Xunit;

    using Assert = AssertEx;

    public static partial class MaybeTests
    {
        private static readonly Maybe<int> Ø = Maybe<int>.None;
        private static readonly Maybe<long> ØL = Maybe<long>.None;

        private static readonly Maybe<int> One = Maybe.Some(1);
        private static readonly Maybe<int> Two = Maybe.Some(2);
        private static readonly Maybe<long> TwoL = Maybe.Some(2L);

        private const string MyText = "text";
        private static readonly Maybe<string> NoText = Maybe<string>.None;
        private static readonly Maybe<string> SomeText = Maybe.SomeOrNone(MyText);

        private static readonly Uri MyUri;
        private static readonly Maybe<Uri> NoUri = Maybe<Uri>.None;
        private static readonly Maybe<Uri> SomeUri;

#pragma warning disable CA1810 // Initialize reference type static fields inline
        static MaybeTests()
        {
            MyUri = new Uri("http://www.narvalo.org");
            SomeUri = Maybe.SomeOrNone(MyUri);
        }
#pragma warning restore CA1810
    }

    // Factories.
    public partial class MaybeTests
    {
        [Fact]
        public static void None_IsDefault()
        {
            Assert.Equal(default, Maybe<Unit>.None);
            Assert.Equal(default, Maybe<int>.None);
            Assert.Equal(default, Maybe<int?>.None);
            Assert.Equal(default, Maybe<string>.None);
            Assert.Equal(default, Maybe<Uri>.None);
            Assert.Equal(default, Maybe<AnyT>.None);
            Assert.Equal(default, Maybe<object>.None);
        }

        [Fact]
        public static void None_IsNone()
        {
            Assert.None(Maybe<Unit>.None);
            Assert.None(Maybe<int>.None);
            Assert.None(Maybe<int?>.None);
            Assert.None(Maybe<string>.None);
            Assert.None(Maybe<Uri>.None);
            Assert.None(Maybe<AnyT>.None);
            Assert.None(Maybe<object>.None);
        }

        [Fact]
        public static void NoneT_IsNone()
        {
            // NB: int? is not permitted here.
            Assert.None(Maybe.None<Unit>());
            Assert.None(Maybe.None<int>());
            Assert.None(Maybe.None<string>());
            Assert.None(Maybe.None<Uri>());
            Assert.None(Maybe.None<AnyT>());
            Assert.None(Maybe.None<object>());
        }

        [Fact]
        public static void NoneT_ReturnsNone()
        {
            // Maybe.None<T>() simply returns Maybe<T>.None.
            Assert.Equal(Maybe<Unit>.None, Maybe.None<Unit>());
            Assert.Equal(Maybe<int>.None, Maybe.None<int>());
            Assert.Equal(Maybe<string>.None, Maybe.None<string>());
            Assert.Equal(Maybe<Uri>.None, Maybe.None<Uri>());
            Assert.Equal(Maybe<AnyT>.None, Maybe.None<AnyT>());
            Assert.Equal(Maybe<object>.None, Maybe.None<object>());
        }

        [Fact]
        public static void Of_WithValueType()
        {
            Assert.Some(1, Maybe.Of(1));

#pragma warning disable CS0618 // Type or member is obsolete
            Assert.None(Maybe.Of((int?)null));
            Assert.Some(1, Maybe.Of((int?)1));
#pragma warning restore CS0618
        }

        [Fact]
        public static void Of_WithReferenceType()
        {
            Assert.None(Maybe.Of((string?)null));
            Assert.Some(MyText, Maybe.Of(MyText));

            Assert.None(Maybe.Of((Uri?)null));
            Assert.Some(MyUri, Maybe.Of(MyUri));

            var anyT = AnyT.Value;
            Assert.None(Maybe.Of((AnyT?)null));
            Assert.Some(anyT, Maybe.Of(anyT));
        }

        [Fact]
        public static void Some()
        {
            Assert.Some(1, Maybe.Some(1));
        }

        [Fact]
        public static void SomeOrNone_WithValueType()
        {
            Assert.None(Maybe.SomeOrNone((int?)null));
            Assert.Some(1, Maybe.SomeOrNone((int?)1));
        }

        [Fact]
        public static void SomeOrNone_WithReferenceType()
        {
            Assert.None(Maybe.SomeOrNone((string?)null));
            Assert.Some(MyText, Maybe.SomeOrNone(MyText));

            Assert.None(Maybe.SomeOrNone((Uri?)null));
            Assert.Some(MyUri, Maybe.SomeOrNone(MyUri));

            var anyT = AnyT.Value;
            Assert.None(Maybe.SomeOrNone((AnyT?)null));
            Assert.Some(anyT, Maybe.SomeOrNone(anyT));
        }

        [Fact]
        public static void Square()
        {
            Assert.Some(One, Maybe.Square(1));
        }

        [Fact]
        public static void SquareOrNone_WithValueType()
        {
            Assert.None(Maybe.SquareOrNone((int?)null));
            Assert.Some(One, Maybe.SquareOrNone((int?)1));
        }

        [Fact]
        public static void SquareOrNone_WithReferenceType()
        {
            Assert.None(Maybe.SquareOrNone((string?)null));
            Assert.Some(Maybe.SomeOrNone(MyText), Maybe.SquareOrNone(MyText));

            Assert.None(Maybe.SquareOrNone((Uri?)null));
            Assert.Some(Maybe.SomeOrNone(MyUri), Maybe.SquareOrNone(MyUri));

            var anyT = AnyT.Value;
            Assert.None(Maybe.SquareOrNone((AnyT?)null));
            Assert.Some(Maybe.SomeOrNone(anyT), Maybe.SquareOrNone(anyT));
        }
    }

    // Simple conversions.
    public partial class MaybeTests
    {
        [Fact]
        public static void ToString_None()
        {
            Assert.Equal("Maybe(None)", Maybe<int>.None.ToString());
            Assert.Equal("Maybe(None)", Maybe<string>.None.ToString());
            Assert.Equal("Maybe(None)", Maybe<Uri>.None.ToString());
            Assert.Equal("Maybe(None)", Maybe<AnyT>.None.ToString());
        }

        [Fact]
        public static void ToString_Some()
        {
            // Arrange
            string text = "My Value";
            var some = Maybe.SomeOrNone(text);
            // Act & Assert
            Assert.Contains(text, some.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public static void Explicit_FromMaybe_WithNone()
        {
            Assert.Throws<InvalidCastException>(() => (int)Ø);
            Assert.Throws<InvalidCastException>(() => (string)NoText);
            Assert.Throws<InvalidCastException>(() => (Uri)NoUri);
            Assert.Throws<InvalidCastException>(() => (AnyT)AnyT.None);
        }

        [Fact]
        public static void Explicit_FromMaybe_WithSome()
        {
            Assert.Equal(1, (int)One);
            Assert.Equal(MyText, (string)SomeText);
            Assert.Equal(MyUri, (Uri)SomeUri);

            var anyT = AnyT.New();
            Assert.Equal(anyT.Value, (AnyT)anyT.Some);
        }

        //[Fact]
        //public static void Implicit_ToMaybe()
        //{
        //    // Arrange
        //    Maybe<string> none = My.NullString; // implicit cast of a null-string

        //    // Act & Assert
        //    Assert.Some(1, 1);      // the second 1 is implicit casted to Maybe<int>
        //    Assert.None(none);

        //    Assert.True(1 == One);
        //    Assert.True(One == 1);
        //}
    }

    // Core methods.
    public partial class MaybeTests
    {
        [Fact]
        public static void Bind_None_NullBinder()
        {
            Assert.ThrowsArgNullEx("binder", () => Ø.Bind(Kunc<int, AnyResult>.Null));
        }

        [Fact]
        public static void Bind_Some_NullBinder()
        {
            Assert.ThrowsArgNullEx("binder", () => One.Bind(Kunc<int, AnyResult>.Null));
        }

        [Fact]
        public static void Bind_None()
        {
            Assert.None(Ø.Bind(_ => AnyResult.Some));
            Assert.None(NoText.Bind(_ => AnyResult.Some));
            Assert.None(NoUri.Bind(_ => AnyResult.Some));
            Assert.None(AnyT.None.Bind(_ => AnyResult.Some));
        }

        [Fact]
        public static void Bind_Some()
        {
            Assert.Some(AnyResult.Value, One.Bind(_ => AnyResult.Some));
            Assert.Some(AnyResult.Value, SomeText.Bind(_ => AnyResult.Some));
            Assert.Some(AnyResult.Value, SomeUri.Bind(_ => AnyResult.Some));
            Assert.Some(AnyResult.Value, AnyT.Some.Bind(_ => AnyResult.Some));

            Assert.None(One.Bind(_ => AnyResult.None));
            Assert.None(SomeText.Bind(_ => AnyResult.None));
            Assert.None(SomeUri.Bind(_ => AnyResult.None));
            Assert.None(AnyT.Some.Bind(_ => AnyResult.None));

            // Beyond smoke tests.
            Assert.Some(2L, One.Bind(x => Maybe.Some(2L * x)));
            Assert.Some(6L, Two.Bind(x => Maybe.Some(3L * x)));
            Assert.Some(8L, TwoL.Bind(x => Maybe.Some(4 * x)));
            Assert.Some(MyUri.AbsoluteUri, SomeUri.Bind(x => Maybe.SomeOrNone(x.AbsoluteUri)));
        }

        [Fact]
        public static void Flatten_None()
        {
            Assert.Equal(Ø, Maybe<Maybe<int>>.None.Flatten());
            Assert.Equal(Ø, Maybe<Maybe<int?>>.None.Flatten());
            Assert.Equal(NoText, Maybe<Maybe<string>>.None.Flatten());
            Assert.Equal(NoUri, Maybe<Maybe<Uri>>.None.Flatten());
            Assert.Equal(AnyT.None, Maybe<Maybe<AnyT>>.None.Flatten());
        }

        [Fact]
        public static void Flatten_SomeOfNone()
        {
            Assert.Equal(Ø, Maybe.Some(Ø).Flatten());
            Assert.Equal(NoText, Maybe.Some(NoText).Flatten());
            Assert.Equal(NoUri, Maybe.Some(NoUri).Flatten());
            Assert.Equal(AnyT.None, Maybe.Some(AnyT.None).Flatten());
        }

        [Fact]
        public static void Flatten_SomeOfSome()
        {
            Assert.Equal(One, Maybe.Some(One).Flatten());
            Assert.Equal(SomeText, Maybe.Some(SomeText).Flatten());
            Assert.Equal(SomeUri, Maybe.Some(SomeUri).Flatten());

            var some = AnyT.Some;
            Assert.Equal(some, Maybe.Some(some).Flatten());

            Maybe<int?> one = One.Select(x => (int?)x);
            Assert.Equal(One, Maybe.Some(one).Flatten());
        }
    }

    // Safe escapes.
    public partial class MaybeTests
    {
        [Fact]
        public static void Switch_None_NullCaseNone_Throws()
        {
            Assert.ThrowsArgNullEx("caseNone", () =>
                Ø.Switch(Funk<int, AnyResult>.Any, Funk<AnyResult>.Null));
        }

        [Fact]
        public static void Switch_None_NullCaseSome_DoesNotThrow()
        {
            // Act & Assert
            AnyResult v = Ø.Switch(Funk<int, AnyResult>.Null, () => AnyResult.Value);
            Assert.Same(AnyResult.Value, v); // Sanity check
        }

        [Fact]
        public static void Switch_Some_NullCaseSome_Throws()
        {
            Assert.ThrowsArgNullEx("caseSome", () =>
                One.Switch(Funk<int, AnyResult>.Null, Funk<AnyResult>.Any));
            Assert.ThrowsArgNullEx("caseSome", () =>
                One.Switch(Funk<int, AnyResult>.Null, AnyResult.Value));
        }

        [Fact]
        public static void Switch_Some_NullCaseNone_DoesNotThrow()
        {
            // Act & Assert
            AnyResult v = One.Switch(x => AnyResult.Value, Funk<AnyResult>.Null);
            Assert.Same(AnyResult.Value, v); // Sanity check
        }

        [Fact]
        public static void Switch_None()
        {
            // Arrange
            bool onSomeCalled = false;
            bool onNoneCalled = false;
            // Act
            int v = NoText.Switch(
                caseSome: x => { onSomeCalled = true; return x.Length; },
                caseNone: () => { onNoneCalled = true; return 0; });
            // Assert
            Assert.False(onSomeCalled);
            Assert.True(onNoneCalled);
            Assert.Equal(0, v);
        }

        [Fact]
        public static void Switch_None_WithConstCaseNone()
        {
            // Arrange
            bool onSomeCalled = false;
            // Act
            int v = NoText.Switch(
                caseSome: x => { onSomeCalled = true; return x.Length; },
                caseNone: 0);
            // Assert
            Assert.False(onSomeCalled);
            Assert.Equal(0, v);
        }

        [Fact]
        public static void Switch_Some()
        {
            // Arrange
            bool onSomeCalled = false;
            bool onNoneCalled = false;
            // Act
            int v = SomeText.Switch(
                caseSome: x => { onSomeCalled = true; return x.Length; },
                caseNone: () => { onNoneCalled = true; return 0; });
            // Assert
            Assert.True(onSomeCalled);
            Assert.False(onNoneCalled);
            Assert.Equal(4, v);
        }

        [Fact]
        public static void Switch_Some_WithConstCaseNone()
        {
            // Arrange
            bool onSomeCalled = false;
            // Act
            int v = SomeText.Switch(
                caseSome: x => { onSomeCalled = true; return x.Length; },
                caseNone: 0);
            // Assert
            Assert.True(onSomeCalled);
            Assert.Equal(4, v);
        }

        [Fact]
        public static void TryGetValue_None()
        {
            Assert.False(Ø.TryGetValue(out int _));
            Assert.False(NoText.TryGetValue(out string _));
            Assert.False(NoUri.TryGetValue(out Uri _));
            Assert.False(AnyT.None.TryGetValue(out AnyT _));
        }

        [Fact]
        public static void TryGetValue_Some()
        {
            Assert.True(One.TryGetValue(out int one));
            Assert.Equal(1, one);

            Assert.True(SomeText.TryGetValue(out string? text));
            Assert.Equal(MyText, text);

            Assert.True(SomeUri.TryGetValue(out Uri? uri));
            Assert.Equal(MyUri, uri);

            var anyT = AnyT.New();
            Assert.True(anyT.Some.TryGetValue(out AnyT? value));
            Assert.Equal(anyT.Value, value);
        }

        [Fact]
        public static void ValueOrDefault_None()
        {
            Assert.Equal(0, Ø.ValueOrDefault());
            Assert.Null(NoText.ValueOrDefault());
            Assert.Null(NoUri.ValueOrDefault());
            Assert.Null(AnyT.None.ValueOrDefault());
        }

        [Fact]
        public static void ValueOrDefault_Some()
        {
            Assert.Equal(1, One.ValueOrDefault());
            Assert.Equal(MyText, SomeText.ValueOrDefault());
            Assert.Equal(MyUri, SomeUri.ValueOrDefault());

            var anyT = AnyT.New();
            Assert.Equal(anyT.Value, anyT.Some.ValueOrDefault());
        }

        [Fact]
        public static void ValueOrElse_None_NullFactory_Throws()
        {
            Assert.ThrowsArgNullEx("valueFactory", () => Ø.ValueOrElse(Funk<int>.Null));
        }

        [Fact]
        public static void ValueOrElse_Some_NullFactory_DoesNotThrow()
        {
            Assert.Equal(1, One.ValueOrElse(Funk<int>.Null));
            Assert.Equal(MyText, SomeText.ValueOrElse(Funk<string>.Null));
            Assert.Equal(MyUri, SomeUri.ValueOrElse(Funk<Uri>.Null));

            var anyT = AnyT.New();
            Assert.Equal(anyT.Value, anyT.Some.ValueOrElse(Funk<AnyT>.Null));
        }

        [Fact]
        public static void ValueOrElse_None()
        {
            Assert.Equal(3, Ø.ValueOrElse(3));
            Assert.Equal("other", NoText.ValueOrElse("other"));

            var otherUri = new Uri("https://source.dot.net/");
            Assert.Equal(otherUri, NoUri.ValueOrElse(otherUri));

            var otherAnyT = AnyT.Value;
            Assert.Equal(otherAnyT, AnyT.None.ValueOrElse(otherAnyT));
        }

        [Fact]
        public static void ValueOrElse_None_WithFactory()
        {
            Assert.Equal(3, Ø.ValueOrElse(() => 3));
            Assert.Equal("other", NoText.ValueOrElse(() => "other"));

            var otherUri = new Uri("https://source.dot.net/");
            Assert.Equal(otherUri, NoUri.ValueOrElse(() => otherUri));

            var otherAnyT = AnyT.Value;
            Assert.Equal(otherAnyT, AnyT.None.ValueOrElse(() => otherAnyT));
        }

        [Fact]
        public static void ValueOrElse_Some()
        {
            Assert.Equal(1, One.ValueOrElse(3));
            Assert.Equal(MyText, SomeText.ValueOrElse("other"));

            var otherUri = new Uri("https://source.dot.net/");
            Assert.Equal(MyUri, SomeUri.ValueOrElse(otherUri));

            var anyT = AnyT.New();
            Assert.Equal(anyT.Value, anyT.Some.ValueOrElse(AnyT.Value));
        }

        [Fact]
        public static void ValueOrElse_Some_WithFactory()
        {
            Assert.Equal(1, One.ValueOrElse(() => 3));
            Assert.Equal(MyText, SomeText.ValueOrElse(() => "other"));

            var otherUri = new Uri("https://source.dot.net/");
            Assert.Equal(MyUri, SomeUri.ValueOrElse(() => otherUri));

            var anyT = AnyT.New();
            Assert.Equal(anyT.Value, anyT.Some.ValueOrElse(() => AnyT.Value));
        }

        [Fact]
        public static void ValueOrThrow_NullException()
        {
            Assert.ThrowsArgNullEx("exception", () => Ø.ValueOrThrow(null!));
        }

        [Fact]
        public static void ValueOrThrow_None()
        {
            Assert.Throws<InvalidOperationException>(() => Ø.ValueOrThrow());
            Assert.Throws<InvalidOperationException>(() => NoText.ValueOrThrow());
            Assert.Throws<InvalidOperationException>(() => NoUri.ValueOrThrow());
            Assert.Throws<InvalidOperationException>(() => AnyT.None.ValueOrThrow());
        }

        [Fact]
        public static void ValueOrThrow_None_WithCustomException()
        {
            Assert.Throws<NotSupportedException>(() => Ø.ValueOrThrow(new NotSupportedException()));
            Assert.Throws<NotSupportedException>(() => NoText.ValueOrThrow(new NotSupportedException()));
            Assert.Throws<NotSupportedException>(() => NoUri.ValueOrThrow(new NotSupportedException()));
            Assert.Throws<NotSupportedException>(() => AnyT.None.ValueOrThrow(new NotSupportedException()));
        }

        [Fact]
        public static void ValueOrThrow_Some()
        {
            Assert.Equal(1, One.ValueOrThrow());
            Assert.Equal(MyText, SomeText.ValueOrThrow());
            Assert.Equal(MyUri, SomeUri.ValueOrThrow());

            var anyT = AnyT.New();
            Assert.Equal(anyT.Value, anyT.Some.ValueOrThrow());
        }

        [Fact]
        public static void ValueOrThrow_Some_WithCustomException()
        {
            Assert.Equal(1, One.ValueOrThrow(new NotSupportedException()));
            Assert.Equal(MyText, SomeText.ValueOrThrow(new NotSupportedException()));
            Assert.Equal(MyUri, SomeUri.ValueOrThrow(new NotSupportedException()));

            var anyT = AnyT.New();
            Assert.Equal(anyT.Value, anyT.Some.ValueOrThrow(new NotSupportedException()));
        }
    }

    // Side effects.
    public partial class MaybeTests
    {
        [Fact]
        public static void Do_None_NullOnNone_Throws()
        {
            Assert.ThrowsArgNullEx("onNone", () => Ø.Do(Act<int>.Noop, Act.Null));
        }

        [Fact]
        public static void Do_None_NullOnSome_DoesNotThrow()
        {
            Ø.Do(Act<int>.Null, Act.Noop);
        }

        [Fact]
        public static void Do_Some_NullOnSome_Throws()
        {
            Assert.ThrowsArgNullEx("onSome", () => One.Do(Act<int>.Null, Act.Noop));
        }

        [Fact]
        public static void Do_Some_NullOnNone_DoesNotThrow()
        {
            One.Do(Act<int>.Noop, Act.Null);
        }

        [Fact]
        public static void Do_None()
        {
            // Arrange
            bool onSomeCalled = false;
            bool onNoneCalled = false;
            // Act
            NoText.Do(_ => { onSomeCalled = true; }, () => { onNoneCalled = true; });
            // Assert
            Assert.False(onSomeCalled);
            Assert.True(onNoneCalled);
        }

        [Fact]
        public static void Do_Some()
        {
            // Arrange
            bool onSomeCalled = false;
            bool onNoneCalled = false;
            // Act
            SomeText.Do(_ => { onSomeCalled = true; }, () => { onNoneCalled = true; });
            // Assert
            Assert.True(onSomeCalled);
            Assert.False(onNoneCalled);
        }

        [Fact]
        public static void OnSome_None_NullAction_DoesNotThrow()
        {
            Ø.OnSome(Act<int>.Null);
        }

        [Fact]
        public static void OnSome_Some_NullAction_Throws()
        {
            Assert.ThrowsArgNullEx("action", () => One.OnSome(Act<int>.Null));
        }

        [Fact]
        public static void OnSome_None()
        {
            // Arrange
            bool wasCalled = false;
            // Act
            NoText.OnSome(_ => { wasCalled = true; });
            // Assert
            Assert.False(wasCalled);
        }

        [Fact]
        public static void OnSome_Some()
        {
            // Arrange
            bool wasCalled = false;
            // Act
            SomeText.OnSome(_ => { wasCalled = true; });
            // Assert
            Assert.True(wasCalled);
        }

        [Fact]
        public static void When_None_WithFalse()
        {
            // Arrange
            bool onSomeCalled = false;
            bool onNoneCalled = false;
            // Act
            NoText.When(false, _ => { onSomeCalled = true; }, () => { onNoneCalled = true; });
            NoText.When(false, null, () => { onNoneCalled = true; });
            NoText.When(false, _ => { onSomeCalled = true; }, null);
            NoText.When(false, null, null);
            // Assert
            Assert.False(onSomeCalled);
            Assert.False(onNoneCalled);
        }

        [Fact]
        public static void When_Some_WithFalse()
        {
            // Arrange
            bool onSomeCalled = false;
            bool onNoneCalled = false;
            // Act
            SomeText.When(false, _ => { onSomeCalled = true; }, () => { onNoneCalled = true; });
            SomeText.When(false, null, () => { onNoneCalled = true; });
            SomeText.When(false, _ => { onSomeCalled = true; }, null);
            SomeText.When(false, null, null);
            // Assert
            Assert.False(onSomeCalled);
            Assert.False(onNoneCalled);
        }

        [Fact]
        public static void When_None_WithTrue()
        {
            // Arrange
            bool onSomeCalled = false;
            bool onNoneCalled = false;

            // Act & Assert
            NoText.When(true, _ => { onSomeCalled = true; }, () => { onNoneCalled = true; });
            Assert.False(onSomeCalled);
            Assert.True(onNoneCalled);

            onSomeCalled = false;
            NoText.When(true, _ => { onSomeCalled = true; }, null);
            Assert.False(onSomeCalled);

            onNoneCalled = false;
            NoText.When(true, null, () => { onNoneCalled = true; });
            Assert.True(onNoneCalled);

            // Does not throw.
            NoText.When(true, null, null);
        }

        [Fact]
        public static void When_Some_WithTrue()
        {
            // Arrange
            bool onSomeCalled = false;
            bool onNoneCalled = false;

            // Act & Assert
            SomeText.When(true, _ => { onSomeCalled = true; }, () => { onNoneCalled = true; });
            Assert.True(onSomeCalled);
            Assert.False(onNoneCalled);

            onSomeCalled = false;
            SomeText.When(true, _ => { onSomeCalled = true; }, null);
            Assert.True(onSomeCalled);

            onNoneCalled = false;
            SomeText.When(true, null, () => { onNoneCalled = true; });
            Assert.False(onNoneCalled);

            // Does not throw.
            SomeText.When(true, null, null);
        }
    }

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

    // Async methods.
    public partial class MaybeTests
    {
        #region BindAsync()

        [Fact]
        public static void BindAsync_None_NullBinder()
        {
            Assert.Async.ThrowsArgNullEx("binder", () =>
                Ø.BindAsync(Kunc<int, AnyT>.NullAsync));
        }

        [Fact]
        public static void BindAsync_Some_NullBinder()
        {
            Assert.Async.ThrowsArgNullEx("binder", () =>
                One.BindAsync(Kunc<int, AnyT>.NullAsync));
        }

        [Fact]
        public static void BindAsync_None()
        {
            Assert.Async.None(Ø.BindAsync(_ => Task.FromResult(AnyResult.Some)));
            Assert.Async.None(NoText.BindAsync(_ => Task.FromResult(AnyResult.Some)));
            Assert.Async.None(NoUri.BindAsync(_ => Task.FromResult(AnyResult.Some)));
            Assert.Async.None(AnyT.None.BindAsync(_ => Task.FromResult(AnyResult.Some)));
        }

        [Fact]
        public static void BindAsync_Some()
        {
            Assert.Async.Some(AnyResult.Value, One.BindAsync(_ => Task.FromResult(AnyResult.Some)));
            Assert.Async.Some(AnyResult.Value, SomeText.BindAsync(_ => Task.FromResult(AnyResult.Some)));
            Assert.Async.Some(AnyResult.Value, SomeUri.BindAsync(_ => Task.FromResult(AnyResult.Some)));
            Assert.Async.Some(AnyResult.Value, AnyT.Some.BindAsync(_ => Task.FromResult(AnyResult.Some)));

            Assert.Async.None(One.BindAsync(_ => Task.FromResult(AnyResult.None)));
            Assert.Async.None(SomeText.BindAsync(_ => Task.FromResult(AnyResult.None)));
            Assert.Async.None(SomeUri.BindAsync(_ => Task.FromResult(AnyResult.None)));
            Assert.Async.None(AnyT.Some.BindAsync(_ => Task.FromResult(AnyResult.None)));
        }

        #endregion

        #region SelectAsync()

        [Fact]
        public static void SelectAsync_None_NullSelector()
        {
            Assert.Async.ThrowsArgNullEx("selector", () =>
                Ø.SelectAsync(Funk<int, AnyT>.NullAsync));
        }

        [Fact]
        public static void SelectAsync_Some_NullSelector()
        {
            Assert.Async.ThrowsArgNullEx("selector", () =>
                One.SelectAsync(Funk<int, AnyT>.NullAsync));
        }

        #endregion

        #region OrElseAsync()

        [Fact]
        public static void OrElseAsync_None_NullOther()
        {
            Assert.Async.ThrowsArgNullEx("other", () => Ø.OrElseAsync(null!));
        }

        [Fact]
        public static void OrElseAsync_Some_NullOther()
        {
            Assert.Async.ThrowsArgNullEx("other", () => One.OrElseAsync(null!));
        }

        #endregion

        #region SwitchAsync()

        [Fact]
        public static void SwitchAsync_None_NullCaseNone_Throws()
        {
            Assert.Async.ThrowsArgNullEx("caseNone", () =>
                Ø.SwitchAsync(
                    caseSome: Funk<int, AnyT>.AnyAsync,
                    caseNone: null!));
        }

        [Fact]
        public static void SwitchAsync_None_NullCaseSome_DoesNotThrow()
        {
            // Act & Assert
            Task<AnyResult> v = Ø.SwitchAsync(
                caseSome: Funk<int, AnyResult>.NullAsync,
                caseNone: AnyResult.AsyncValue);
            Assert.Same(AnyResult.Value, v.Result); // Sanity check
        }

        [Fact]
        public static void SwitchAsync_Some_NullCaseSome_Throws()
        {
            Assert.Async.ThrowsArgNullEx("caseSome", () =>
                One.SwitchAsync(
                    caseSome: Funk<int, AnyT>.NullAsync,
                    caseNone: AnyT.AsyncValue));
        }

        [Fact]
        public static void SwitchAsync_Some_NullCaseNone_DoesNotThrow()
        {
            // Act & Assert
            Task<AnyResult> v = One.SwitchAsync(
                caseSome: x => AnyResult.AsyncValue,
                caseNone: null!);
            Assert.Same(AnyResult.Value, v.Result); // Sanity check
        }

        #endregion
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
            Assert.Equal(Two, Two | One);   // non-abelian
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
            Assert.Equal(One, Two & One);   // non-abelian
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
        public static void ZipWith_None_NullZipper()
        {
            Assert.ThrowsArgNullEx("zipper", () =>
                Ø.ZipWith(TwoL, Funk<int, long, AnyT>.Null));
        }

        [Fact]
        public static void ZipWith_Some_NullZipper()
        {
            Assert.ThrowsArgNullEx("zipper", () =>
                One.ZipWith(TwoL, Funk<int, long, AnyT>.Null));
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
        public static void Skip_None()
        {
            Assert.Equal(Maybe.Zero, Ø.Skip());
        }

        [Fact]
        public static void Skip_Some()
        {
            Assert.Equal(Maybe.Unit, One.Skip());
        }
    }

    // Iterable.
    public partial class MaybeTests
    {
        [Fact]
        public static void ToEnumerable()
        {
            Assert.Equal(Enumerable.Repeat(MyText, 1), SomeText.ToEnumerable());
            Assert.Empty(NoText.ToEnumerable());
        }

        [Fact]
        public static void GetEnumerator_None()
        {
            foreach (string _ in NoText)
            {
                Assert.True(false, "An empty maybe should create an empty iterator.");
            }

            // Using an explicit iterator.
            var iter = NoText.GetEnumerator();

            Assert.False(iter.MoveNext());
            iter.Reset();
            Assert.False(iter.MoveNext());
        }

        [Fact]
        public static void GetEnumerator_Some()
        {
            // Arrange
            int count = 0;

            // Act & Assert
            // First loop.
            foreach (string x in SomeText) { count++; Assert.Equal(MyText, x); }
            Assert.Equal(1, count);
            // Second loop (new iterator).
            count = 0;
            foreach (string x in SomeText) { count++; Assert.Equal(MyText, x); }
            Assert.Equal(1, count);

            // Using an explicit iterator.
            var iter = SomeText.GetEnumerator();

            Assert.True(iter.MoveNext());
            Assert.Same(MyText, iter.Current);
            Assert.False(iter.MoveNext());

            iter.Reset();

            Assert.True(iter.MoveNext());
            Assert.Same(MyText, iter.Current);
            Assert.False(iter.MoveNext());
        }

        [Fact]
        public static void Yield_None()
        {
            Assert.Empty(NoText.Yield(0));
            Assert.Empty(NoText.Yield(10));
            Assert.Empty(NoText.Yield(100));
            Assert.Empty(NoText.Yield(1000));

            Assert.Empty(NoText.Yield());
        }

        [Fact]
        public static void Yield_Some()
        {
            Assert.Equal(Enumerable.Repeat(MyText, 0), SomeText.Yield(0));
            Assert.Equal(Enumerable.Repeat(MyText, 1), SomeText.Yield(1));
            Assert.Equal(Enumerable.Repeat(MyText, 10), SomeText.Yield(10));
            Assert.Equal(Enumerable.Repeat(MyText, 100), SomeText.Yield(100));
            Assert.Equal(Enumerable.Repeat(MyText, 1000), SomeText.Yield(1000));

            Assert.Equal(Enumerable.Repeat(MyText, 0), SomeText.Yield().Take(0));
            Assert.Equal(Enumerable.Repeat(MyText, 1), SomeText.Yield().Take(1));
            Assert.Equal(Enumerable.Repeat(MyText, 10), SomeText.Yield().Take(10));
            Assert.Equal(Enumerable.Repeat(MyText, 100), SomeText.Yield().Take(100));
            Assert.Equal(Enumerable.Repeat(MyText, 1000), SomeText.Yield().Take(1000));
        }

        [Fact]
        public static void Contains_None_NullComparer()
        {
            Assert.ThrowsArgNullEx("comparer", () => Ø.Contains(1, null!));
        }

        [Fact]
        public static void Contains_Some_NullComparer()
        {
            Assert.ThrowsArgNullEx("comparer", () => One.Contains(1, null!));
        }

        [Fact]
        public static void Contains_None()
        {
            Assert.False(Ø.Contains(0));
            Assert.False(Ø.Contains(1));
            Assert.False(Ø.Contains(2));

            Assert.False(NoText.Contains("XXX"));
            Assert.False(NoText.Contains("XXX", StringComparer.Ordinal));
            Assert.False(NoText.Contains("XXX", StringComparer.OrdinalIgnoreCase));
        }

        [Fact]
        public static void Contains_Some()
        {
            Assert.False(One.Contains(0));
            Assert.True(One.Contains(1));
            Assert.False(One.Contains(2));

            Assert.True(Maybe.SomeOrNone("XXX").Contains("XXX"));
            // Default comparison does NOT ignore case.
            Assert.False(Maybe.SomeOrNone("XXX").Contains("xxx"));
            Assert.False(Maybe.SomeOrNone("XXX").Contains("yyy"));

            Assert.True(Maybe.SomeOrNone("XXX").Contains("XXX", StringComparer.Ordinal));
            Assert.False(Maybe.SomeOrNone("XXX").Contains("xxx", StringComparer.Ordinal));
            Assert.False(Maybe.SomeOrNone("XXX").Contains("yyy", StringComparer.Ordinal));

            Assert.True(Maybe.SomeOrNone("XXX").Contains("XXX", StringComparer.OrdinalIgnoreCase));
            Assert.True(Maybe.SomeOrNone("XXX").Contains("xxx", StringComparer.OrdinalIgnoreCase));
            Assert.False(Maybe.SomeOrNone("XXX").Contains("yyy", StringComparer.OrdinalIgnoreCase));
        }
    }

    // Comparison.
    //
    // Expected algebraic properties.
    //   1) Reflexivity
    //   2) Anti-symmetry
    //   3) Transitivity
    public partial class MaybeTests
    {
        [Fact]
        public static void Comparison_WithNone()
        {
            // The result is always "false".

            Assert.False(One < Ø);
            Assert.False(One > Ø);
            Assert.False(One <= Ø);
            Assert.False(One >= Ø);

            // The other way around.
            Assert.False(Ø < One);
            Assert.False(Ø > One);
            Assert.False(Ø <= One);
            Assert.False(Ø >= One);

            Maybe<int> none = Ø;
            Assert.False(Ø < none);
            Assert.False(Ø > none);
            Assert.False(Ø <= none);
            Assert.False(Ø >= none);
        }

        [Fact]
        public static void Comparison()
        {
            Assert.True(One < Two);
            Assert.False(One > Two);
            Assert.True(One <= Two);
            Assert.False(One >= Two);

            Maybe<int> one = One;
            Assert.False(One < one);
            Assert.False(One > one);
            Assert.True(One <= one);
            Assert.True(One >= one);
        }

        [Fact]
        public static void CompareTo_WithNone()
        {
            Assert.Equal(1, One.CompareTo(Ø));
            Assert.Equal(-1, Ø.CompareTo(One));
            Assert.Equal(0, Ø.CompareTo(Ø));
        }

        [Fact]
        public static void CompareTo_WithSome()
        {
            Assert.Equal(1, Two.CompareTo(One));
            Assert.Equal(0, One.CompareTo(One));
            Assert.Equal(-1, One.CompareTo(Two));
        }

        [Fact]
        public static void Comparable()
        {
            // Arrange
            IComparable none = Ø;
            IComparable one = One;
            IComparable two = Two;

            // Act & Assert
            Assert.Equal(1, none.CompareTo(null));
            Assert.Equal(1, one.CompareTo(null));

            Assert.ThrowsArgEx("obj", () => none.CompareTo(new object()));
            Assert.ThrowsArgEx("obj", () => one.CompareTo(new object()));

            // With None
            Assert.Equal(1, one.CompareTo(none));
            Assert.Equal(-1, none.CompareTo(one));
            Assert.Equal(0, none.CompareTo(none));

            // Without None
            Assert.Equal(1, two.CompareTo(one));
            Assert.Equal(0, one.CompareTo(one));
            Assert.Equal(-1, one.CompareTo(two));
        }

        [Fact]
        public static void CompareTo_Structural_None_NullComparer()
        {
            // Arrange
            IStructuralComparable none = Ø;
            // Act & Assert
            Assert.ThrowsArgNullEx("comparer", () => none.CompareTo(One, null!));
        }

        [Fact]
        public static void CompareTo_Structural_Some_NullComparer()
        {
            // Arrange
            IStructuralComparable one = One;
            // Act & Assert
            Assert.ThrowsArgNullEx("comparer", () => one.CompareTo(One, null!));
        }

        [Fact]
        public static void CompareTo_Structural()
        {
            // Arrange
            var cmp = Comparer<int>.Default;
            IStructuralComparable none = Ø;
            IStructuralComparable one = One;
            IStructuralComparable two = Two;

            // Act & Assert
            Assert.ThrowsArgEx("other", () => none.CompareTo(new object(), cmp));
            Assert.ThrowsArgEx("other", () => one.CompareTo(new object(), cmp));

            Assert.Equal(1, none.CompareTo(null, cmp));
            Assert.Equal(1, one.CompareTo(null, cmp));

            // With None
            Assert.Equal(1, one.CompareTo(Ø, cmp));
            Assert.Equal(-1, none.CompareTo(One, cmp));
            Assert.Equal(0, none.CompareTo(Ø, cmp));

            // Without None
            Assert.Equal(1, two.CompareTo(One, cmp));
            Assert.Equal(0, one.CompareTo(One, cmp));
            Assert.Equal(-1, one.CompareTo(Two, cmp));
        }
    }

    // Equality.
    //
    // Expected algebraic properties.
    //   1) Reflexivity
    //   2) Symmetry
    //   3) Transitivity
    public partial class MaybeTests
    {
        [Fact]
        public static void Equality_None_ValueType()
        {
            // Arrange
            var none = Maybe<int>.None;
            var same = Maybe<int>.None;
            var notSame = Maybe.Some(2);

            // Act & Assert
            Assert.True(none == same);
            Assert.True(same == none);
            Assert.False(none == notSame);
            Assert.False(notSame == none);

            Assert.False(none != same);
            Assert.False(same != none);
            Assert.True(none != notSame);
            Assert.True(notSame != none);

            Assert.True(none.Equals(none));
            Assert.True(none.Equals(same));
            Assert.True(same.Equals(none));
            Assert.False(none.Equals(notSame));
            Assert.False(notSame.Equals(none));

            Assert.True(none.Equals((object)same));
            Assert.False(none.Equals((object)notSame));

            Assert.False(none.Equals(null));
            Assert.False(none.Equals(new object()));
        }

        [Fact]
        public static void Equality_Some_ValueType()
        {
            // Arrange
            var some = Maybe.Some(1);
            var same = Maybe.Some(1);
            var notSame = Maybe.Some(2);

            // Act & Assert
            Assert.True(some == same);
            Assert.True(same == some);
            Assert.False(some == notSame);
            Assert.False(notSame == some);

            Assert.False(some != same);
            Assert.False(same != some);
            Assert.True(some != notSame);
            Assert.True(notSame != some);

            Assert.True(some.Equals(some));
            Assert.True(some.Equals(same));
            Assert.True(same.Equals(some));
            Assert.False(some.Equals(notSame));
            Assert.False(notSame.Equals(some));

            Assert.True(some.Equals((object)same));
            Assert.False(some.Equals((object)notSame));

            Assert.False(some.Equals(null));
            Assert.False(some.Equals(new object()));
        }

        [Fact]
        public static void Equality_None_ReferenceType()
        {
            // Arrange
            var none = Maybe<Uri>.None;
            var same = Maybe<Uri>.None;
            var notSame = Maybe.SomeOrNone(new Uri("https://source.dot.net/"));

            // Act & Assert
            Assert.True(none == same);
            Assert.True(same == none);
            Assert.False(none == notSame);
            Assert.False(notSame == none);

            Assert.False(none != same);
            Assert.False(same != none);
            Assert.True(none != notSame);
            Assert.True(notSame != none);

            Assert.True(none.Equals(none));
            Assert.True(none.Equals(same));
            Assert.True(same.Equals(none));
            Assert.False(none.Equals(notSame));
            Assert.False(notSame.Equals(none));

            Assert.True(none.Equals((object)same));
            Assert.False(none.Equals((object)notSame));

            Assert.False(none.Equals(null));
            Assert.False(none.Equals(new object()));
        }

        [Fact]
        public static void Equality_Some_ReferenceType()
        {
            // Arrange
            var some = Maybe.SomeOrNone(new Uri("http://www.narvalo.org"));
            var same = Maybe.SomeOrNone(new Uri("http://www.narvalo.org"));
            var notSame = Maybe.SomeOrNone(new Uri("https://source.dot.net/"));

            // Act & Assert
            Assert.True(some == same);
            Assert.True(same == some);
            Assert.False(some == notSame);
            Assert.False(notSame == some);

            Assert.False(some != same);
            Assert.False(same != some);
            Assert.True(some != notSame);
            Assert.True(notSame != some);

            Assert.True(some.Equals(some));
            Assert.True(some.Equals(same));
            Assert.True(same.Equals(some));
            Assert.False(some.Equals(notSame));
            Assert.False(notSame.Equals(some));

            Assert.True(some.Equals((object)same));
            Assert.False(some.Equals((object)notSame));

            Assert.False(some.Equals(null));
            Assert.False(some.Equals(new object()));
        }

        [Fact]
        public static void Equals_Structural_NullComparer()
        {
            // Arrange
            IStructuralEquatable one = One;
            // Act & Assert
            Assert.ThrowsArgNullEx("comparer", () => one.Equals(One, null!));
        }

        [Fact]
        public static void Equals_Structural_None_ValueType()
        {
            // Arrange
            IStructuralEquatable none = Maybe<int>.None;
            var same = Maybe<int>.None;
            var some = Maybe.Some(2);
            var cmp = EqualityComparer<int>.Default;

            // Act & Assert
            Assert.False(none.Equals(null, cmp));
            Assert.False(none.Equals(new object(), cmp));

            Assert.True(none.Equals(none, cmp));
            Assert.True(none.Equals(same, cmp));

            Assert.False(none.Equals(some, cmp));
        }

        [Fact]
        public static void Equals_Structural_Some_ValueType()
        {
            // Arrange
            IStructuralEquatable some = Maybe.Some(1);
            var same = Maybe.Some(1);
            var notSame = Maybe.Some(2);
            var none = Maybe<int>.None;
            var cmp = EqualityComparer<int>.Default;

            // Act & Assert
            Assert.False(some.Equals(null, cmp));
            Assert.False(some.Equals(new object(), cmp));

            Assert.True(some.Equals(some, cmp));
            Assert.True(some.Equals(same, cmp));
            Assert.False(some.Equals(notSame, cmp));

            Assert.False(some.Equals(none, cmp));
        }

        [Fact]
        public static void Equals_Structural_None_ReferenceType()
        {
            // Arrange
            IStructuralEquatable none = Maybe<Uri>.None;
            var same = Maybe<Uri>.None;
            var some = Maybe.SomeOrNone(new Uri("https://source.dot.net/"));
            var cmp = EqualityComparer<Uri>.Default;

            // Act & Assert
            Assert.False(none.Equals(null, cmp));
            Assert.False(none.Equals(new object(), cmp));

            Assert.True(none.Equals(none, cmp));
            Assert.True(none.Equals(same, cmp));

            Assert.False(none.Equals(some, cmp));
        }

        [Fact]
        public static void Equals_Structural_Some_ReferenceType()
        {
            // Arrange
            IStructuralEquatable some = Maybe.SomeOrNone(new Uri("http://www.narvalo.org"));
            var same = Maybe.SomeOrNone(new Uri("http://www.narvalo.org"));
            var notSame = Maybe.SomeOrNone(new Uri("https://source.dot.net/"));
            var none = Maybe<Uri>.None;
            var cmp = EqualityComparer<Uri>.Default;

            // Act & Assert
            Assert.False(some.Equals(null, cmp));
            Assert.False(some.Equals(new object(), cmp));

            Assert.True(some.Equals(some, cmp));
            Assert.True(some.Equals(same, cmp));
            Assert.False(some.Equals(notSame, cmp));

            Assert.False(some.Equals(none, cmp));
        }

        [Fact]
        public static void GetHashCode_None()
        {
            Assert.Equal(0, Ø.GetHashCode());
            Assert.Equal(0, ØL.GetHashCode());
            Assert.Equal(0, NoText.GetHashCode());
            Assert.Equal(0, NoUri.GetHashCode());
            Assert.Equal(0, AnyT.None.GetHashCode());
        }

        [Fact]
        public static void GetHashCode_Some()
        {
            Assert.Equal(1.GetHashCode(), One.GetHashCode());
            Assert.Equal(2.GetHashCode(), Two.GetHashCode());
            Assert.Equal(2L.GetHashCode(), TwoL.GetHashCode());
            Assert.Equal(MyText.GetHashCode(StringComparison.Ordinal), SomeText.GetHashCode());
            Assert.Equal(MyUri.GetHashCode(), SomeUri.GetHashCode());

            var anyT = AnyT.New();
            Assert.Equal(anyT.Value.GetHashCode(), anyT.Some.GetHashCode());
        }

        [Fact]
        public static void GetHashCode_Structural_NullComparer()
        {
            // Arrange
            IStructuralEquatable one = One;
            // Act & Assert
            Assert.ThrowsArgNullEx("comparer", () => one.GetHashCode(null!));
        }

        [Fact]
        public static void GetHashCode_Structural_None()
        {
            // Arrange
            var icmp = EqualityComparer<int>.Default;
            var lcmp = EqualityComparer<long>.Default;
            var scmp = EqualityComparer<string>.Default;
            var ucmp = EqualityComparer<Uri>.Default;
            var acmp = EqualityComparer<AnyT>.Default;
            // Act & Assert
            Assert.Equal(0, ((IStructuralEquatable)Ø).GetHashCode(icmp));
            Assert.Equal(0, ((IStructuralEquatable)ØL).GetHashCode(lcmp));
            Assert.Equal(0, ((IStructuralEquatable)NoText).GetHashCode(scmp));
            Assert.Equal(0, ((IStructuralEquatable)NoUri).GetHashCode(ucmp));
            Assert.Equal(0, ((IStructuralEquatable)AnyT.None).GetHashCode(acmp));
        }

        [Fact]
        public static void GetHashCode_Structural_Some()
        {
            // Arrange
            var icmp = EqualityComparer<int>.Default;
            var lcmp = EqualityComparer<long>.Default;
            var scmp = EqualityComparer<string>.Default;
            var ucmp = EqualityComparer<Uri>.Default;
            var acmp = EqualityComparer<AnyT>.Default;
            // Act & Assert
            Assert.Equal(icmp.GetHashCode(1), ((IStructuralEquatable)One).GetHashCode(icmp));
            Assert.Equal(icmp.GetHashCode(2), ((IStructuralEquatable)Two).GetHashCode(icmp));
            Assert.Equal(lcmp.GetHashCode(2L), ((IStructuralEquatable)TwoL).GetHashCode(lcmp));
            Assert.Equal(scmp.GetHashCode(MyText), ((IStructuralEquatable)SomeText).GetHashCode(scmp));
            Assert.Equal(ucmp.GetHashCode(MyUri), ((IStructuralEquatable)SomeUri).GetHashCode(ucmp));

            var anyT = AnyT.New();
            Assert.Equal(acmp.GetHashCode(anyT.Value), ((IStructuralEquatable)anyT.Some).GetHashCode(acmp));
        }
    }
}
