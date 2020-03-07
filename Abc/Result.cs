// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    // FIXME: Of() vs SomeOrNone().

    public static class Result
    {
        public static readonly Result<Unit> Ok = Some(default(Unit));

        [Pure]
        public static ResultFactory<T> OfType<T>()
            => ResultFactory<T>.Uniq;

        [Pure]
        public static Result<T> Of<T>([AllowNull]T value)
            => value is null ? ResultFactory<T>.None_ : new Result<T>.Some(value);

        [Pure]
        public static Result<T> None<T>() where T : notnull
            => ResultFactory<T>.None_;

        [Pure]
        public static Result<T> Some<T>(T value) where T : struct
            => new Result<T>.Some(value);

        [Pure]
        public static Result<T> SomeOrNone<T>(T? value) where T : struct
            => value.HasValue ? new Result<T>.Some(value.Value) : ResultFactory<T>.None_;

        [Pure]
        public static Result<T> SomeOrNone<T>(T? value) where T : class
            => value is null ? ResultFactory<T>.None_ : new Result<T>.Some(value);
    }
}
