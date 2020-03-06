// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Runtime.ExceptionServices;

    using Anexn = System.ArgumentNullException;

    public static class Result
    {
        public static readonly Result<Unit> Ok = Some(default(Unit));

        [Pure]
        public static Result<T> None<T>() => ResultFactory<T>.None_;

        [Pure]
        public static ResultFactory<T> OfType<T>() => ResultFactory<T>.Uniq;

        [Pure]
        public static Result<T> Some<T>(T value) where T : struct
            => new Result<T>.Some(value);

        [Pure]
        public static Result<T> SomeOrNone<T>([AllowNull]T value)
            => value is null ? ResultFactory<T>.None_ : new Result<T>.Some(value);

        [Pure]
        public static Result<T> SomeOrNone<T>(T? value) where T : struct
            => value.HasValue ? new Result<T>.Some(value.Value) : ResultFactory<T>.None_;

#pragma warning disable CA1031 // Do not catch general exception types

        [Pure]
        public static Result<Unit> TryWith(Action action)
        {
            if (action is null) { throw new Anexn(nameof(action)); }

            try
            {
                action();
                return Ok;
            }
            catch (Exception ex)
            {
                var edi = ExceptionDispatchInfo.Capture(ex);
                return new Result<Unit>.Threw(edi);
            }
        }

        [Pure]
        public static Result<TResult> TryWith<TResult>(Func<TResult> func)
        {
            if (func is null) { throw new Anexn(nameof(func)); }

            try
            {
                return SomeOrNone(func());
            }
            catch (Exception ex)
            {
                var edi = ExceptionDispatchInfo.Capture(ex);
                return new Result<TResult>.Threw(edi);
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
                return Ok;
            }
            catch (Exception ex)
            {
                var edi = ExceptionDispatchInfo.Capture(ex);
                return new Result<Unit>.Threw(edi);
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
                return SomeOrNone(func());
            }
            catch (Exception ex)
            {
                var edi = ExceptionDispatchInfo.Capture(ex);
                return new Result<TResult>.Threw(edi);
            }
            finally
            {
                finallyAction();
            }
        }

#pragma warning restore CA1031
    }
}
