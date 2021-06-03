// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

namespace Abc.Utilities
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;

    // Will most certainly be obsoleted with C# 9.0.
    // See https://github.com/dotnet/csharplang/issues/2145
    // REVIEW:
    // - add unconstrained versions.
    // - now that we have NRTs, the attr ValidatedNotNull seems useless?

    internal abstract partial class Guard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Guard"/> class.
        /// </summary>
        protected Guard() { }
    }

    internal partial class Guard
    {
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void NotNull<T>([ValidatedNotNull] T value, string paramName)
            where T : class
        {
            if (value is null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        // This is one is for use when calling a base constructor.
        [Pure, DebuggerNonUserCode, DebuggerStepThrough]
        public static T NotNullPassThru<T>([ValidatedNotNull] T value, string paramName)
            where T : class =>
            value ?? throw new ArgumentNullException(paramName);
    }
}
