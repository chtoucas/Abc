// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    using Anexn = System.ArgumentNullException;
    using EF = Abc.Utilities.ExceptionFactory;

    [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Visual Basic: use an escaped name")]
    public sealed class Error<T> : Result<T>
    {
        public static readonly Error<T> None = new Error<T>("No value", true);

        public Error([DisallowNull] string message) : this(message, false) { }

        private Error([DisallowNull] string message, bool isNone)
        {
            Message = message ?? throw new Anexn(nameof(message));
            IsNone = isNone;
        }

        public override bool IsError => true;

        public override T Value { [DoesNotReturn] get => throw EF.Result_NoValue; }

        public bool IsNone { get; }

        [NotNull] public string Message { get; }

        public override string ToString()
            => Message;

        /// <summary>
        /// Deconstructs the current instance into its components.
        /// </summary>
        public void Deconstruct(out string message, out bool isNone)
            => (message, isNone) = (Message, IsNone);

        [Pure]
        public override Result<T> OrElse(Result<T> other)
            => other;

        [Pure]
        public override Result<TResult> Select<TResult>(Func<T, TResult> selector)
            => new Error<TResult>(Message);

        [Pure]
        public override Result<T> Where(Func<T, bool> predicate)
            => this;
    }
}
