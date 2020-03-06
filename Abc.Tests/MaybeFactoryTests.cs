// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using Xunit;

    using Assert = AssertEx;

    public static class MaybeFactoryTests
    {
        [Fact]
        public static void Some()
        {
            Assert.Some(MaybeFactory.Some(1));
        }
    }
}
