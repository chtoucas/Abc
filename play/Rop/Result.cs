// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    using Abc.Utilities;

    using Anexn = System.ArgumentNullException;

    public static partial class Result { }

    public partial class Result
    {
        public static readonly Result<Unit> Ok = Some(default(Unit));

        [Pure]
        // Not actually obsolete, but clearly states that we shouldn't use it.
        [Obsolete("Use SomeOrNone() instead.")]
        public static Result<T> Of<T>(T? value) where T : struct
        {
            if (value.HasValue) { return new Ok<T>(value.Value); }
            else { return Abc.Error<T>.None; }
        }

        [Pure]
        public static Result<T> Of<T>([AllowNull]T value)
        {
            if (value is null) { return Abc.Error<T>.None; }
            else { return new Ok<T>(value); }
        }

        [Pure]
        public static Error<T> None<T>() where T : notnull
            => Abc.Error<T>.None;

        [Pure]
        public static Ok<T> Some<T>(T value) where T : struct
            => new Ok<T>(value);

        [Pure]
        public static Result<T> SomeOrNone<T>(T? value) where T : struct
        {
            if (value.HasValue) { return new Ok<T>(value.Value); }
            else { return Abc.Error<T>.None; }
        }

        [Pure]
        public static Result<T> SomeOrNone<T>(T? value) where T : class
        {
            if (value is null) { return Abc.Error<T>.None; }
            else { return new Ok<T>(value); }
        }

        [SuppressMessage("Design", "CA1034:Nested types should not be visible")]
        public static class OfType<T>
        {
            [Pure]
            [SuppressMessage("Design", "CA1000:Do not declare static members on generic types")]
            public static Error<T> Error([DisallowNull]string message)
                => new Error<T>(message);
        }
    }

    public partial class Result
    {
        public static Maybe<T> ToMaybe<T>(this Result<T> @this)
        {
            Require.NotNull(@this, nameof(@this));
            // This method converts all errors to an empty maybe.
            return @this.IsError ? Maybe<T>.None : Maybe.Of(@this.Value);
        }

        public static Ok<T> Squash<T>(this Ok<T?> @this)
            where T : struct
        {
            Require.NotNull(@this, nameof(@this));
            return Some(@this.Value.Value);
        }

        public static Error<T> Squash<T>(this Error<T?> @this)
            where T : struct
        {
            Require.NotNull(@this, nameof(@this));
            return @this.IsNone ? Abc.Error<T>.None : new Error<T>(@this.Message);
        }

        public static Result<T> Squash<T>(this Result<T?> @this)
            where T : struct
        {
            return @this switch
            {
                Ok<T?> ok => Some(ok.Value.Value),
                Error<T?> err when err.IsNone => Abc.Error<T>.None,
                Error<T?> err when !err.IsNone => new Error<T>(err.Message),
                null => throw new Anexn(nameof(@this)),
                _ => throw new InvalidOperationException()
            };
        }

        public static Result<T> Flatten<T>(this Result<Result<T>> @this)
        {
            return @this switch
            {
                Ok<Ok<T>> ok => ok.Value,
                Ok<Error<T>> ok => ok.Value,
                Error<Ok<T>> err when err.IsNone => Abc.Error<T>.None,
                Error<Ok<T>> err when !err.IsNone => new Error<T>(err.Message),
                Error<Error<T>> err when err.IsNone => Abc.Error<T>.None,
                Error<Error<T>> err when !err.IsNone => new Error<T>(err.Message),
                null => throw new Anexn(nameof(@this)),
                _ => throw new InvalidOperationException()
            };
        }
    }
}
