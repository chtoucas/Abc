// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;

    using Xunit;

    public static class MaybeComparerTests
    {
        private static readonly Maybe<int> Ø = Maybe<int>.None;
        private static readonly Maybe<int> One = Maybe.Some(1);
        private static readonly Maybe<int> Two = Maybe.Some(2);

        [Fact]
        public static void Compare()
        {
            // Arrange
            var cmp = MaybeComparer<int>.Default;

            // Act & Assert
            // With None
            Assert.Equal(1, cmp.Compare(One, Ø));
            Assert.Equal(-1, cmp.Compare(Ø, One));
            Assert.Equal(0, cmp.Compare(Ø, Ø));

            // Without None
            Assert.Equal(1, cmp.Compare(Two, One));
            Assert.Equal(0, cmp.Compare(One, One));
            Assert.Equal(-1, cmp.Compare(One, Two));
        }

        [Fact]
        public static void Compare_Objects()
        {
            // Arrange
            var cmp = MaybeComparer<int>.Default;
            object none = Maybe<int>.None;
            object one = Maybe.Some(1);
            object two = Maybe.Some(2);

            // Act & Assert
            Assert.Equal(0, cmp.Compare(null, null));
            Assert.Equal(-1, cmp.Compare(null, new object()));
            Assert.Equal(1, cmp.Compare(new object(), null));
            Assert.Equal(1, cmp.Compare(new object(), null));

            // With None
            Assert.Equal(1, cmp.Compare(one, none));
            Assert.Equal(-1, cmp.Compare(none, one));
            Assert.Equal(0, cmp.Compare(none, none));

            // Without None
            Assert.Equal(1, cmp.Compare(two, one));
            Assert.Equal(0, cmp.Compare(one, one));
            Assert.Equal(-1, cmp.Compare(one, two));

            // Not comparable
            Assert.Throws<ArgumentException>(() => cmp.Compare(new object(), none));
            Assert.Throws<ArgumentException>(() => cmp.Compare(new object(), one));
            Assert.Throws<ArgumentException>(() => cmp.Compare(none, new object()));
            Assert.Throws<ArgumentException>(() => cmp.Compare(one, new object()));
        }
    }
}
