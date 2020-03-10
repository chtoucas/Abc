// See LICENSE.txt in the project root for license information.

#pragma warning disable CA1000 // Do not declare static members on generic types

namespace Abc.Samples
{
    using System;

    public static class ResultSamples
    {
        // Option POV.
        public static string SomeOrNone(bool ok)
        {
            Result<int> r = __fun(ok);

            return r.IsError ? "Failure." : $"{r.Value}";

            static Result<int> __fun(bool ok)
            {
                if (ok) { return Result.Some(1); }
                else { return Result.None<int>(); }
            }
        }

        // Result POV.
        public static string SomeOrError(bool ok)
        {
            Result<int> r = __fun(ok);

            return r switch
            {
                Ok<int> _ => $"{r.Value}",
                Err<int> err when err.IsNone => "The method returned a null.",
                Err<int> err => err.Message,
                _ => throw new InvalidOperationException()
            };

            static Result<int> __fun(bool ok)
            {
                if (ok) { return Result.Some(1); }
                else { return Result.Err<int>("Boum!!!"); }
            }
        }
    }
}
