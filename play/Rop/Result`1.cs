// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    // FIXME: Result type.
    // - Of() vs SomeOrNone(). Constraints.
    // - return type of Of, Some & co.
    // - add SelectMany & Join?
    // - construction is fine, pattern matching is not.

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
