// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using Xunit;

    public static class MaybeComparerTests
    {
        private static readonly Maybe<int> Ø = Maybe<int>.None;
        private static readonly Maybe<int> One = Maybe.Some(1);
        private static readonly Maybe<int> Two = Maybe.Some(2);

        [Fact]
        public static void Compare_WithNone()
        {
            var cmp = MaybeComparer<int>.Default;
            Assert.Equal(1, cmp.Compare(One, Ø));
            Assert.Equal(-1, cmp.Compare(Ø, One));
            Assert.Equal(0, cmp.Compare(Ø, Ø));
        }

        [Fact]
        public static void Compare()
        {
            var cmp = MaybeComparer<int>.Default;
            Assert.Equal(1, cmp.Compare(Two, One));
            Assert.Equal(0, cmp.Compare(One, One));
            Assert.Equal(-1, cmp.Compare(One, Two));
        }
    }
}
