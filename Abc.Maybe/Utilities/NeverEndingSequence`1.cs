// See LICENSE.txt in the project root for license information.

namespace Abc.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    [DebuggerDisplay("Count = ∞")]
    internal sealed class NeverEndingSequence<T> : IEnumerable<T>, IEnumerator<T>
    {
        [NotNull] private readonly T _element;

        public NeverEndingSequence([DisallowNull] T element)
        {
            _element = element;
        }

        // IEnumerable<T>
        public IEnumerator<T> GetEnumerator() => this;

        // IEnumerable
        IEnumerator IEnumerable.GetEnumerator() => this;

        // IEnumerator<T>
        public T Current => _element;

        // IEnumerator
        object IEnumerator.Current => _element;
        public bool MoveNext() => true;
        void IEnumerator.Reset() { }

        // IDisposable
        void IDisposable.Dispose() { }
    }
}
