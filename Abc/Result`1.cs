// See LICENSE.txt in the project root for license information.

// REVIEW: remove Bind()?
#if DEBUG
// Only in DEBUG mode, otherwise we would have to update PublicAPI.
//#define ENABLE_BIND
#endif

namespace Abc
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    using Anexn = System.ArgumentNullException;

    // Both an Option type and a Result type.
    public abstract class Result<T>
    {
        private protected Result() { }

#if ENABLE_BIND
        [Pure]
        public abstract Result<TResult> Bind<TResult>(Func<T, Result<TResult>> binder);
#endif

        [Pure]
        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Query Expression Pattern")]
        public abstract Result<TResult> Select<TResult>(Func<T, TResult> selector);

        [Pure]
        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords")]
        public abstract Result<T> OrElse(Result<T> other);

#pragma warning disable CA1034 // Nested types should not be visible

        public sealed class Some : Result<T>
        {
            internal Some(T value)
            {
                Value = value ?? throw new Anexn(nameof(value));
            }

            public T Value { get; }

#if ENABLE_BIND
            [Pure]
            public override Result<TResult> Bind<TResult>(Func<T, Result<TResult>> binder)
            {
                if (binder is null) { throw new Anexn(nameof(binder)); }
                return binder(Value);
            }
#endif

            [Pure]
            public override Result<TResult> Select<TResult>(Func<T, TResult> selector)
            {
                if (selector is null) { throw new Anexn(nameof(selector)); }
                return new Result<TResult>.Some(selector(Value));
            }

            [Pure]
            public override Result<T> OrElse(Result<T> other)
                => this;
        }

        public class None : Result<T>
        {
            internal None() { }

#if ENABLE_BIND
            [Pure]
            public override Result<TResult> Bind<TResult>(Func<T, Result<TResult>> binder)
                => ResultFactory<TResult>.None_;
#endif

            [Pure]
            public override Result<TResult> Select<TResult>(Func<T, TResult> selector)
                => ResultFactory<TResult>.None_;

            [Pure]
            public override Result<T> OrElse(Result<T> other)
                => other;
        }

        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords")]
        public abstract class Error : Result<T>
        {
            private protected Error() { }

            [Pure]
            public sealed override Result<T> OrElse(Result<T> other)
                => other;
        }

        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords")]
        public sealed class Error<TErr> : Error
        {
            internal Error(TErr err)
            {
                InnerErr = err ?? throw new Anexn(nameof(err));
            }

            public TErr InnerErr { get; }

#if ENABLE_BIND
            [Pure]
            public override Result<TResult> Bind<TResult>(Func<T, Result<TResult>> binder)
                => new Result<TResult>.Error<TErr>(InnerErr);
#endif

            [Pure]
            public override Result<TResult> Select<TResult>(Func<T, TResult> binder)
                => new Result<TResult>.Error<TErr>(InnerErr);
        }

#pragma warning restore CA1034
    }
}
