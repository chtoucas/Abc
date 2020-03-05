// See LICENSE.txt in the project root for license information.

#pragma warning disable CA1000 // Do not declare static members on generic types

namespace Abc
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.ExceptionServices;

    using Anexn = System.ArgumentNullException;

    public static partial class Result
    {
        public static readonly Result<Unit> Ok = Result<Unit>.Ok(default);

        public static Result<T> Some<T>([DisallowNull]T value) where T : struct
            => new Just<T>(value);

        public static Result<T> Of<T>([AllowNull]T value)
            => value is null ? Result<T>.None : new Just<T>(value);
    }

    // Both an Option type and a Result type.
    public abstract class Result<T>
    {
        private protected Result() { }

        public static Result<T> None { get; } = new None<T>();

        public static Result<T> Ok([DisallowNull]T value)
            => new Just<T>(value);

        public static Result<T> Error<TError>(TError error)
            => new Failure<T, TError>(error);

        public static Result<T> Panic(ExceptionDispatchInfo edi)
            => new Panic<T>(edi);
    }

    public sealed class Just<T> : Result<T>
    {
        internal Just(T value)
        {
            Value = value ?? throw new Anexn(nameof(value));
        }

        public T Value { get; }
    }

    public class None<T> : Result<T>
    {
        internal None() { }
    }

    // Failure<T> extends None<T> to simplify pattern matching.
    public sealed class Failure<T, TError> : None<T>
    {
        internal Failure(TError error)
        {
            Error = error ?? throw new Anexn(nameof(error));
        }

        public TError Error { get; }
    }

    public sealed class Panic<T> : None<T>
    {
        internal Panic(ExceptionDispatchInfo edi)
        {
            Edi = edi ?? throw new Anexn(nameof(edi));
        }

        public ExceptionDispatchInfo Edi { get; }

        public void Rethrow()
            => Edi.Throw();
    }

    // Helpers.
    public partial class Result
    {
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Raison d'être of this method.")]
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
                return Result<Unit>.Panic(edi);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Raison d'être of ResultOrError.")]
        public static Result<TResult> TryWith<TResult>(Func<TResult> func)
        {
            if (func is null) { throw new Anexn(nameof(func)); }

            try
            {
                return Of(func());
            }
            catch (Exception ex)
            {
                var edi = ExceptionDispatchInfo.Capture(ex);
                return Result<TResult>.Panic(edi);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "[Intentionally] Raison d'être of this method.")]
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
                return Result<Unit>.Panic(edi);
            }
            finally
            {
                finallyAction();
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Raison d'être of ResultOrError.")]
        public static Result<TResult> TryFinally<TResult>(Func<TResult> func, Action finallyAction)
        {
            if (func is null) { throw new Anexn(nameof(func)); }
            if (finallyAction is null) { throw new Anexn(nameof(finallyAction)); }

            try
            {
                return Of(func());
            }
            catch (Exception ex)
            {
                var edi = ExceptionDispatchInfo.Capture(ex);
                return Result<TResult>.Panic(edi);
            }
            finally
            {
                finallyAction();
            }
        }
    }
}
