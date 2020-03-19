// See LICENSE.txt in the project root for license information.

namespace Abc.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using AoorException = System.ArgumentOutOfRangeException;
    using EF = ExceptionFactory;

    /// <summary>
    /// Provides static helpers to produce new sequences.
    /// <para>This class cannot be inherited.</para>
    /// </summary>
    internal static class Sequence
    {
        /// <summary>
        /// Generates a sequence that contains exactly one element.
        /// </summary>
        public static IEnumerable<T> Singleton<T>([DisallowNull] T element)
        {
            // We could write:
            //   return Enumerable.Repeat(element, 1);
            // but then many LINQ operators are optimized for lists, and
            // Enumerable.Repeat() does not seem to produce one.
            return new SingletonList_<T>(element);
        }

        /// <summary>
        /// Generates an infinite sequence of one repeated element.
        /// </summary>
        public static IEnumerable<T> Repeat<T>(T element)
        {
            while (true)
            {
                yield return element;
            }
        }

        private sealed class SingletonList_<T> : IList<T>, IReadOnlyList<T>
        {
            [NotNull] private readonly T _element;

            public SingletonList_([DisallowNull] T element)
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

            public IEnumerator<T> GetEnumerator()
            {
                // REVIEW: yield return _element;
                return new Iterator_(this);
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            private sealed class Iterator_ : IEnumerator<T>
            {
                private readonly T _value;

                private bool _done = false;

                public Iterator_(SingletonList_<T> list)
                {
                    _value = list._element;
                }

                public T Current => _value;

                object? IEnumerator.Current => _value;

                public bool MoveNext()
                {
                    if (_done) { return false; }

                    _done = true;
                    return true;
                }

                public void Reset()
                    => throw new NotSupportedException();

                public void Dispose() { }
            }
        }
    }
}
