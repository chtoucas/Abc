// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System.Diagnostics.Contracts;

    /// <example>
    /// Usage recommendation:
    /// <code><![CDATA[
    /// using static MaybeFactory;
    /// ]]></code>
    /// </example>
    public static class MaybeFactory
    {
        /// <summary>
        /// Obtains an instance of the empty <see cref="Maybe{T}" />.
        /// <para>This static property is thread-safe.</para>
        /// </summary>
        public static Maybe<T> None<T>() => Maybe<T>.None;

        /// <summary>
        /// Creates a new instance of the <see cref="Maybe{T}"/> struct from the
        /// specified value.
        /// </summary>
        [Pure]
        public static Maybe<T> Some<T>(T value) where T : struct
            => new Maybe<T>(value);

        /// <summary>
        /// Creates a new instance of the <see cref="Maybe{T}"/> struct from the
        /// specified nullable value.
        /// </summary>
        [Pure]
        public static Maybe<T> SomeOrNone<T>(T? value) where T : struct
            // DO NOT REMOVE THIS METHOD.
            // Prevents the creation of a Maybe<T?> **directly**.
            => value.HasValue ? new Maybe<T>(value.Value) : Maybe<T>.None;

        /// <summary>
        /// Creates a new instance of the <see cref="Maybe{T}"/> struct from the
        /// specified nullable value.
        /// </summary>
        [Pure]
        public static Maybe<T> SomeOrNone<T>(T? value) where T : class
            => value is null ? Maybe<T>.None : new Maybe<T>(value);
    }
}
