// See LICENSE.txt in the project root for license information.

#pragma warning disable CA1000 // Do not declare static members on generic types
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace Abc
{
    public static class Option
    {
        public static Option<T> Of<T>(T value)
            => value is null ? Option<T>.None : Option<T>.Some(value);
    }

    public abstract class Option<T>
    {
        private protected Option() { }

        public static Option<T> None => None<T>.Uniq;

        internal static Option<T> Some(T value) => new Some<T>(value);
    }

    public sealed class Some<T> : Option<T>
    {
        internal Some(T value) => Value = value;

        public T Value { get; }
    }

    public sealed class None<T> : Option<T>
    {
        internal static readonly Option<T> Uniq = new None<T>();

        private None() { }
    }

    public static class Outcome
    {
        public static Outcome<T> Success<T>(T value) => new Success<T>(value);

        public static Outcome<T> Failure<T, TErr>(TErr error) => new Failure<T, TErr>(error);
    }

    public abstract class Outcome<T>
    {
        protected Outcome() { }
    }

    public class Success<T> : Outcome<T>
    {
        internal Success(T value) => Value = value;

        public T Value { get; }
    }

    public class Failure<T, TErr> : Outcome<T>
    {
        internal Failure(TErr error) => Error = error;

        public TErr Error { get; }
    }
}
