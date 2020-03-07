// See LICENSE.txt in the project root for license information.

#pragma warning disable CA1000 // Do not declare static members on generic types

namespace Abc.Samples
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.ExceptionServices;

    using Abc.Utilities;

    using Anexn = System.ArgumentNullException;

    // Simulating Either<T, TError> with a ValueTuple.
    public static class Result<T, TError> where T : struct
    {
        public static (Result<T> success, TError error) Ok(T value)
            => (Result.Some(value), default);

        public static (Result<T> success, TError error) Error(TError error)
            => (Result.None<T>(), error);
    }

    public static partial class ResultSamples { }

    // With classes instead of structs, we can use pattern matching.
    // Of course, if we had sum types like in F# it would be simpler and clearer.
    public partial class ResultSamples
    {
        public static string SomeOrNone(bool ok)
        {
            var r = ok ? Result.Some(1) : Result.None<int>();

            return r.IsSome ? $"{r.Value}" : "No value";
        }

        public static string SomeOrError(bool ok)
        {
            var result = Result.OfType<int>();
            var r = ok ? result.Some(1) : result.Error("Boum!!!");

            return r switch
            {
                Result<int>.Some some => $"{some.Value}",
                Result<int>.Error<string> err => err.InnerError,
                _ => throw new InvalidOperationException()
            };
        }

        public static string SomeOrThrew(bool ok)
        {
            var result = Result.OfType<int>();
            var r = ok ? result.Some(1) : result.Error("Boum!!!");

            return r switch
            {
                Result<int>.Some some => $"{some.Value}",
                Result<int>.Error<string> err => err.InnerError,
                Result<int>.Error<NotSupportedException> exn => exn.Rethrow(default(string)!),
                _ => throw new InvalidOperationException()
            };
        }

        public static string OkOrError(bool ok)
        {
            var r = ok ? Result<int, string>.Ok(1) : Result<int, string>.Error("Boum!!!");

            return r switch
            {
                (Result<int>.Some some, _) => $"{some.Value}",
                (Result<int>.None _, string err) => err,
                _ => throw new InvalidOperationException()
            };
        }
    }

    // When the error is in fact an exception.
    // Just for demo, in most cases, it is not a good idea to replace the
    // standard exception system.
    public partial class ResultSamples
    {
        public static void Rethrow<T, TException>(
            this Result<T>.Error<TException> @this)
            where TException : Exception
        {
            Require.NotNull(@this, nameof(@this));

            ExceptionDispatchInfo.Capture(@this.InnerError).Throw();
        }

        public static TResult Rethrow<T, TException, TResult>(
            this Result<T>.Error<TException> @this, TResult fake)
            where TException : Exception
        {
            Require.NotNull(@this, nameof(@this));

            ExceptionDispatchInfo.Capture(@this.InnerError).Throw();

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
