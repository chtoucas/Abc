// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;

    using Xunit;

    public static class MaybeComparerTests
    {
        [Fact]
        public static void Compare_ValueType()
        {
            // Arrange
            var cmp = MaybeComparer<int>.Default;
            var none = Maybe<int>.None;
            var one = Maybe.Some(1);
            var two = Maybe.Some(2);

            // Act & Assert
            // With None
            Assert.Equal(1, cmp.Compare(one, none));
            Assert.Equal(-1, cmp.Compare(none, one));
            Assert.Equal(0, cmp.Compare(none, none));

            // Without None
            Assert.Equal(1, cmp.Compare(two, one));
            Assert.Equal(0, cmp.Compare(one, one));
            Assert.Equal(-1, cmp.Compare(one, two));
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

        [Fact]
        public static void Equals_ValueType()
        {
            // Arrange
            var cmp = MaybeComparer<int>.Default;
            var none = Maybe<int>.None;
            var some = Maybe.Some(1);
            var same = Maybe.Some(1);
            var notSame = Maybe.Some(2);

            // Act & Assert
            // With None
            Assert.False(cmp.Equals(some, none));
            Assert.False(cmp.Equals(none, some));
            Assert.True(cmp.Equals(none, none));

            // Without None
            Assert.False(cmp.Equals(notSame, some));
            Assert.True(cmp.Equals(same, some));
            Assert.True(cmp.Equals(some, some));
            Assert.True(cmp.Equals(some, same));
            Assert.False(cmp.Equals(some, notSame));
        }

        [Fact]
        public static void Equals_ReferenceType()
        {
            // Arrange
            var cmp = MaybeComparer<Uri>.Default;
            var none = Maybe<Uri>.None;
            var some = Maybe.SomeOrNone(new Uri("http://www.narvalo.org"));
            var same = Maybe.SomeOrNone(new Uri("http://www.narvalo.org"));
            var notSame = Maybe.SomeOrNone(new Uri("https://source.dot.net/"));

            // Act & Assert
            // With None
            Assert.False(cmp.Equals(some, none));
            Assert.False(cmp.Equals(none, some));
            Assert.True(cmp.Equals(none, none));

            // Without None
            Assert.False(cmp.Equals(notSame, some));
            Assert.True(cmp.Equals(same, some));
            Assert.True(cmp.Equals(some, some));
            Assert.True(cmp.Equals(some, same));
            Assert.False(cmp.Equals(some, notSame));
        }
    }
}
