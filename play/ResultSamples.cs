﻿// See LICENSE.txt in the project root for license information.

#pragma warning disable CA1000 // Do not declare static members on generic types

namespace Abc
{
    using System;

    public static class ResultSamples
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
        public static string SomeOrError(bool ok)
        {
            Result<int> r = __fun();

            return r switch
            {
                Ok<int> _ => $"{r.Value}",
                Err<int> err => err.Message,
                _ => throw new InvalidOperationException()
            };

            Result<int> __fun()
            {
                if (ok) { return Result.Some(1); }
                else { return Result.Err<int>("Boum!!!"); }
            }
        }

        public static string OkOrThrew(bool ok)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            Exceptional<int> exn = Exceptional.TryWith(__fun);
#pragma warning restore CS0618

            return exn.IsError
                // FIXME: NULL_FORGIVING
                ? Exceptional.Rethrow<string>(exn.InnerException!)
                : $"{exn.Value}";

            int __fun() => ok ? 1 : throw new DivideByZeroException();
        }
    }
}
