// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

namespace Abc.Utilities
{
    using Xunit;

    public static class InternalForTestingAttributeTests
    {
        [Fact]
        public static void Constructor()
        {
            // Act
            var attr = new InternalForTestingAttribute();
            // Assert
            Assert.Equal(AccessibilityLevel.Private, attr.GenuineAccessibility);
        }

        [Fact]
        public static void GenuineAccessibility()
        {
            // Act
            var attr = new InternalForTestingAttribute
            {
                GenuineAccessibility = AccessibilityLevel.Protected
            };
            // Assert
            Assert.Equal(AccessibilityLevel.Protected, attr.GenuineAccessibility);
        }
    }
}
