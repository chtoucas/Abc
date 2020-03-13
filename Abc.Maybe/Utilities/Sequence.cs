// See LICENSE.txt in the project root for license information.

namespace Abc.Utilities
{
    using System.Collections;
    using System.Collections.Generic;

    using AoorException = System.ArgumentOutOfRangeException;
    using EF = ExceptionFactory;

    // REVIEW: optimize.

    /// <summary>
    /// Provides static helpers to produce new sequences.
    /// <para>This class cannot be inherited.</para>
    /// </summary>
    internal static class Sequence
    {
        /// <summary>
        /// Generates a sequence that contains exactly one element.
        /// </summary>
        public static IEnumerable<T> Return<T>(T element)
        {
            // We could write:
            //   return Enumerable.Repeat(element, 1);
            // but then LINQ operators can optimize operations for lists which
            // does not seem to be the type produced by Repeat().
            return new ReturnList_<T>(element);
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

        private sealed class ReturnList_<T> : IList<T>, IReadOnlyList<T>
        {
            readonly T _value;

            public ReturnList_(T value)
            {
                _value = value;
            }

            public int Count => 1;

            public bool IsReadOnly => true;

            public T this[int index]
            {
                get => index == 0 ? _value : throw new AoorException(nameof(index));
                set => throw EF.ReadOnlyCollection;
            }

            public bool Contains(T item)
                => EqualityComparer<T>.Default.Equals(item, _value);

            public void CopyTo(T[] array, int arrayIndex)
                => array[arrayIndex] = _value;

            public int IndexOf(T item)
                => EqualityComparer<T>.Default.Equals(item, _value) ? 0 : -1;

            public void Add(T item) => throw EF.ReadOnlyCollection;
            public void Clear() => throw EF.ReadOnlyCollection;
            public void Insert(int index, T item) => throw EF.ReadOnlyCollection;
            public bool Remove(T item) => throw EF.ReadOnlyCollection;
            public void RemoveAt(int index) => throw EF.ReadOnlyCollection;

            public IEnumerator<T> GetEnumerator()
            {
                yield return _value;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
