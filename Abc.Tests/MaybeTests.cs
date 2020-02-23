// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using Xunit;

    using static global::My;

    using Assert = AssertEx;

    public static partial class MaybeTests { }

    public partial class MaybeTests
    {
        [Fact]
        public static void ReplaceWith()
        {
            // Arrange
            var some = Maybe.Of(1);
            var none = Maybe<int>.None;

            // Act & Assert
            Assert.Some("value", some.ReplaceWith("value"));
            Assert.None(none.ReplaceWith("value"));

            Assert.None(some.ReplaceWith(NullString));
            Assert.None(none.ReplaceWith(NullString));

#nullable disable
            Assert.Some(2, some.ReplaceWith((int?)2));
            Assert.None(none.ReplaceWith((int?)2));

            Assert.None(some.ReplaceWith(NullNullString));
            Assert.None(none.ReplaceWith(NullNullString));
#nullable restore
        }
    }
}
