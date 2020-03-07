// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    public static class Result
    {
        public static readonly Result<Unit> Ok = Some(default(Unit));

        [Pure]
        public static ResultFactory<T> OfType<T>()
            => ResultFactory<T>.Uniq;

        [Pure]
        // Not actually obsolete, but clearly states that we shouldn't use it.
        [Obsolete("Use SomeOrNone() instead.")]
        public static Result<T> Of<T>(T? value) where T : struct
            => value.HasValue ? new Result<T>.Some(value.Value) : Result<T>.None.Uniq;

        [Pure]
        public static Result<T> Of<T>([AllowNull]T value)
            => value is null ? Result<T>.None.Uniq : new Result<T>.Some(value);

        [Pure]
        public static Result<T> None<T>() where T : notnull
            => Result<T>.None.Uniq;

        [Pure]
        public static Result<T> Some<T>(T value) where T : struct
            => new Result<T>.Some(value);

        [Pure]
        public static Result<T> SomeOrNone<T>(T? value) where T : struct
            => value.HasValue ? new Result<T>.Some(value.Value) : Result<T>.None.Uniq;

        [Pure]
        public static Result<T> SomeOrNone<T>(T? value) where T : class
            => value is null ? Result<T>.None.Uniq : new Result<T>.Some(value);

    }
}
