﻿// See LICENSE.txt in the project root for license information.

#pragma warning disable CA1710 // Identifiers should have correct suffix

namespace Abc
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Class to help for deferred execution tests: it throw an exception
    /// when <see cref="GetEnumerator"/> is called.
    /// </summary>
    public sealed class ThrowingEnumerable<T> : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator() => throw new InvalidOperationException();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}