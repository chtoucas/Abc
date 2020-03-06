// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.ExceptionServices;

    using Anexn = System.ArgumentNullException;

    // Both an Option type and a Result type.
    public abstract class Result<T>
    {
        private protected Result() { }

        internal static readonly Result<T> None_ = new None();

#pragma warning disable CA1034 // Nested types should not be visible

        public sealed class Some : Result<T>
        {
            internal Some(T value)
            {
                Value = value ?? throw new Anexn(nameof(value));
            }

            public T Value { get; }
        }

        public class None : Result<T>
        {
            internal None() { }
        }

        // REVIEW: Error<TErr> extends None to simplify pattern matching.
        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords")]
        public sealed class Error<TErr> : None
        {
            internal Error(TErr message)
            {
                Message = message ?? throw new Anexn(nameof(message));
            }

            public TErr Message { get; }
        }

        public sealed class Threw : Result<T>
        {
            internal Threw(ExceptionDispatchInfo edi)
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

    public static class Result
    {
        public static readonly Result<Unit> Ok = Some(default(Unit));

        public static Result<T> Some<T>(T value) where T : struct
            => new Result<T>.Some(value);

        public static Result<T> SomeOrNone<T>([AllowNull]T value)
            => value is null ? Result<T>.None_ : new Result<T>.Some(value);

        public static Result<T> SomeOrNone<T>(T? value) where T : struct
            => value.HasValue ? new Result<T>.Some(value.Value) : OfType<T>.None;


        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Fluent API")]
        public static class OfType<T>
        {
#pragma warning disable CA1000 // Do not declare static members on generic types

            public static Result<T> None => Result<T>.None_;

            public static Result<T> Error<TErr>([DisallowNull]TErr message)
                => new Result<T>.Error<TErr>(message);

            public static Result<T> Threw(ExceptionDispatchInfo edi)
                => new Result<T>.Threw(edi);

#pragma warning restore CA1000
        }

#pragma warning disable CA1031 // Do not catch general exception types

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
                return OfType<Unit>.Threw(edi);
            }
        }

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
                return OfType<TResult>.Threw(edi);
            }
        }

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
                return OfType<Unit>.Threw(edi);
            }
            finally
            {
                finallyAction();
            }
        }

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
                return OfType<TResult>.Threw(edi);
            }
            finally
            {
                finallyAction();
            }
        }

#pragma warning restore CA1031
    }
}
