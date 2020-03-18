﻿// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System.Collections.Generic;

    using Xunit;

    using Assert = AssertEx;

    public static partial class MaybeTests
    {
        public const string NullString = null;
        public const string? NullNullString = null;

        private static readonly Maybe<int> Ø = Maybe<int>.None;
        private static readonly Maybe<int> One = Maybe.Some(1);
        private static readonly Maybe<int> Two = Maybe.Some(2);
        private static readonly Maybe<long> ØL = Maybe<long>.None;
        private static readonly Maybe<long> TwoL = Maybe.Some(2L);
    }

    public partial class MaybeTests
    {
        // Not a test for Maybe's.
        // Comparison w/ null is weird.
        //   "For the comparison operators <, >, <=, and >=, if one or both
        //   operands are null, the result is false; otherwise, the contained
        //   values of operands are compared. Do not assume that because a
        //   particular comparison (for example, <=) returns false, the opposite
        //   comparison (>) returns true."
        // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/nullable-value-types#lifted-operators
        //   "Basically the rule here is that nulls compare for equality normally,
        //   but any other comparison results in false."
        // https://ericlippert.com/2015/08/31/nullable-comparisons-are-weird/
        // Three options (extracts from Lippert's article):
        // 1) Make comparison operators produce nullable bool.
        // 2) Make comparison operators produce bool, and say that
        //    greater-than-or-equal comparisons to null have the same semantics
        //    as "or-ing" together the greater-than and equals operations.
        // 3) Make comparison operators produce a bool and apply a total
        //    ordering.
        // Choice 3 is for sorting.
        [Fact]
        public static void Comparisons()
        {
            int? one = 1;
            int? nil = null;
            // Default comparer for nullable int.
            var comparer = Comparer<int?>.Default;

            // If one of the operand is null, the comparison returns false.
            // Important consequence: we can't say that "x >= y" is equivalent
            // to "not(x < y)"...
            Assert.False(one < nil);    // false
            Assert.False(one > nil);    // false    "contradicts" Compare; see below
            Assert.False(one <= nil);   // false
            Assert.False(one >= nil);   // false

            // Equality is fine.
            Assert.False(one == nil);   // false
            Assert.True(one != nil);    // true

#pragma warning disable CS1718 // Comparison made to same variable
            Assert.False(nil < nil);    // false
            Assert.False(nil > nil);    // false
            Assert.False(nil <= nil);   // false    weird
            Assert.False(nil >= nil);   // false    weird

            Assert.True(nil == nil);    // true
            Assert.False(nil != nil);   // false
#pragma warning restore CS1718

            Assert.Equal(1, comparer.Compare(one, nil));    // "one > nil"
            Assert.Equal(-1, comparer.Compare(nil, one));   // "nil < one"
            Assert.Equal(0, comparer.Compare(nil, nil));    // "nil >= nil"
        }
    }

    // Misc methods.
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

    // Gates, bools and bits.
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

            // OrElse() is OrElseRTL() flipped.
            Assert.Equal(One, Two.OrElseRTL(One));
            Assert.Equal(One, Ø.OrElseRTL(One));
            Assert.Equal(Two, Two.OrElseRTL(Ø));
            Assert.Equal(Ø, Ø.OrElseRTL(Ø));
        }

        [Fact]
        public static void OrElseRTL()
        {
            // Some Some -> Some
            Assert.Equal(Two, One.OrElseRTL(Two));
            // Some None -> Some
            Assert.Equal(One, One.OrElseRTL(Ø));
            // None Some -> Some
            Assert.Equal(Two, Ø.OrElseRTL(Two));
            // None None -> None
            Assert.Equal(Ø, Ø.OrElseRTL(Ø));

            // OrElseRTL() is OrElse() flipped.
            Assert.Equal(Two, Two.OrElse(One));
            Assert.Equal(One, Ø.OrElse(One));
            Assert.Equal(Two, Two.OrElse(Ø));
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

            // AndThen() is PassThruWhen() flipped.
            Assert.Equal(TwoL, TwoL.PassThruWhen(One));
            Assert.Equal(ØL, ØL.PassThruWhen(One));
            Assert.Equal(ØL, TwoL.PassThruWhen(Ø));
            Assert.Equal(ØL, ØL.PassThruWhen(Ø));
        }

        [Fact]
        public static void ContinueWithIfNone()
        {
            // Some Some -> None
            Assert.Equal(ØL, One.ContinueWithIfNone(TwoL));
            // Some None -> None
            Assert.Equal(ØL, One.ContinueWithIfNone(ØL));
            // None Some -> Some
            Assert.Equal(TwoL, Ø.ContinueWithIfNone(TwoL));
            // None None -> None
            Assert.Equal(ØL, Ø.ContinueWithIfNone(ØL));

            // ContinueWithIfNone() is ZeroOutWhen() flipped.
            Assert.Equal(ØL, TwoL.ZeroOutWhen(One));
            Assert.Equal(ØL, ØL.ZeroOutWhen(One));
            Assert.Equal(TwoL, TwoL.ZeroOutWhen(Ø));
            Assert.Equal(ØL, ØL.ZeroOutWhen(Ø));
        }

        [Fact]
        public static void PassThruWhen()
        {
            // Some Some -> Some
            Assert.Equal(One, One.PassThruWhen(TwoL));
            // Some None -> None
            Assert.Equal(Ø, One.PassThruWhen(ØL));
            // None Some -> None
            Assert.Equal(Ø, Ø.PassThruWhen(TwoL));
            // None None -> None
            Assert.Equal(Ø, Ø.PassThruWhen(ØL));

            // PassThruWhen() is AndThen() flipped.
            Assert.Equal(One, TwoL.AndThen(One));
            Assert.Equal(Ø, ØL.AndThen(One));
            Assert.Equal(Ø, TwoL.AndThen(Ø));
            Assert.Equal(Ø, ØL.AndThen(Ø));
        }

        [Fact]
        public static void ZeroOutWhen()
        {
            // Some Some -> None
            Assert.Equal(Ø, One.ZeroOutWhen(TwoL));
            // Some None -> Some
            Assert.Equal(One, One.ZeroOutWhen(ØL));
            // None Some -> None
            Assert.Equal(Ø, Ø.ZeroOutWhen(TwoL));
            // None None -> None
            Assert.Equal(Ø, Ø.ZeroOutWhen(ØL));

            // ZeroOutWhen() is ContinueWithIfNone() flipped.
            Assert.Equal(Ø, TwoL.ContinueWithIfNone(One));
            Assert.Equal(One, ØL.ContinueWithIfNone(One));
            Assert.Equal(Ø, TwoL.ContinueWithIfNone(Ø));
            Assert.Equal(Ø, ØL.ContinueWithIfNone(Ø));
        }

        [Fact]
        public static void LeftAnd()
        {
            // Some Some -> Some
            Assert.Equal(One, MaybeEx.LeftAnd(One, Two));
            // Some None -> None
            Assert.Equal(Ø, MaybeEx.LeftAnd(One, Ø));
            // None Some -> Some
            Assert.Equal(Two, MaybeEx.LeftAnd(Ø, Two));
            // None None -> None
            Assert.Equal(Ø, MaybeEx.LeftAnd(Ø, Ø));

            // LeftAnd() is RightAnd() flipped.
            Assert.Equal(One, MaybeEx.RightAnd(Two, One));
            Assert.Equal(Ø, MaybeEx.RightAnd(Ø, One));
            Assert.Equal(Two, MaybeEx.RightAnd(Two, Ø));
            Assert.Equal(Ø, MaybeEx.RightAnd(Ø, Ø));
        }

        [Fact]
        public static void RightAnd()
        {
            // Some Some -> Some
            Assert.Equal(Two, MaybeEx.RightAnd(One, Two));
            // Some None -> Some
            Assert.Equal(One, MaybeEx.RightAnd(One, Ø));
            // None Some -> None
            Assert.Equal(Ø, MaybeEx.RightAnd(Ø, Two));
            // None None -> None
            Assert.Equal(Ø, MaybeEx.RightAnd(Ø, Ø));

            // RightAnd() is LeftAnd() flipped.
            Assert.Equal(Two, MaybeEx.LeftAnd(Two, One));
            Assert.Equal(One, MaybeEx.LeftAnd(Ø, One));
            Assert.Equal(Ø, MaybeEx.LeftAnd(Two, Ø));
            Assert.Equal(Ø, MaybeEx.LeftAnd(Ø, Ø));
        }

        [Fact]
        public static void Ignore()
        {
            // Some Some -> Some
            Assert.Equal(One, One.Ignore(TwoL));
            // Some None -> Some
            Assert.Equal(One, One.Ignore(ØL));
            // None Some -> None
            Assert.Equal(Ø, Ø.Ignore(TwoL));
            // None None -> None
            Assert.Equal(Ø, Ø.Ignore(ØL));

            // Ignore() is Always() flipped.
            Assert.Equal(One, TwoL.ContinueWith(One));
            Assert.Equal(One, ØL.ContinueWith(One));
            Assert.Equal(Ø, TwoL.ContinueWith(Ø));
            Assert.Equal(Ø, ØL.ContinueWith(Ø));
        }

        [Fact]
        public static void ContinueWith()
        {
            // Some Some -> Some
            Assert.Equal(TwoL, One.ContinueWith(TwoL));
            // Some None -> None
            Assert.Equal(ØL, One.ContinueWith(ØL));
            // None Some -> Some
            Assert.Equal(TwoL, Ø.ContinueWith(TwoL));
            // None None -> None
            Assert.Equal(ØL, Ø.ContinueWith(ØL));

            // Always() is Ignore() flipped.
            Assert.Equal(TwoL, TwoL.Ignore(One));
            Assert.Equal(ØL, ØL.Ignore(One));
            Assert.Equal(TwoL, TwoL.Ignore(Ø));
            Assert.Equal(ØL, ØL.Ignore(Ø));
        }
    }
}
