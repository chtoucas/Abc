// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    using Anexn = System.ArgumentNullException;
    using EF = Abc.Utilities.ExceptionFactory;

    // FIXME: Result type. Too many problems with the current design.
    // - Of() vs SomeOrNone(). Constraints.
    // - add SelectMany & Join?
    // - nested types make things a bit obscure. Construction is fine, pattern
    //   matching is not.

    // Since Value is public, Bind() is not really useful, furthermore
    // it would complicate the API.

    // Both an Option type and a Result type.
    public abstract class Result<T>
    {
        private protected Result() { }

        public abstract bool IsSome { get; }

        [NotNull] public abstract T Value { get; }

        [Pure]
        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Visual Basic: use an escaped name")]
        public abstract Result<T> OrElse(Result<T> other);

        [Pure]
        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Query Expression Pattern")]
        public abstract Result<TResult> Select<TResult>(Func<T, TResult> selector);

        [Pure]
        public abstract Result<T> Where(Func<T, bool> predicate);

#pragma warning disable CA1034 // Nested types should not be visible

        public sealed class Some : Result<T>
        {
            internal Some([DisallowNull]T value)
            {
                Value = value ?? throw new Anexn(nameof(value));
            }

            public override bool IsSome => true;

            [NotNull] public override T Value { get; }

            [Pure]
            public override Result<T> OrElse(Result<T> other)
                => this;

            [Pure]
            public override Result<TResult> Select<TResult>(Func<T, TResult> selector)
            {
                if (selector is null) { throw new Anexn(nameof(selector)); }
                return new Result<TResult>.Some(selector(Value));
            }

            [Pure]
            public override Result<T> Where(Func<T, bool> predicate)
            {
                if (predicate is null) { throw new Anexn(nameof(predicate)); }
                return predicate(Value) ? this : None.Uniq;
            }

            //[Pure]
            //public Result<TResult> SelectMany<TMiddle, TResult>(
            //    Func<T, Result<TMiddle>> selector,
            //    Func<T, TMiddle, TResult> resultSelector)
            //{
            //    if (selector is null) { throw new Anexn(nameof(selector)); }
            //    if (resultSelector is null) { throw new Anexn(nameof(resultSelector)); }

            //    var middle = selector(Value);
            //    if (!middle.IsSome) { return Result<TResult>.None.Uniq; }

            //    return Result.Of(resultSelector(Value, middle.Value));
            //}
        }

        public sealed class None : Result<T>
        {
            internal static readonly Result<T> Uniq = new None();

            private None() { }

            public override bool IsSome => false;

            public override T Value { [DoesNotReturn] get => throw EF.Result_NoValue; }

            [Pure]
            public override Result<T> OrElse(Result<T> other)
                => other;

            [Pure]
            public override Result<TResult> Select<TResult>(Func<T, TResult> selector)
                => Result<TResult>.None.Uniq;

            [Pure]
            public override Result<T> Where(Func<T, bool> predicate)
                => this;
        }

        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Visual Basic: use an escaped name")]
        public abstract class Error : Result<T>
        {
            private protected Error() { }

            public sealed override bool IsSome => false;

            public sealed override T Value { [DoesNotReturn] get => throw EF.Result_NoValue; }

            [Pure]
            public sealed override Result<T> OrElse(Result<T> other)
                => other;
        }

        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Visual Basic: use an escaped name")]
        public sealed class Error<TErr> : Error
        {
            internal Error([DisallowNull] TErr err)
            {
                InnerError = err ?? throw new Anexn(nameof(err));
            }

            [NotNull] public TErr InnerError { get; }

            [Pure]
            public override Result<TResult> Select<TResult>(Func<T, TResult> selector)
                => new Result<TResult>.Error<TErr>(InnerError);

            [Pure]
            public override Result<T> Where(Func<T, bool> predicate)
                => this;
        }

#pragma warning restore CA1034
    }
}
