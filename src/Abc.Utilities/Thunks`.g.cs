// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

#nullable enable

namespace Abc.Utilities
{
    using System;
    using System.Diagnostics;

    internal static partial class Thunks { }

    internal static partial class Thunks<T> { }

    internal partial class Thunks
    {
        /// <summary>
        /// Represents the action that does nothing.
        /// <para>This field is read-only.</para>
        /// </summary>
        [DebuggerNonUserCode]
        public static Action Noop { get; } = () => { };
    }

    internal partial class Thunks<T>
    {
        /// <summary>
        /// Represents the identity map.
        /// <para>This field is read-only.</para>
        /// </summary>
        [DebuggerNonUserCode]
        public static Func<T, T> Ident { get; } = x => x;

        /// <summary>
        /// Represents the action that does nothing.
        /// <para>This field is read-only.</para>
        /// </summary>
        [DebuggerNonUserCode]
        public static Action<T> Noop { get; } = _ => { };
    }
}
