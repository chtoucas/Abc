// See LICENSE.txt in the project root for license information.

namespace Abc.Rop
{
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides extension methods for <see cref="ResultFactory{T}"/>.
    /// <para>This class cannot be inherited.</para>
    /// </summary>
    public static class ResultFactory
    {
        [Pure]
        [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "The param is a discard")]
        public static Ok<T> Ok<T>(this ResultFactory<T> _, T value)
            where T : struct
            => new Ok<T>(value);

        [Pure]
        [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "The param is a discard")]
        public static Result<T> OkIfNotNull<T>(this ResultFactory<T> _, T? value)
            where T : struct
        {
            if (value.HasValue) { return new Ok<T>(value.Value); }
            else { return Error<T>.Instance; }
        }

        [Pure]
        [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "The param is a discard")]
        public static Result<T> OkIfNotNull<T>(this ResultFactory<T> _, T? value)
            where T : class
        {
            if (value is null) { return Error<T>.Instance; }
            else { return new Ok<T>(value); }
        }
    }

    public sealed class ResultFactory<T>
    {
        private ResultFactory() { }

        internal static readonly ResultFactory<T> Uniq = new ResultFactory<T>();

        [Pure]
        public Error<T> EmptyError => Abc.Rop.Error<T>.Instance;

        [Pure]
        public Error<T, TErr> Error<TErr>([DisallowNull]TErr err)
            => new Error<T, TErr>(err);
    }
}
