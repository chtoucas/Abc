// See LICENSE.txt in the project root for license information.

namespace Abc.Rop
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    // FIXME: Result type.
    // - Of() vs SomeOrNone(). Constraints.
    // - return type of Of, Some & co.
    // - add SelectMany & Join?
    // - construction is fine, pattern matching is not.

    public static class Result
    {
        [Pure]
        public static ResultFactory<T> OfType<T>()
            => ResultFactory<T>.Uniq;

        [Pure]
        // Not actually obsolete, but clearly states that we shouldn't use it.
        [Obsolete("Use SomeOrNone() instead.")]
        public static Result<T> Of<T>(T? value) where T : struct
        {
            if (value.HasValue) { return new Ok<T>(value.Value); }
            else { return NullResult<T>.Instance; }
        }

        [Pure]
        public static Result<T> Of<T>([AllowNull]T value)
        {
            if (value is null) { return NullResult<T>.Instance; }
            else { return new Ok<T>(value); }
        }

        [Pure]
        public static NullResult<T> None<T>() where T : notnull
            => NullResult<T>.Instance;

        [Pure]
        public static Ok<T> Some<T>(T value) where T : struct
            => new Ok<T>(value);

        [Pure]
        public static Result<T> SomeOrNone<T>(T? value) where T : struct
        {
            if (value.HasValue) { return new Ok<T>(value.Value); }
            else { return NullResult<T>.Instance; }
        }

        [Pure]
        public static Result<T> SomeOrNone<T>(T? value) where T : class
        {
            if (value is null) { return NullResult<T>.Instance; }
            else { return new Ok<T>(value); }
        }
    }

    // Since Value is public, Bind() is not really useful, furthermore
    // it would complicate the API.

    // Both an Option type and a Result type.
    public abstract class Result<T>
    {
        private protected Result() { }

        public abstract bool IsError { get; }

        [NotNull] public abstract T Value { get; }

        [Pure]
        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Visual Basic: use an escaped name")]
        public abstract Result<T> OrElse(Result<T> other);

        [Pure]
        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Query Expression Pattern")]
        public abstract Result<TResult> Select<TResult>(Func<T, TResult> selector);

        [Pure]
        public abstract Result<T> Where(Func<T, bool> predicate);
    }
}
