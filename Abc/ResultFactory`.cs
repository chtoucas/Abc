﻿// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides extension methods for <see cref="ResultFactory{T}"/>.
    /// <para>This class cannot be inherited.</para>
    /// </summary>
    public static class ResultFactory
    {
#pragma warning disable CA1707 // Identifiers should not contain underscores

        [Pure]
        public static Result<T> Some<T>(this ResultFactory<T> _, T value)
            where T : struct
            => new Result<T>.Some(value);

        [Pure]
        public static Result<T> SomeOrNone<T>(this ResultFactory<T> _, T? value)
            where T : struct
            => value.HasValue ? new Result<T>.Some(value.Value) : Result<T>.None.Uniq;

        [Pure]
        public static Result<T> SomeOrNone<T>(this ResultFactory<T> _, T? value)
            where T : class
            => value is null ? Result<T>.None.Uniq : new Result<T>.Some(value);

#pragma warning restore CA1707
    }

    public sealed class ResultFactory<T>
    {
        private ResultFactory() { }

        internal static readonly ResultFactory<T> Uniq = new ResultFactory<T>();

        public Result<T> None => Result<T>.None.Uniq;

        [Pure]
        public Result<T> Error<TErr>([DisallowNull]TErr err)
            => new Result<T>.Error<TErr>(err);
    }

}
