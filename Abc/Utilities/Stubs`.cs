// See LICENSE.txt in the project root for license information.

namespace Abc.Utilities
{
    using System;

    internal static class Stubs
    {
        /// <summary>
        /// Represents the action that does nothing.
        /// <para>This field is read-only.</para>
        /// </summary>
        public static readonly Action Noop = () => { };
    }

    internal static class Stubs<T>
    {
        /// <summary>
        /// Represents the function that always returns the default value.
        /// <para>This field is read-only.</para>
        /// </summary>
        public static readonly Func<T> ReturnsDefault = () => default!;

        /// <summary>
        /// Represents the identity map.
        /// <para>This field is read-only.</para>
        /// </summary>
        public static readonly Func<T, T> Ident = x => x;

        /// <summary>
        /// Represents the action that does nothing.
        /// <para>This field is read-only.</para>
        /// </summary>
        public static readonly Action<T> Ignore = _ => { };
    }

    internal static class Stubs<TSource, TResult>
    {
        /// <summary>
        /// Represents the function that always evaluates to the default value.
        /// <para>This field is read-only.</para>
        /// </summary>
        public static readonly Func<TSource, TResult> ReturnsDefault = _ => default!;
    }
}
