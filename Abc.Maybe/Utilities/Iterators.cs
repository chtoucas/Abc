// See LICENSE.txt in the project root for license information.

namespace Abc.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents the empty iterator.
    /// </summary>
    [DebuggerDisplay("Count = 0")]
    internal sealed class EmptyIterator<T> : IEnumerator<T>
    {
        public static readonly IEnumerator<T> Instance = new EmptyIterator<T>();

        private EmptyIterator() { }

        // No one should ever call these properties.
        [MaybeNull] public T Current => default;
        [MaybeNull] object IEnumerator.Current => default;

        public bool MoveNext() => false;

        void IEnumerator.Reset() { }
        void IDisposable.Dispose() { }
    }

    /// <summary>
    /// Represents a single value iterator.
    /// <para>This iterator is resettable.</para>
    /// </summary>
    [DebuggerDisplay("Count = 1")]
    internal sealed class ValueIterator<T> : IEnumerator<T>
    {
        [NotNull] private readonly T _value;
        private bool _done = false;

        public ValueIterator([DisallowNull] T value) => _value = value;

        // Common behaviour:
        // - before any call to MoveNext(), returns default(T)
        // - when done iterating, returns the last value
        // Here, we always return _value.
        public T Current => _value;
        object IEnumerator.Current => _value;

        public bool MoveNext()
        {
            if (_done) { return false; }

            _done = true;
            return true;
        }

        // It seems that it is now a requirement to throw an exception
        // (eg not supported), anyway it doesn't really matter.
        void IEnumerator.Reset() => _done = false;
        void IDisposable.Dispose() { }
    }
}
