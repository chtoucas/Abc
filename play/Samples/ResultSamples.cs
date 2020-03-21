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
            Result<int> r = ok ? Result.Some(1) : Result.None<int>();

            return r.IsError ? "Failure." : $"{r.Value}";
        }

        // Result POV.
        public static string SomeOrError(bool ok)
        {
            Result<int> r = ok ? Result.Some(1) : Result.Err<int>("Boum!!!");

            return r switch
            {
                Ok<int> _ => $"{r.Value}",
                Err<int> err when err.IsNone => "The method returned a null.",
                Err<int> err => err.Message,
                _ => throw new InvalidOperationException()
            };
        }
    }
}
