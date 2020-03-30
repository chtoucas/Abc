// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System.Threading.Tasks;

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
            False(maybe.IsNone, "The maybe should not be empty.");

            if (maybe.IsSome)
            {
                // BONSANG! When IsSome is true, Value is NOT null.
                Equal(exp, maybe.Value!);
            }

            // We also test Contains().
            True(maybe.Contains(exp));
        }

        /// <summary>
        /// Verifies that <paramref name="maybe"/> evaluates to true in a
        /// boolean context.
        /// </summary>
        public static void LogicalTrue<T>(Maybe<T> maybe)
            => True(maybe.ToBoolean(), "The maybe should evaluate to true.");

        /// <summary>
        /// Verifies that <paramref name="maybe"/> evaluates to false in a
        /// boolean context.
        /// </summary>
        public static void LogicalFalse<T>(Maybe<T> maybe)
            => False(maybe.ToBoolean(), "The maybe should evaluate to false.");

        /// <summary>
        /// Verifies that <paramref name="maybe"/> is <see cref="Maybe.Unknown"/>.
        /// </summary>
        public static void Unknown(Maybe<bool> maybe)
            => True(maybe.IsNone, "The maybe should be empty.");
    }

    // Async.
    public partial class AssertEx
    {
        public partial class Async
        {
            // TODO: make them async... not blocking.

            /// <summary>
            /// Verifies that the result of <paramref name="task"/> is empty.
            /// </summary>
            public static void None<T>(Task<Maybe<T>> task)
            {
                if (IsNull(nameof(task), task)) { return; }

                var maybe = task.Result;

                True(maybe.IsNone, "The maybe should be empty.");
            }

            /// <summary>
            /// Verifies that the result of <paramref name="task"/> is NOT empty.
            /// </summary>
            public static void Some<T>(Task<Maybe<T>> task)
            {
                if (IsNull(nameof(task), task)) { return; }

                var maybe = task.Result;

                False(maybe.IsNone, "The maybe should not be empty.");
            }

            /// <summary>
            /// Verifies that the result of <paramref name="task"/> is NOT empty
            /// and contains <paramref name="exp"/>.
            /// </summary>
            public static void Some<T>(T exp, Task<Maybe<T>> task)
            {
                if (IsNull(nameof(task), task)) { return; }

                Maybe<T> maybe = task.Result;

                False(maybe.IsNone, "The maybe should not be empty.");

                if (maybe.IsSome)
                {
                    // BONSANG! When IsSome is true, Value is NOT null.
                    Equal(exp, maybe.Value!);
                }

                // We also test Contains().
                True(maybe.Contains(exp));
            }
        }
    }
}
