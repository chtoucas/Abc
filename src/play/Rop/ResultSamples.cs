﻿// See LICENSE in the project root for license information.

namespace Abc
{
    using System;

    public static class ResultSamples
    {
        // Option POV.
        public static string SomeOrNone(bool pass)
        {
            Result<int> r = pass ? Result.Some(1) : Result.None<int>();

            return r.TryGetValue(out int value) ? "Nothing." : $"{value}";
        }

        // Result POV.
        public static string SomeOrError(bool pass)
        {
            Result<int> r = pass ? Result.Some(1) : Result.Err<int>("Boum!!!");

            return r switch
            {
                Ok<int> ok => $"{ok.Value}",
                Err<int> err when err.IsNone => "The method returned null.",
                Err<int> err => err.Message,
                _ => throw new InvalidOperationException()
            };
        }
    }
}