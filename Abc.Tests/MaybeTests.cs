// See LICENSE.txt in the project root for license information.

namespace Abc.Tests
{
    using Xunit;

    using static global::My;

    using Assert = AssertEx;

    public static partial class MaybeTests
    {
        [Fact]
        public static void ReplaceWith_ValueType()
        {
            // Arrange
            var some = Maybe.Of(1);
            var none = Maybe<int>.None;
            int newVal = 2;
            // Act & Assert
            Assert.Some(newVal, some.ReplaceWith(newVal));
            Assert.None(none.ReplaceWith(newVal));
        }

        [Fact]
        public static void ReplaceWith_RefType()
        {
            // Arrange
            var some = Maybe.Of(new Mutable());
            var none = Maybe<Mutable>.None;
            var newVal = new Mutable("newVal");
            // Act & Assert
            Assert.Some(newVal, some.ReplaceWith(newVal));
            Assert.None(none.ReplaceWith(newVal));
        }
    }
}
