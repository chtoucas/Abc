﻿// See LICENSE in the project root for license information.

#nullable enable

namespace Abc.Utilities
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    [DebuggerNonUserCode]
    internal abstract partial class Thunks
    {
        [ExcludeFromCodeCoverage]
        protected Thunks() { }
    }

    [DebuggerNonUserCode]
    internal abstract partial class Thunks<T>
    {
        [ExcludeFromCodeCoverage]
        protected Thunks() { }
    }

    internal partial class Thunks
    {
        /// <summary>
        /// Represents the action that does nothing.
        /// <para>This field is read-only.</para>
        /// </summary>
        public static readonly Action Noop = () => { };
    }

    internal partial class Thunks<T>
    {
        /// <summary>
        /// Represents the identity map.
        /// <para>This field is read-only.</para>
        /// </summary>
        public static readonly Func<T, T> Ident = x => x;

        /// <summary>
        /// Represents the action that does nothing.
        /// <para>This field is read-only.</para>
        /// </summary>
        public static readonly Action<T> Noop = _ => { };
    }
}
