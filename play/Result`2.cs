// See LICENSE.txt in the project root for license information.

#pragma warning disable CA1000 // Do not declare static members on generic types

namespace Abc
{
    using System;

    // Hand-made Either using a ValueTuple.
    public static class Result<T, TError> where T : notnull
    {
        public static (Result<T> success, TError error) Ok(T value)
            => (Result<T>.Ok(value), default);

        public static (Result<T> success, TError error) Error(TError error)
            => (Result<T>.None, error);
    }

    // With classes instead of structs, we can use pattern matching.
    // Of course, if we had sum types like in F# it would be simpler and clearer.

    public static class Simple
    {
        public static string ShowOption(bool ok)
            => GetOption(ok) switch
            {
                Just<int> some => $"{some.Value}",
                None<int> _ => "No value",
                _ => throw new InvalidOperationException()
            };

        public static string ShowOutcome(bool ok)
            => GetOutcome(ok) switch
            {
                Just<int> some => $"{some.Value}",
                Failure<int, string> err => err.Error,
                _ => throw new InvalidOperationException()
            };

        public static string ShowEither(bool ok)
            => GetEither(ok) switch
            {
                (Just<int> some, _) => $"{some.Value}",
                (None<int> _, string err) => err,
                _ => throw new InvalidOperationException()
            };

        public static Result<int> GetOption(bool ok)
            => ok ? Result.Some(1) : Result<int>.None;

        public static Result<int> GetOutcome(bool ok)
            => ok ? Result.Some(1) : Result<int>.Error("Boum!!!");

        public static (Result<int> success, string err) GetEither(bool ok)
            => ok ? Result<int, string>.Ok(1) : Result<int, string>.Error("Boum!!!");
    }
}
