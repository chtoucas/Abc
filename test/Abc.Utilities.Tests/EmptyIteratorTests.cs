// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

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
