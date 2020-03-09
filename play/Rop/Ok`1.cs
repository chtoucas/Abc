// See LICENSE.txt in the project root for license information.

namespace Abc.Rop
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    using Anexn = System.ArgumentNullException;

    /// <summary>
    /// Represents the successful outcome of a computation.
    /// <para>This class cannot be inherited.</para>
    /// </summary>
    public sealed class Ok<T> : Result<T>
    {
        internal Ok([DisallowNull]T value)
        {
            Value = value;
        }

        public override bool IsError => false;

        [NotNull] public override T Value { get; }

        [Pure]
        public override Result<T> OrElse(Result<T> other)
            => this;

        [Pure]
        public override Result<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            if (selector is null) { throw new Anexn(nameof(selector)); }
            return new Ok<TResult>(selector(Value));
        }

        [Pure]
        public override Result<T> Where(Func<T, bool> predicate)
        {
            if (predicate is null) { throw new Anexn(nameof(predicate)); }
            if (predicate(Value)) { return this; }
            else { return NullResult<T>.Instance; }
        }

        //[Pure]
        //public Result<TResult> SelectMany<TMiddle, TResult>(
        //    Func<T, Result<TMiddle>> selector,
        //    Func<T, TMiddle, TResult> resultSelector)
        //{
        //    if (selector is null) { throw new Anexn(nameof(selector)); }
        //    if (resultSelector is null) { throw new Anexn(nameof(resultSelector)); }

        //    var middle = selector(Value);
        //    if (!middle.IsSome) { return Result.None<TResult>(); }

        //    return Result.Of(resultSelector(Value, middle.Value));
        //}
    }
}
