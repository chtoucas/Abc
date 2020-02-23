// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using Xunit;

    public sealed class AssertEx : Assert
    {
        public static void None<T>(Maybe<T> maybe)
            => False(maybe.IsSome);

        public static void Some<T>(Maybe<T> maybe)
            => True(maybe.IsSome);

        public static void Some<T>(T exp, Maybe<T> maybe)
        {
            True(maybe.IsSome);
            Equal(exp, maybe.Value);
        }
    }
}
