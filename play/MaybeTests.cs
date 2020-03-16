﻿// See LICENSE.txt in the project root for license information.

namespace Abc
{
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
            // OrElse() is OrElseRTL() flipped.
            Assert.Some(1, Two.OrElseRTL(One));
            Assert.Some(1, Ø.OrElseRTL(One));
            Assert.Some(2, Two.OrElseRTL(Ø));
            Assert.None(Ø.OrElseRTL(Ø));
        }

        [Fact]
        public static void OrElseRTL()
        {
            // Some Some -> Some
            Assert.Some(2, One.OrElseRTL(Two));
            // Some None -> Some
            Assert.Some(1, One.OrElseRTL(Ø));
            // None Some -> Some
            Assert.Some(2, Ø.OrElseRTL(Two));
            // None None -> None
            Assert.None(Ø.OrElseRTL(Ø));

            // OrElseRTL() is OrElse() flipped.
            Assert.Some(2, Two.OrElse(One));
            Assert.Some(1, Ø.OrElse(One));
            Assert.Some(2, Two.OrElse(Ø));
            Assert.None(Ø.OrElse(Ø));
        }

        [Fact]
        public static void ContinueWith()
        {
            // ContinueWith() is ContinueWhen() flipped.
            Assert.Some(2L, TwoL.ContinueWhen(One));
            Assert.None(ØL.ContinueWhen(One));
            Assert.None(TwoL.ContinueWhen(Ø));
            Assert.None(ØL.ContinueWhen(Ø));
        }

        [Fact]
        public static void ContinueWhen()
        {
            // Some Some -> Some
            Assert.Some(1, One.ContinueWhen(TwoL));
            // Some None -> None
            Assert.None(One.ContinueWhen(ØL));
            // None Some -> None
            Assert.None(Ø.ContinueWhen(TwoL));
            // None None -> None
            Assert.None(Ø.ContinueWhen(ØL));

            // ContinueWhen() is ContinueWith() flipped.
            Assert.Some(1, TwoL.ContinueWith(One));
            Assert.None(ØL.ContinueWith(One));
            Assert.None(TwoL.ContinueWith(Ø));
            Assert.None(ØL.ContinueWith(Ø));
        }

        [Fact]
        public static void ContinueWithIfNone()
        {
            // Some Some -> None
            Assert.None(One.ContinueWithIfNone(TwoL));
            // Some None -> None
            Assert.None(One.ContinueWithIfNone(ØL));
            // None Some -> Some
            Assert.Some(2L, Ø.ContinueWithIfNone(TwoL));
            // None None -> None
            Assert.None(Ø.ContinueWithIfNone(ØL));

            // ContinueWithIfNone() is ContinueUnless() flipped.
            Assert.None(TwoL.ContinueUnless(One));
            Assert.None(ØL.ContinueUnless(One));
            Assert.Some(2L, TwoL.ContinueUnless(Ø));
            Assert.None(ØL.ContinueUnless(Ø));
        }

        [Fact]
        public static void ContinueUnless()
        {
            // Some Some -> None
            Assert.None(One.ContinueUnless(TwoL));
            // Some None -> Some
            Assert.Some(1, One.ContinueUnless(ØL));
            // None Some -> None
            Assert.None(Ø.ContinueUnless(TwoL));
            // None None -> None
            Assert.None(Ø.ContinueUnless(ØL));

            // ContinueUnless() is ContinueWithIfNone() flipped.
            Assert.None(TwoL.ContinueWithIfNone(One));
            Assert.Some(1, ØL.ContinueWithIfNone(One));
            Assert.None(TwoL.ContinueWithIfNone(Ø));
            Assert.None(ØL.ContinueWithIfNone(Ø));
        }

        [Fact]
        public static void RightProject()
        {
            // Some Some -> Some
            Assert.Some(1, One.RightProject(Two));
            // Some None -> None
            Assert.None(One.RightProject(Ø));
            // None Some -> Some
            Assert.Some(2, Ø.RightProject(Two));
            // None None -> None
            Assert.None(Ø.RightProject(Ø));

            // RightProject() is LeftProject() flipped.
            Assert.Some(1, Two.LeftProject(One));
            Assert.None(Ø.LeftProject(One));
            Assert.Some(2, Two.LeftProject(Ø));
            Assert.None(Ø.LeftProject(Ø));
        }

        [Fact]
        public static void LeftProject()
        {
            // Some Some -> Some
            Assert.Some(2, One.LeftProject(Two));
            // Some None -> Some
            Assert.Some(1, One.LeftProject(Ø));
            // None Some -> None
            Assert.None(Ø.LeftProject(Two));
            // None None -> None
            Assert.None(Ø.LeftProject(Ø));

            // LeftProject() is RightProject() flipped.
            Assert.Some(2, Two.RightProject(One));
            Assert.Some(1, Ø.RightProject(One));
            Assert.None(Two.RightProject(Ø));
            Assert.None(Ø.RightProject(Ø));
        }

        [Fact]
        public static void Ignore()
        {
            // Some Some -> Some
            Assert.Some(1, One.Ignore(TwoL));
            // Some None -> Some
            Assert.Some(1, One.Ignore(ØL));
            // None Some -> None
            Assert.None(Ø.Ignore(TwoL));
            // None None -> None
            Assert.None(Ø.Ignore(ØL));

            // Ignore() is Always() flipped.
            Assert.Some(1, TwoL.Always(One));
            Assert.Some(1, ØL.Always(One));
            Assert.None(TwoL.Always(Ø));
            Assert.None(ØL.Always(Ø));
        }

        [Fact]
        public static void Always()
        {
            // Some Some -> Some
            Assert.Some(2L, One.Always(TwoL));
            // Some None -> None
            Assert.None(One.Always(ØL));
            // None Some -> Some
            Assert.Some(2L, Ø.Always(TwoL));
            // None None -> None
            Assert.None(Ø.Always(ØL));

            // Always() is Ignore() flipped.
            Assert.Some(2L, TwoL.Ignore(One));
            Assert.None(ØL.Ignore(One));
            Assert.Some(2L, TwoL.Ignore(Ø));
            Assert.None(ØL.Ignore(Ø));
        }
    }
}