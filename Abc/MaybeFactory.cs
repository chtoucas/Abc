// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides static methods to create new instances of the
    /// <see cref="Maybe{T}"/> struct.
    /// <para>This class cannot be inherited.</para>
    /// </summary>
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
        /// </summary>
        /// <remarks>
        /// To obtain the empty maybe for an unconstrained type, please see
        /// <see cref="Maybe{T}.None"/>.
        /// </remarks>
        public static Maybe<T> None<T>() where T : notnull
            => Maybe<T>.None;

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
        /// <remarks>
        /// To create a maybe for an unconstrained type, please see
        /// <see cref="Maybe.Of{T}(T)"/>.
        /// </remarks>
        [Pure]
        public static Maybe<T> SomeOrNone<T>(T? value) where T : struct
            // DO NOT REMOVE THIS METHOD.
            // Prevents the creation of a Maybe<T?> **directly**.
            => value.HasValue ? new Maybe<T>(value.Value) : Maybe<T>.None;

        /// <summary>
        /// Creates a new instance of the <see cref="Maybe{T}"/> struct from the
        /// specified nullable value.
        /// </summary>
        /// <remarks>
        /// To create a maybe for an unconstrained type, please see
        /// <see cref="Maybe.Of{T}(T)"/>.
        /// </remarks>
        [Pure]
        public static Maybe<T> SomeOrNone<T>(T? value) where T : class
            => value is null ? Maybe<T>.None : new Maybe<T>(value);
    }
}
