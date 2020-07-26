// See LICENSE in the project root for license information.

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
