// See LICENSE.txt in the project root for license information.

namespace Abc.Rop
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    using Anexn = System.ArgumentNullException;
    using EF = Abc.Utilities.ExceptionFactory;

    [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Visual Basic: use an escaped name")]
    public sealed class Error<T> : Result<T>
    {
        public static readonly Error<T> Instance = new Error<T>();

        private Error() { }

        public override bool IsError => true;

        public override T Value { [DoesNotReturn] get => throw EF.Result_NoValue; }

        [Pure]
        public override Result<T> OrElse(Result<T> other)
            => other;

        [Pure]
        public override Result<TResult> Select<TResult>(Func<T, TResult> selector)
            => Error<TResult>.Instance;

        [Pure]
        public override Result<T> Where(Func<T, bool> predicate)
            => this;
    }

    [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Visual Basic: use an escaped name")]
    public sealed class Error<T, TErr> : Result<T>
    {
        public Error([DisallowNull] TErr err)
        {
            InnerError = err ?? throw new Anexn(nameof(err));
        }

        public override bool IsError => true;

        public override T Value { [DoesNotReturn] get => throw EF.Result_NoValue; }

        [NotNull] public TErr InnerError { get; }

        [Pure]
        public override Result<T> OrElse(Result<T> other)
            => other;

        [Pure]
        public override Result<TResult> Select<TResult>(Func<T, TResult> selector)
            => new Error<TResult, TErr>(InnerError);

        [Pure]
        public override Result<T> Where(Func<T, bool> predicate)
            => this;
    }
}
