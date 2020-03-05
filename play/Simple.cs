// See LICENSE.txt in the project root for license information.

#pragma warning disable CA1000 // Do not declare static members on generic types

namespace Abc
{
    using System;

    // Hand-made Either using a ValueTuple.
    public static class Result<T, TError> where T : notnull
    {
        public static (Result<T> success, TError error) Ok(T value)
            => (Result.Of(value), default);

        public static (Result<T> success, TError error) Error(TError error)
            => (Result<T>.None, error);
    }

    // With classes instead of structs, we can use pattern matching.
    // Of course, if we had sum types like in F# it would be simpler and clearer.

    public static class Simple
    {
        public static string ShowSomeOrNone(bool ok)
            => SomeOrNone(ok) switch
            {
                Result<int>.Any some => $"{some.Value}",
                Result<int>.Zero _ => "No value",
                _ => throw new InvalidOperationException()
            };

        public static string ShowSomeOrError(bool ok)
            => SomeOrError(ok) switch
            {
                Result<int>.Any some => $"{some.Value}",
                Result<int>.Fault<string> err => err.Error,
                Result<int>.Panic exn => exn.Rethrow<string>(),
                _ => throw new InvalidOperationException()
            };

        public static string ShowEither(bool ok)
            => Either(ok) switch
            {
                (Result<int>.Any some, _) => $"{some.Value}",
                (Result<int>.Zero _, string err) => err,
                _ => throw new InvalidOperationException()
            };

        public static Result<int> SomeOrNone(bool ok)
            => ok ? Result.Some(1) : Result<int>.None;

        public static Result<int> SomeOrError(bool ok)
            => ok ? Result.Some(1) : Result<int>.Error("Boum!!!");

        public static (Result<int> success, string err) Either(bool ok)
            => ok ? Result<int, string>.Ok(1) : Result<int, string>.Error("Boum!!!");
    }
}
