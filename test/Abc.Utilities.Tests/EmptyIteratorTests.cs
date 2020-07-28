// See LICENSE in the project root for license information.

namespace Abc.Utilities
{
    using System.Collections.Generic;

    using Xunit;

    public static class EmptyIteratorTests
    {
        [Fact]
        public static void Iterate()
        {
            // Arrange
            IEnumerator<AnyT> it = EmptyIterator<AnyT>.Instance;
            // Act & Assert
            Assert.False(it.MoveNext());
            it.Reset();
            Assert.False(it.MoveNext());
            it.Dispose();
            Assert.False(it.MoveNext());
        }
    }
}
