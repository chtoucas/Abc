﻿// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

namespace Abc.Utilities
{
    using System;
    using System.Diagnostics;

    internal abstract partial class Predicates
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Predicates"/> class.
        /// </summary>
        [DebuggerNonUserCode]
        protected Predicates() { }
    }

    internal abstract partial class Predicates<TSource>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Predicate{T}"/> class.
        /// </summary>
        [DebuggerNonUserCode]
        protected Predicates() { }
    }

    internal partial class Predicates
    {
        /// <summary>
        /// Represents the function that always returns false.
        /// <para>This field is read-only.</para>
        /// </summary>
        [DebuggerNonUserCode]
        public static Func<bool> False { get; } = () => false;

        /// <summary>
        /// Represents the function that always returns true.
        /// <para>This field is read-only.</para>
        /// </summary>
        [DebuggerNonUserCode]
        public static Func<bool> True { get; } = () => true;
    }

    internal partial class Predicates<TSource>
    {
        /// <summary>
        /// Represents the predicate that always evaluates to false.
        /// <para>This field is read-only.</para>
        /// </summary>
        [DebuggerNonUserCode]
        public static Func<TSource, bool> False { get; } = _ => false;

        /// <summary>
        /// Represents the predicate that always evaluates to true.
        /// <para>This field is read-only.</para>
        /// </summary>
        [DebuggerNonUserCode]
        public static Func<TSource, bool> True { get; } = _ => true;
    }
}
