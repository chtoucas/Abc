// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

#nullable enable

namespace Abc.Utilities
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    [DebuggerNonUserCode, _ExcludeFromCodeCoverage]
    internal abstract partial class Predicates
    {
        [ExcludeFromCodeCoverage]
        protected Predicates() { }
    }

    [DebuggerNonUserCode, _ExcludeFromCodeCoverage]
    internal abstract partial class Predicates<TSource>
    {
        [ExcludeFromCodeCoverage]
        protected Predicates() { }
    }

    internal partial class Predicates
    {
        /// <summary>
        /// Represents the function that always returns false.
        /// <para>This field is read-only.</para>
        /// </summary>
        public static readonly Func<bool> False = () => false;

        /// <summary>
        /// Represents the function that always returns true.
        /// <para>This field is read-only.</para>
        /// </summary>
        public static readonly Func<bool> True = () => true;
    }

    internal partial class Predicates<TSource>
    {
        /// <summary>
        /// Represents the predicate that always evaluates to false.
        /// <para>This field is read-only.</para>
        /// </summary>
        public static readonly Func<TSource, bool> False = _ => false;

        /// <summary>
        /// Represents the predicate that always evaluates to true.
        /// <para>This field is read-only.</para>
        /// </summary>
        public static readonly Func<TSource, bool> True = _ => true;
    }
}
