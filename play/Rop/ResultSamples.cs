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

    public static partial class ResultSamples { }

    public partial class ResultSamples
    {
        // Option POV.
        public static string SomeOrNone(bool ok)
        {
            Result<int> r = __fun();

            return r.IsError ? "No value" : $"{r.Value}";

            Result<int> __fun()
            {
                if (ok) { return Result.Some(1); }
                else { return Result.None<int>(); }
            }
        }

        // Result POV.
        public static string OkOrError(bool ok)
        {
            Result<int> r = __fun();

            return r switch
            {
                Ok<int> _ => $"{r.Value}",
                Error<int> err => err.Message,
                //Error<int> (string message, bool wasNull) => wasNull ? "BAD" : message,
                _ => throw new InvalidOperationException()
            };

            Result<int> __fun()
            {
                return ok ? Result.Of(1) : Result.OfType<int>.Error("Boum!!!");
            }
        }

        public static string OkOrThrew(bool ok)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            Exceptional<int> r = Exceptional.TryWith(__fun);
#pragma warning restore CS0618

            return r switch
            {
                Ok1<int> _ => $"{r.Value}",
                Threw<int> exn => Exceptional.Rethrow<string>(exn.InnerException),
                _ => throw new InvalidOperationException()
            };

            int __fun() => ok ? 1 : throw new DivideByZeroException();
        }

        public static string OkOrError1(bool ok)
        {
            var r = ok ? Result<int, string>.Ok(1) : Result<int, string>.Error("Boum!!!");

            return r switch
            {
                (Ok<int> some, _) => $"{some.Value}",
                (Error<int> _, string err) => err,
                _ => throw new InvalidOperationException()
            };
        }
    }
}
