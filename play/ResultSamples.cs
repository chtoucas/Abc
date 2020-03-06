// See LICENSE.txt in the project root for license information.

#pragma warning disable CA1000 // Do not declare static members on generic types

namespace Abc
{
    using System;

    // Simulating Either<T, TError> with a ValueTuple.
    public static class Result<T, TError> where T : struct
    {
        public static (Result<T> success, TError error) Ok(T value)
            => (Result.Some(value), default);

        public static (Result<T> success, TError error) Error(TError error)
            => (Result.None<T>(), error);
    }

    // With classes instead of structs, we can use pattern matching.
    // Of course, if we had sum types like in F# it would be simpler and clearer.

    public static class ResultSamples
    {
        public static string SomeOrNone(bool ok)
        {
            var r = ok ? Result.Some(1) : Result.None<int>();

            return r switch
            {
                Result<int>.Some some => $"{some.Value}",
                Result<int>.None _ => "No value",
                _ => throw new InvalidOperationException()
            };
        }

        public static string SomeOrError(bool ok)
        {
            var result = Result.OfType<int>();
            var r = ok ? result.Some(1) : result.Error("Boum!!!");

            return r switch
            {
                Result<int>.Some some => $"{some.Value}",
                Result<int>.Error<string> err => err.Message,
                Result<int>.Threw exn => exn.Rethrow<string>(),
                _ => throw new InvalidOperationException()
            };
        }

        public static string OkOrError(bool ok)
        {
            var r = ok ? Result<int, string>.Ok(1) : Result<int, string>.Error("Boum!!!");

            return r switch
            {
                (Result<int>.Some some, _) => $"{some.Value}",
                (Result<int>.None _, string err) => err,
                _ => throw new InvalidOperationException()
            };
        }
    }
}
