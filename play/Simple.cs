// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;

    // With classes instead of structs, we can use pattern matching.
    // Of course, if we had sum types like in F# it would be simpler and clearer.

    public static class Simple
    {
        public static string ShowOption(bool ok)
            => GetOption(ok) switch
            {
                Some<int> some => $"{some.Value}",
                None<int> _ => "No value",
                _ => throw new InvalidOperationException()
            };

        public static string ShowOutcome(bool ok)
            => GetOutcome(ok) switch
            {
                Result<int>.Success success => $"{success.Value}",
                Result<int>.Failure<string> err => err.Error,
                _ => throw new InvalidOperationException()
            };

        public static string ShowEither(bool ok)
            => GetEither(ok) switch
            {
                (Some<int> some, _) => $"{some.Value}",
                (None<int> _, string err) => err,
                _ => throw new InvalidOperationException()
            };

        public static Result<int> GetOption(bool ok)
            => ok ? Result<int>.Some(1) : Result<int>.None;

        public static Result<int> GetOutcome(bool ok)
            => ok ? Result<int>.Ok(1) : Result<int>.Error("Boum!!!");

        public static (Result<int> success, string err) GetEither(bool ok)
            => ok ? Result<int, string>.Ok(1) : Result<int, string>.Error("Boum!!!");
    }
}
