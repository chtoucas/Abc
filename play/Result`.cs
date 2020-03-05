// See LICENSE.txt in the project root for license information.

#pragma warning disable CA1000 // Do not declare static members on generic types
#pragma warning disable CA1034 // Nested types should not be visible

namespace Abc
{
    // Hand-made Either using a ValueTuple.
    public static class Result<T, TError> where T : notnull
    {
        public static (Result<T> success, TError error) Ok(T value)
            => (Result<T>.Some(value), default);

        public static (Result<T> success, TError error) Error(TError error)
            => (Result<T>.None, error);
    }

    // Both an Option and a Result type.
    public abstract class Result<T> where T : notnull
    {
        private protected Result() { }

        public static Result<T> None => None<T>.Uniq;

        public static Result<T> Some(T value) => new Some<T>(value);

        //
        // Simulating a sum type with inheritance.
        //

        public static Result<T> Ok(T value) => new Success(value);

        public static Result<T> Error<TError>(TError error) => new Failure<TError>(error);

        public sealed class Success : Result<T>
        {
            internal Success(T value) => Value = value;

            public T Value { get; }
        }

        public sealed class Failure<TError> : Result<T>
        {
            internal Failure(TError error) => Error = error;

            public TError Error { get; }
        }
    }

    public sealed class Some<T> : Result<T> where T : notnull
    {
        internal Some(T value) => Value = value;

        public T Value { get; }
    }

    public sealed class None<T> : Result<T> where T : notnull
    {
        internal static readonly Result<T> Uniq = new None<T>();

        private None() { }
    }
}
