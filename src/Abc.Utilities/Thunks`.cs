﻿// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

#nullable enable

namespace Abc.Utilities
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    [DebuggerNonUserCode, _ExcludeFromCodeCoverage]
    internal abstract partial class Thunks
    {
        [ExcludeFromCodeCoverage]
        protected Thunks() { }
    }

    [DebuggerNonUserCode, _ExcludeFromCodeCoverage]
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
