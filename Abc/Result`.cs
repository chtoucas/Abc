// See LICENSE.txt in the project root for license information.

#pragma warning disable CA1000 // Do not declare static members on generic types

namespace Abc
{
    // Both an Option type and a Result type.
    public abstract class Result<T> where T : notnull
    {
        private protected Result() { }

        public static Result<T> None { get; } = new None<T>();

        public static Result<T> Ok(T value) => new Just<T>(value);

        public static Result<T> Error<TError>(TError error) => new Failure<T, TError>(error);
    }

    public sealed class Just<T> : Result<T> where T : notnull
    {
        internal Just(T value) => Value = value;

        public T Value { get; }
    }

    public class None<T> : Result<T> where T : notnull
    {
        internal None() { }
    }

    // Failure<T> extends None<T> to simplify pattern matching.
    public sealed class Failure<T, TError> : None<T> where T : notnull
    {
        internal Failure(TError error) => Error = error;

        public TError Error { get; }
    }
}
