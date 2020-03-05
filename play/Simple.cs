// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;

    public static class Simple
    {
        public static string ShowOption(bool ok)
            => GetOption(ok) switch
            {
                Some<string> some => some.Value,
                None<string> _ => "No value",
                _ => throw new InvalidOperationException()
            };

        public static string ShowOutcome(bool ok)
            => GetOutcome(ok) switch
            {
                Success<string> success => success.Value,
                Failure<string, string> err => err.Error,
                _ => throw new InvalidOperationException()
            };

        public static Option<string> GetOption(bool ok)
            => ok ? Option.Of("OK") : Option<string>.None;

        public static Outcome<string> GetOutcome(bool ok)
            => ok ? Outcome.Success("OK") : Outcome.Failure<string, string>("Boum!!!");

        public static (Maybe<string> success, Maybe<string> err) GetMaybes(bool ok)
            => ok ? (Maybe.Of("OK"), Maybe<string>.None)
                : (Maybe<string>.None, Maybe.Of("Boum!!!"));
    }
}
