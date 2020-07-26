// See LICENSE in the project root for license information.

namespace Abc.Utilities
{
    using Xunit;

    public static class ThunksTests
    {
        [Fact]
        public static void Noop()
        {
            Assert.Same(Thunks.Noop, Thunks.Noop);
            Assert.NotNull(Thunks.Noop);
            Thunks.Noop.Invoke();
        }

        [Fact]
        public static void Ident()
        {
            // Arrange
            var value = new AnyT();
            // Act & Assert
            Assert.Same(Thunks<AnyT>.Ident, Thunks<AnyT>.Ident);
            Assert.NotNull(Thunks<AnyT>.Ident);
            Assert.Same(value, Thunks<AnyT>.Ident.Invoke(value));
        }

        [Fact]
        public static void NoopT()
        {
            Assert.Same(Thunks<AnyT>.Noop, Thunks<AnyT>.Noop);
            Assert.NotNull(Thunks<AnyT>.Noop);
            Thunks<AnyT>.Noop.Invoke(new AnyT());
        }
    }
}
