// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

#nullable enable

namespace Abc.Utilities
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    [DebuggerNonUserCode]
    internal abstract partial class Require
    {
        [ExcludeFromCodeCoverage]
        protected Require() { }
    }

    internal partial class Require
    {
#if !ABC_UTILITIES_ENABLE_CODE_COVERAGE
        [ExcludeFromCodeCoverage]
#endif
        [DebuggerStepThrough]
        public static void NotNull<T>([ValidatedNotNull] T value, string paramName)
            where T : class
        {
            if (value is null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
