﻿// See LICENSE.txt in the project root for license information.

namespace Abc.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    using AoorException = System.ArgumentOutOfRangeException;
    using EF = ExceptionFactory;

    /// <summary>
    /// Represents the empty iterator.
    /// <para>This class cannot be inherited.</para>
    /// </summary>
    [DebuggerDisplay("Count = 0")]
    internal sealed class EmptyIterator<T> : IEnumerator<T>
    {
        public static readonly IEnumerator<T> Instance = new EmptyIterator<T>();

        private EmptyIterator() { }

        // No one should ever call these properties.
        [ExcludeFromCodeCoverage] [MaybeNull] public T Current => default;
        [ExcludeFromCodeCoverage] [MaybeNull] object IEnumerator.Current => default;

        public bool MoveNext() => false;

        void IEnumerator.Reset() { }
        void IDisposable.Dispose() { }
    }

    /// <summary>
    /// Represents a single value iterator, a read-only singleton set.
    /// <para>This iterator is resettable.</para>
    /// <para>This class cannot be inherited.</para>
    /// </summary>
    /// <remarks>
    /// We could use:
    /// <code>
    ///   return Enumerable.Repeat(element, 1);
    /// </code>
    /// but then many LINQ operators are optimized for lists, and
    /// Enumerable.Repeat() does not seem to produce one.
    /// </remarks>
    [DebuggerDisplay("Count = 1")]
    internal sealed class SingletonIterator<T>
        : IList<T>, IReadOnlyList<T>, IEnumerator<T>
    {
        [NotNull] private readonly T _element;
        private bool _done = false;

        public SingletonIterator([DisallowNull] T element)
        {
            _element = element;
        }

        public int Count => 1;

        public bool IsReadOnly => true;

        public T this[int index]
        {
            get => index == 0 ? _element : throw new AoorException(nameof(index));
            set => throw EF.ReadOnlyCollection;
        }

        public bool Contains(T item)
            => EqualityComparer<T>.Default.Equals(item, _element);

        public void CopyTo(T[] array, int arrayIndex)
            => array[arrayIndex] = _element;

        public int IndexOf(T item)
            => EqualityComparer<T>.Default.Equals(item, _element) ? 0 : -1;

        public void Add(T item) => throw EF.ReadOnlyCollection;
        public void Clear() => throw EF.ReadOnlyCollection;
        public void Insert(int index, T item) => throw EF.ReadOnlyCollection;
        public bool Remove(T item) => throw EF.ReadOnlyCollection;
        public void RemoveAt(int index) => throw EF.ReadOnlyCollection;

        public IEnumerator<T> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        #region IEnumerator<T>

        // Common behaviour:
        // - before any call to MoveNext(), returns default(T)
        // - when done iterating, returns the last value
        // Here, we always return _element.
        public T Current => _element;
        object IEnumerator.Current => _element;

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

        #endregion
    }

    [DebuggerDisplay("Count = ∞")]
    internal sealed class NeverEndingIterator<T> : IEnumerable<T>, IEnumerator<T>
    {
        [NotNull] private readonly T _element;

        public NeverEndingIterator([DisallowNull] T element)
        {
            _element = element;
        }

        public IEnumerator<T> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        #region IEnumerator<T>

        public T Current => _element;
        object IEnumerator.Current => _element;

        public bool MoveNext() => true;

        void IEnumerator.Reset() { }
        void IDisposable.Dispose() { }

        #endregion
    }
}
