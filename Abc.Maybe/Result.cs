// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    using Anexn = System.ArgumentNullException;

    public static partial class Result { }

    public partial class Result
    {
        public static readonly Result<Unit> Unit = Some(default(Unit));

        public static readonly Err<Unit> Zero = Abc.Err<Unit>.None;

        [Pure]
        // Not actually obsolete, but clearly states that we shouldn't use it.
        [Obsolete("Use SomeOrNone() instead.")]
        public static Result<T> Of<T>(T? value) where T : struct
        {
            if (value.HasValue) { return new Ok<T>(value.Value); }
            else { return Result<T>.None; }
        }

        [Pure]
        public static Result<T> Of<T>([AllowNull]T value)
        {
            if (value is null) { return Result<T>.None; }
            else { return new Ok<T>(value); }
        }

        // Unconstrained version: see Result<T>.None.
        [Pure]
        public static Err<T> None<T>() where T : notnull
            => Abc.Err<T>.None;

        [Pure]
        public static Ok<T> Some<T>(T value) where T : struct
            => new Ok<T>(value);

        [Pure]
        public static Result<T> SomeOrNone<T>(T? value) where T : struct
        {
            if (value.HasValue) { return new Ok<T>(value.Value); }
            else { return Result<T>.None; }
        }

        [Pure]
        public static Result<T> SomeOrNone<T>(T? value) where T : class
        {
            if (value is null) { return Result<T>.None; }
            else { return new Ok<T>(value); }
        }

        [Pure]
        public static Err<Unit> Err([DisallowNull]string message)
            => new Err<Unit>(message);

        // Unconstrained version: new Err<T>(message).
        [Pure]
        public static Err<T> Err<T>([DisallowNull]string message) where T : notnull
            => new Err<T>(message);
    }

    public partial class Result
    {
        [Pure]
        public static Ok<T> Squash<T>(this Ok<T?> @this)
            where T : struct
        {
            if (@this is null) { throw new Anexn(nameof(@this)); }
            return Some(@this.Value.Value);
        }

        [Pure]
        public static Err<T> Squash<T>(this Err<T?> @this)
            where T : struct
        {
            if (@this is null) { throw new Anexn(nameof(@this)); }
            return @this.WithReturnType<T>();
        }

        [Pure]
        public static Result<T> Squash<T>(this Result<T?> @this)
            where T : struct
        {
            return @this switch
            {
                Ok<T?> ok => Some(ok.Value.Value),
                Err<T?> err => err.WithReturnType<T>(),
                null => throw new Anexn(nameof(@this)),
                _ => throw new InvalidOperationException()
            };
        }

        [Pure]
        public static Result<T> Flatten<T>(this Result<Result<T>> @this)
        {
            return @this switch
            {
                Ok<Ok<T>> ok => ok.Value,
                Ok<Err<T>> ok => ok.Value,
                Err<Ok<T>> err => err.WithReturnType<T>(),
                Err<Err<T>> err => err.WithReturnType<T>(),
                null => throw new Anexn(nameof(@this)),
                _ => throw new InvalidOperationException()
            };
        }
    }
}
