// See LICENSE.txt in the project root for license information.

namespace Abc.Rop
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.ExceptionServices;

    using Abc.Utilities;

    using Anexn = System.ArgumentNullException;

    // When the error is in fact an exception.
    // Just for demo, in general it is a really bad idea to try to replace
    // the standard exception system.

    public static class Exceptional
    {
        public static void Rethrow(Exception ex)
        {
            Require.NotNull(ex, nameof(ex));
            ExceptionDispatchInfo.Capture(ex).Throw();
        }

        public static TResult Rethrow<TResult>(Exception ex)
        {
            Require.NotNull(ex, nameof(ex));
            ExceptionDispatchInfo.Capture(ex).Throw();
            return default;
        }

#pragma warning disable CA1031 // Do not catch general exception types

        [Pure]
        public static Result<Unit> TryWith(Action action)
        {
            if (action is null) { throw new Anexn(nameof(action)); }

            try
            {
                action();
                return ResultEx.Ok;
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
                return ResultEx.Ok;
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
