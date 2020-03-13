// See LICENSE.txt in the project root for license information.

namespace Abc.Utilities
{
    using System.Collections;
    using System.Collections.Generic;

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
        public static IEnumerable<T> Singleton<T>(T element)
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
            private readonly T _element;

            public SingletonList_(T element)
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
                yield return _element;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
