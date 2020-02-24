// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using Xunit;

    public sealed class AssertEx : Assert
    {
        public static void None<T>(Maybe<T> maybe)
            => True(maybe.IsNone);

        public static void Some<T>(Maybe<T> maybe)
            => False(maybe.IsNone);

        // TODO: à améliorer.
        public static void Some<T>(T exp, Maybe<T> maybe)
        {
#if MONADS_PURE
            False(maybe.IsNone);
            True(maybe.Contains(exp));
#else
            True(maybe.IsSome);
            Equal(exp, maybe.Value);
#endif
        }
    }
}
