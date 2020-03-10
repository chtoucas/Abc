// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Anexn = System.ArgumentNullException;

    // To simplify things, we mirror the API of Maybe<T>.
    // This explains why, sometimes, a method might seem superfluous (it's
    // especially the case with methods related to IEnumerable<>). These methods
    // appear to be very simple since we can access the property Value from an
    // outside assembly, this is not the case with Maybe<T>,
    public static partial class Result { }

    // Factory methods.
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
        public static Err<Unit> Err(string message)
            => new Err<Unit>(message);

        // Unconstrained version: new Err<T>(message).
        [Pure]
        public static Err<T> Err<T>(string message) where T : notnull
            => new Err<T>(message);
    }

    public partial class Result
    {
        [Pure]
        public static Result<T> Flatten<T>(this Result<Result<T>> @this)
        {
            return @this switch
            {
                // Return an Ok<T>.
                Ok<Ok<T>> ok => ok.Value,
                // Return an Err<T>.
                Ok<Err<T>> ok => ok.Value,
                Err<Ok<T>> err => err.WithGenericType<T>(),
                Err<Err<T>> err => err.WithGenericType<T>(),
                // Throw.
                null => throw new Anexn(nameof(@this)),
                _ => throw new InvalidOperationException()
            };
        }
    }

    // Extension methods for Result<T> where T is a struct.
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
            return @this.WithGenericType<T>();
        }

        [Pure]
        public static Result<T> Squash<T>(this Result<T?> @this)
            where T : struct
        {
            return @this switch
            {
                Ok<T?> ok => Some(ok.Value.Value),
                Err<T?> err => err.WithGenericType<T>(),
                null => throw new Anexn(nameof(@this)),
                _ => throw new InvalidOperationException()
            };
        }
    }

    // Extension methods for Result<T> where T is enumerable.
    // LINQ extensions for IEnumerable<Result<T>>.
    public partial class Result
    {
        [Pure]
        public static Result<IEnumerable<T>> Empty<T>()
            => ResultEnumerable_<T>.Empty;

        [Pure]
        public static IEnumerable<T> ValueOrEmpty<T>(this Result<IEnumerable<T>> @this)
        {
            if (@this is null) { throw new Anexn(nameof(@this)); }

            return @this.IsError ? Enumerable.Empty<T>() : @this.Value;
        }

        [Pure]
        public static IEnumerable<T> CollectAny<T>(IEnumerable<Result<T>> source)
        {
            return from x in source where !x.IsError select x.Value;
        }

        [Pure]
        public static Result<T> Any<T>(IEnumerable<Result<T>> source)
        {
            if (source is null) { throw new Anexn(nameof(source)); }

            foreach (Result<T> item in source)
            {
                if (!item.IsError) { return item; }
            }

            return Result<T>.None;
        }

        private static class ResultEnumerable_<T>
        {
            internal static readonly Result<IEnumerable<T>> Empty
                = Of(Enumerable.Empty<T>());
        }
    }
}
