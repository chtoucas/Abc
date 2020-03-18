// See LICENSE.txt in the project root for license information.

namespace Abc
{
    public partial class AssertEx
    {
        public static void None<T>(Maybe<T> maybe)
            => True(maybe.IsNone);

        public static void Some<T>(Maybe<T> maybe)
            => False(maybe.IsNone);

        // TODO: à améliorer.
        public static void Some<T>(T exp, Maybe<T> maybe)
        {
            //False(maybe.IsNone);
            //True(maybe.Contains(exp));

            True(maybe.IsSome);
            Equal(exp, maybe.Value);
        }

        // Converts to the boolean "true".
        public static void IsTrue<T>(Maybe<T> maybe)
            => True(maybe.ToBoolean());

        // Converts to the boolean "false".
        public static void IsFalse<T>(Maybe<T> maybe)
            => False(maybe.ToBoolean());
    }
}
