// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

#nullable enable

namespace Abc.Utilities
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    [DebuggerNonUserCode]
    internal abstract partial class Guard
    {
        [ExcludeFromCodeCoverage]
        protected Guard() { }
    }

    internal partial class Guard
    {
#if !ABC_UTILITIES_ENABLE_CODE_COVERAGE
        [ExcludeFromCodeCoverage]
#endif
        [Pure]
        [DebuggerStepThrough]
        public static T NotNull<T>(T? value, string paramName) 
            where T : class
            => value ?? throw new ArgumentNullException(paramName);
    }
}
