// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.ExceptionServices;

    using Anexn = System.ArgumentNullException;

    public static partial class ResultEx
    {
        public static Result<T> Flatten<T>(this Result<Result<T>> @this)
        {
            var result = Result.OfType<T>();

            return @this switch
            {
                Result<Result<T>>.Some some => some.Value,
                Result<Result<T>>.None _ => result.None,
                // Catch all Error types.
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

    public partial class ResultEx
    {
        public static void Rethrow<T, TException>(
            this Result<T>.Error<TException> @this)
            where TException : Exception
        {
            Require.NotNull(@this, nameof(@this));

            ExceptionDispatchInfo.Capture(@this.InnerErr).Throw();
        }

        public static TResult Rethrow<T, TException, TResult>(
            this Result<T>.Error<TException> @this, TResult fake)
            where TException : Exception
        {
            Require.NotNull(@this, nameof(@this));

            ExceptionDispatchInfo.Capture(@this.InnerErr).Throw();

            return fake;
        }

#pragma warning disable CA1031 // Do not catch general exception types

        [Pure]
        public static Result<Unit> TryWith(Action action)
        {
            if (action is null) { throw new Anexn(nameof(action)); }

            try
            {
                action();
                return Result.Ok;
            }
            catch (Exception ex)
            {
                return Result.OfType<Unit>().Error(ex);
            }
        }

        [Pure]
        public static Result<TResult> TryWith<TResult>(Func<TResult> func)
        {
            if (func is null) { throw new Anexn(nameof(func)); }

            try
            {
                return Result.Of(func());
            }
            catch (Exception ex)
            {
                return Result.OfType<TResult>().Error(ex);
            }
        }

        [Pure]
        public static Result<Unit> TryFinally(Action action, Action finallyAction)
        {
            if (action is null) { throw new Anexn(nameof(action)); }
            if (finallyAction is null) { throw new Anexn(nameof(finallyAction)); }

            try
            {
                action();
                return Result.Ok;
            }
            catch (Exception ex)
            {
                return Result.OfType<Unit>().Error(ex);
            }
            finally
            {
                finallyAction();
            }
        }

        [Pure]
        public static Result<TResult> TryFinally<TResult>(
            Func<TResult> func, Action finallyAction)
        {
            if (func is null) { throw new Anexn(nameof(func)); }
            if (finallyAction is null) { throw new Anexn(nameof(finallyAction)); }

            try
            {
                return Result.Of(func());
            }
            catch (Exception ex)
            {
                return Result.OfType<TResult>().Error(ex);
            }
            finally
            {
                finallyAction();
            }
        }

#pragma warning restore CA1031
    }
}
