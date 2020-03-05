// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.ExceptionServices;

    using Anexn = System.ArgumentNullException;

    public static partial class Result
    {
        public static readonly Result<Unit> Ok = Some(default(Unit));

        public static Result<T> Some<T>(T value) where T : struct
            => new Result<T>.Any(value);

        public static Result<T> Of<T>([AllowNull]T value)
            => value is null ? Result<T>.None : new Result<T>.Any(value);

        public static Result<T> Of<T>(T? value) where T : struct
            => value.HasValue ? Some(value.Value) : Result<T>.None;
    }

    // Both an Option type and a Result type.
    public abstract class Result<T>
    {
        private protected Result() { }

#pragma warning disable CA1000 // Do not declare static members on generic types

        public static Result<T> None { get; } = new Zero();

        public static Result<T> Error<TError>([DisallowNull]TError error)
            => new Fault<TError>(error);

        public static Result<T> Threw(ExceptionDispatchInfo edi)
            => new Panic(edi);

#pragma warning restore CA1000

#pragma warning disable CA1034 // Nested types should not be visible

        public sealed class Any : Result<T>
        {
            internal Any(T value)
            {
                Value = value ?? throw new Anexn(nameof(value));
            }

            public T Value { get; }
        }

        public class Zero : Result<T>
        {
            internal Zero() { }
        }

        // Fault<T> extends Zero to simplify pattern matching.
        public sealed class Fault<TError> : Zero
        {
            internal Fault(TError error)
            {
                Error = error ?? throw new Anexn(nameof(error));
            }

            public TError Error { get; }
        }

        // Fault<T> extends Zero to simplify pattern matching.
        public sealed class Panic : Zero
        {
            internal Panic(ExceptionDispatchInfo edi)
            {
                Edi = edi ?? throw new Anexn(nameof(edi));
            }

            public ExceptionDispatchInfo Edi { get; }

            public void Rethrow()
            {
                Edi.Throw();
            }

            public TResult Rethrow<TResult>()
            {
                Edi.Throw();
                return default!;
            }
        }
#pragma warning restore CA1034
    }

    // Helpers.
    public static partial class Result
    {
        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Raison d'être of this method.")]
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
                return Result<Unit>.Threw(edi);
            }
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Raison d'être of ResultOrError.")]
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
                return Result<TResult>.Threw(edi);
            }
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "[Intentionally] Raison d'être of this method.")]
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
                return Result<Unit>.Threw(edi);
            }
            finally
            {
                finallyAction();
            }
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Raison d'être of ResultOrError.")]
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
                return Result<TResult>.Threw(edi);
            }
            finally
            {
                finallyAction();
            }
        }
    }
}
