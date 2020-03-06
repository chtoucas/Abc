// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Runtime.ExceptionServices;

    using Anexn = System.ArgumentNullException;

    // Both an Option type and a Result type.
    public abstract class Result<T>
    {
        private protected Result() { }

        [Pure]
        public abstract Result<TResult> Bind<TResult>(Func<T, Result<TResult>> binder);

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

            [Pure]
            public override Result<TResult> Bind<TResult>(Func<T, Result<TResult>> binder)
            {
                if (binder is null) { throw new Anexn(nameof(binder)); }
                return binder(Value);
            }

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

        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords")]
        public abstract class Error : Result<T>
        {
            private protected Error() { }

            [Pure]
            public sealed override Result<T> OrElse(Result<T> other)
                => other;
        }

        public class None : Error
        {
            internal None() { }

            [Pure]
            public override Result<TResult> Bind<TResult>(Func<T, Result<TResult>> binder)
                => ResultFactory<TResult>.None_;

            [Pure]
            public override Result<TResult> Select<TResult>(Func<T, TResult> selector)
                => ResultFactory<TResult>.None_;
        }

        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords")]
        public sealed class Error<TErr> : Error
        {
            internal Error(TErr err)
            {
                InnerErr = err ?? throw new Anexn(nameof(err));
            }

            public TErr InnerErr { get; }

            [Pure]
            public override Result<TResult> Bind<TResult>(Func<T, Result<TResult>> binder)
                => new Result<TResult>.Error<TErr>(InnerErr);

            [Pure]
            public override Result<TResult> Select<TResult>(Func<T, TResult> binder)
                => new Result<TResult>.Error<TErr>(InnerErr);
        }

        public sealed class Threw : Error
        {
            internal Threw(ExceptionDispatchInfo edi)
            {
                Edi = edi ?? throw new Anexn(nameof(edi));
            }

            public ExceptionDispatchInfo Edi { get; }

            [Pure]
            public override Result<TResult> Bind<TResult>(Func<T, Result<TResult>> binder)
                => new Result<TResult>.Threw(Edi);

            [Pure]
            public override Result<TResult> Select<TResult>(Func<T, TResult> binder)
                => new Result<TResult>.Threw(Edi);

            public void Rethrow()
                => Edi.Throw();

            public TResult Rethrow<TResult>()
            {
                Edi.Throw();
                return default!;
            }
        }
#pragma warning restore CA1034
    }
}
