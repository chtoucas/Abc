﻿// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

#nullable enable

namespace Abc.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents the empty iterator.
    /// <para>This class cannot be inherited.</para>
    /// </summary>
#if !ABC_UTILITIES_ENABLE_CODE_COVERAGE
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
    [DebuggerNonUserCode]
    [DebuggerDisplay("Count = 0")]
    internal sealed class EmptyIterator<T> : IEnumerator<T>
    {
        public static readonly IEnumerator<T> Instance = new EmptyIterator<T>();

        private EmptyIterator() { }

        // No one should ever call these properties.
        // BONSANG! IEnumerator<T> interface.
        [ExcludeFromCodeCoverage] public T Current => default!;
        [ExcludeFromCodeCoverage] object? IEnumerator.Current => default;

        [Pure] public bool MoveNext() => false;

        void IEnumerator.Reset() { }
        void IDisposable.Dispose() { }
    }
}
