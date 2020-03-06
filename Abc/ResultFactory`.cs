// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Runtime.ExceptionServices;

    /// <summary>
    /// Provides extension methods for <see cref="ResultFactory{T}"/>.
    /// <para>This class cannot be inherited.</para>
    /// </summary>
    public static class ResultFactory
    {
        [Pure]
        public static Result<T> Some<T>(this ResultFactory<T> @this, T value)
            where T : struct
            => new Result<T>.Some(value);

        [Pure]
        public static Result<T> SomeOrNone<T>(this ResultFactory<T> @this, [AllowNull]T value)
            => value is null ? ResultFactory<T>.None_ : new Result<T>.Some(value);

        [Pure]
        public static Result<T> SomeOrNone<T>(this ResultFactory<T> @this, T? value)
            where T : struct
            => value.HasValue ? new Result<T>.Some(value.Value) : ResultFactory<T>.None_;
    }

    public sealed class ResultFactory<T>
    {
        internal static readonly Result<T> None_ = new Result<T>.None();

        private ResultFactory() { }

        internal static readonly ResultFactory<T> Uniq = new ResultFactory<T>();

        public Result<T> None => None_;

        [Pure]
        public Result<T> Error<TErr>([DisallowNull]TErr message)
            => new Result<T>.Error<TErr>(message);

        [Pure]
        public Result<T> Threw(ExceptionDispatchInfo edi)
            => new Result<T>.Threw(edi);
    }

}
