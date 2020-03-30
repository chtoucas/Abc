﻿// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Xunit;

    using Assert = AssertEx;

    // Not actually a test of Maybe.
    public partial class MaybeTests
    {
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