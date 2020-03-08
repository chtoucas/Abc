// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;

    using Abc.Utilities;

    using Anexn = System.ArgumentNullException;

    public static partial class ResultEx
    {
        public static readonly Result<Unit> Ok = Result.Some(default(Unit));

        public static Maybe<T> ToMaybe<T>(this Result<T> @this)
        {
            Require.NotNull(@this, nameof(@this));

            // Converts an error to an empty maybe.
            return @this.IsSome ? Maybe.Of(@this.Value) : Maybe<T>.None;

            //return @this switch
            //{
            //    Result<T>.Some some => Maybe.Of(some.Value),
            //    Result<T>.None _ => Maybe<T>.None,
            //    // Catch all Error's.
            //    Result<T>.Error _ => Maybe<T>.None,
            //    null => throw new Anexn(nameof(@this)),
            //    _ => throw new InvalidOperationException()
            //};
        }

        public static Result<T> Flatten<T>(this Result<Result<T>> @this)
        {
            var result = Result.OfType<T>();

            return @this switch
            {
                Result<Result<T>>.Some some => some.Value,
                Result<Result<T>>.None _ => result.None,
                // Catch all Error's.
                Result<Result<T>>.Error _ => result.None,
                null => throw new Anexn(nameof(@this)),
                _ => throw new InvalidOperationException()
            };
        }

        public static Result<T> Flatten<T, TErr>(this Result<Result<T>> @this)
        {
            var result = Result.OfType<T>();

            return @this switch
            {
                Result<Result<T>>.Some some => some.Value,
                Result<Result<T>>.None _ => result.None,
                Result<Result<T>>.Error<TErr> err => result.Error(err),
                null => throw new Anexn(nameof(@this)),
                _ => throw new InvalidOperationException()
            };
        }
    }
}
