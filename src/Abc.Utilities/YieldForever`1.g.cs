﻿// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

namespace Abc.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    [DebuggerNonUserCode, DebuggerDisplay("Count = ∞")]
    internal sealed class YieldForever<T> : IEnumerable<T>, IEnumerator<T>
    {
        [NotNull] private readonly T _element;

        public YieldForever([DisallowNull] T element)
        {
            _element = element;
        }

        // IEnumerable<T>
        [Pure] public IEnumerator<T> GetEnumerator() => this;

        // IEnumerable
        [Pure] IEnumerator IEnumerable.GetEnumerator() => this;

        // IEnumerator<T>
        [Pure] public T Current => _element;

        // IEnumerator
        [Pure] object IEnumerator.Current => _element;
        [Pure] public bool MoveNext() => true;
        void IEnumerator.Reset() { }

        // IDisposable
        void IDisposable.Dispose() { }
    }
}
