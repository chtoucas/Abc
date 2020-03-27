// See LICENSE.txt in the project root for license information.

namespace Abc
{
    public partial class AssertEx
    {
        /// <summary>
        /// Verifies that <paramref name="maybe"/> is empty.
        /// </summary>
        public static void None<T>(Maybe<T> maybe)
            => True(maybe.IsNone, "The maybe should be empty.");

        /// <summary>
        /// Verifies that <paramref name="maybe"/> is NOT empty.
        /// </summary>
        public static void Some<T>(Maybe<T> maybe)
            // IsNone rather than IsSome because it is the public property.
            => False(maybe.IsNone, "The maybe should not be empty.");

        /// <summary>
        /// Verifies that <paramref name="maybe"/> is NOT empty and contains
        /// <paramref name="exp"/>.
        /// </summary>
        public static void Some<T>(T exp, Maybe<T> maybe)
        {
            // FIXME: Contains()
            //True(maybe.Contains(exp));

            if (maybe.IsSome)
            {
                False(maybe.IsNone, "The maybe should not be empty."); // Sanity check
                // BONSANG! When IsSome is true, Value is NOT null.
                Equal(exp, maybe.Value!);
            }
            else
            {
                True(false, "The maybe is empty.");
            }
        }

        /// <summary>
        /// Verifies that <paramref name="maybe"/> evaluates to true in a
        /// boolean context.
        /// </summary>
        public static void LogicalTrue<T>(Maybe<T> maybe)
            => True(maybe.ToBoolean());

        /// <summary>
        /// Verifies that <paramref name="maybe"/> evaluates to false in a
        /// boolean context.
        /// </summary>
        public static void LogicalFalse<T>(Maybe<T> maybe)
            => False(maybe.ToBoolean());

        /// <summary>
        /// Verifies that <paramref name="maybe"/> is <see cref="Maybe.Unknown"/>.
        /// </summary>
        public static void Unknown(Maybe<bool> maybe)
            => True(maybe.IsNone);
    }
}
