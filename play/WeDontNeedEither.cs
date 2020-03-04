// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;

    public static class WeDontNeedEither
    {
        public static string ShowResult(bool succeeded)
            => TestMethod(succeeded) switch
            {
                Success<string> success => success.Value,
                Failure<string, string> err => err.Value,
                _ => throw new InvalidOperationException()
            };

        public static Result<string> TestMethod(bool succeeded)
        {
            if (succeeded)
            {
                return new Success<string>("OK");
            }
            else
            {
                return new Failure<string, string>("Boum!!!");
            }
        }

        public static (Maybe<string> success, Maybe<string> err) TestMethod2(bool succeeded)
        {
            if (succeeded)
            {
                return (Maybe.Of("OK"), Maybe<string>.None);
            }
            else
            {
                return (Maybe<string>.None, Maybe.Of("Boum!!!"));
            }
        }
    }

    public abstract class Result<T>
    {
        protected Result() { }
    }

    public class Success<T> : Result<T>
    {
        public Success(T value) => Value = value;

        public T Value { get; }

        public override string ToString() => $"Success({Value})";
    }

    public class Failure<T, TErr> : Result<T>
    {
        public Failure(TErr value) => Value = value;

        public TErr Value { get; }

        public override string ToString() => $"Error({Value})";
    }
}
