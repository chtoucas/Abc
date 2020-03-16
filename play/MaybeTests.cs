// See LICENSE.txt in the project root for license information.

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
        }

        [Fact]
        public static void PassThruWhenNone()
        {
            // Some Some -> None
            Assert.None(One.PassThruWhenNone(TwoL));
            // Some None -> Some
            Assert.Some(1, One.PassThruWhenNone(ØL));
            // None Some -> None
            Assert.None(Ø.PassThruWhenNone(TwoL));
            // None None -> None
            Assert.None(Ø.PassThruWhenNone(ØL));
        }

        [Fact]
        public static void Otherwise()
        {
            // Some Some -> Some
            Assert.Some(2, One.Otherwise(Two));
            // Some None -> Some
            Assert.Some(1, One.Otherwise(Ø));
            // None Some -> Some
            Assert.Some(2, Ø.Otherwise(Two));
            // None None -> None
            Assert.None(Ø.Otherwise(Ø));
        }
    }
}
