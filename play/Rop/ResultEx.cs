// See LICENSE.txt in the project root for license information.

namespace Abc.Rop
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
            // This method converts an error to an empty maybe.
            return @this.IsError ? Maybe<T>.None: Maybe.Of(@this.Value);
        }

        public static Ok<T> Squash<T>(this Ok<T?> @this)
            where T : struct
        {
            Require.NotNull(@this, nameof(@this));
            return Result.OfType<T>().Ok(@this.Value.Value);
        }

        public static NullResult<T> Squash<T>(this NullResult<T?> @this)
            where T : struct
        {
            return Result.OfType<T>().EmptyError;
        }

        public static Error<T, TErr> Squash<T, TErr>(this Error<T?, TErr> @this)
            where T : struct
        {
            Require.NotNull(@this, nameof(@this));
            return Result.OfType<T>().Error(@this.InnerError);
        }

        public static Result<T> Squash<T, TErr>(this Result<T?> @this)
            where T : struct
        {
            var result = Result.OfType<T>();

            return @this switch
            {
                Ok<T?> some => result.Ok(some.Value.Value),
                NullResult<T?> _ => result.EmptyError,
                Error<T?, TErr> err => result.Error(err.InnerError),
                null => throw new Anexn(nameof(@this)),
                _ => throw new InvalidOperationException()
            };
        }

        // bad bad bad...

        //public static Result<T> Flatten<T>(this Result<Result<T>> @this)
        //{
        //    var result = Result.OfType<T>();

        //    return @this switch
        //    {
        //        Ok<Ok<T>> some => some.Value,
        //        Result<Result<T>>.None _ => result.None,
        //        // Catch all Error's.
        //        Error<Error<T>> _ => result.None,
        //        null => throw new Anexn(nameof(@this)),
        //        _ => throw new InvalidOperationException()
        //    };
        //}

        //public static Result<T> Flatten<T, TErr>(this Result<Result<T>> @this)
        //{
        //    var result = Result.OfType<T>();

        //    return @this switch
        //    {
        //        Ok<Ok<T>> some => some.Value,
        //        Result<Result<T>>.None _ => result.None,
        //        Error<Error<T, TErr>, TErr> err => result.Error(err),
        //        //Result<Result<T>>.Error _ => throw new NotImplementedException(),
        //        null => throw new Anexn(nameof(@this)),
        //        _ => throw new InvalidOperationException()
        //    };
        //}
    }
}
