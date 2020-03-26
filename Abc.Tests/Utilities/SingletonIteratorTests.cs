// See LICENSE.txt in the project root for license information.

namespace Abc.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Xunit;

    using Assert = AssertEx;

    public static partial class SingletonIteratorTests
    {
        private static readonly AnyT Value;
        private static readonly IEnumerable<AnyT> Iter;
        private static readonly IEnumerator<AnyT> AsEnumerator;

#pragma warning disable CA1810 // Initialize reference type static fields inline
        static SingletonIteratorTests()
#pragma warning restore CA1810
        {
            var anyT = AnyT.New();
            Value = anyT.Value;
            Iter = anyT.Some.ToEnumerable();
            AsEnumerator = anyT.Some.GetEnumerator();
        }
    }

    // IList<T>
    public partial class SingletonIteratorTests
    {
        [Fact]
        public static void Indexer_Get_InvalidIndex()
        {
            // Arrange
            var list = (IList<AnyT>)Iter;
            // Act & Assert
            Assert.ThrowsAoorEx("index", () => list[1]);
        }

        [Fact]
        public static void Indexer_Get()
        {
            // Arrange
            var list = (IList<AnyT>)Iter;
            // Act & Assert
            Assert.Equal(Value, list[0]);
        }

        [Fact]
        public static void Indexer_Set()
        {
            // Arrange
            var list = (IList<AnyT>)Iter;
            // Act & Assert
            Assert.Throws<NotSupportedException>(() => list[1] = AnyT.Value);
        }

        [Fact]
        public static void IndexOf_OK()
        {
            // Arrange
            var list = (IList<AnyT>)Iter;
            // Act & Assert
            Assert.Equal(0, list.IndexOf(Value));
        }

        [Fact]
        public static void IndexOf_KO()
        {
            // Arrange
            var list = (IList<AnyT>)Iter;
            // Act & Assert
            Assert.Equal(-1, list.IndexOf(AnyT.Value));
        }

        [Fact]
        public static void Insert()
        {
            // Arrange
            var list = (IList<AnyT>)Iter;
            // Act & Assert
            Assert.Throws<NotSupportedException>(() => list.Insert(1, AnyT.Value));
        }

        [Fact]
        public static void RemoveAt()
        {
            // Arrange
            var list = (IList<AnyT>)Iter;
            // Act & Assert
            Assert.Throws<NotSupportedException>(() => list.RemoveAt(0));
        }
    }

    // ICollection<T>
    public partial class SingletonIteratorTests
    {
        [Fact]
        public static void Count()
        {
            // Arrange
            var list = (IList<AnyT>)Iter;
            // Act & Assert
            Assert.Equal(1, list.Count);
        }

        [Fact]
        public static void IsReadOnly()
        {
            // Arrange
            var list = (IList<AnyT>)Iter;
            // Act & Assert
            Assert.True(list.IsReadOnly);
        }

        [Fact]
        public static void Add()
        {
            // Arrange
            var list = (IList<AnyT>)Iter;
            // Act & Assert
            Assert.Throws<NotSupportedException>(() => list.Add(AnyT.Value));
        }

        [Fact]
        public static void Clear()
        {
            // Arrange
            var list = (IList<AnyT>)Iter;
            // Act & Assert
            Assert.Throws<NotSupportedException>(() => list.Clear());
        }

        [Fact]
        public static void Contains_OK()
        {
            // Arrange
            var list = (IList<AnyT>)Iter;
            // Act & Assert
            Assert.True(list.Contains(Value));
        }

        [Fact]
        public static void Contains_KO()
        {
            // Arrange
            var list = (IList<AnyT>)Iter;
            // Act & Assert
            Assert.False(list.Contains(AnyT.Value));
        }

        [Fact]
        public static void CopyTo()
        {
            // Arrange
            var list = (IList<AnyT>)Iter;
            var arr = new AnyT[10]
            {
                AnyT.Value,
                AnyT.Value,
                AnyT.Value,
                AnyT.Value,
                AnyT.Value,
                AnyT.Value,
                AnyT.Value,
                AnyT.Value,
                AnyT.Value,
                AnyT.Value,
            };
            // Act
            list.CopyTo(arr, 4);
            // Assert
            Assert.NotSame(Value, arr[0]);
            Assert.NotSame(Value, arr[1]);
            Assert.NotSame(Value, arr[2]);
            Assert.NotSame(Value, arr[3]);
            Assert.Same(Value, arr[4]);
            Assert.NotSame(Value, arr[5]);
            Assert.NotSame(Value, arr[6]);
            Assert.NotSame(Value, arr[7]);
            Assert.NotSame(Value, arr[8]);
            Assert.NotSame(Value, arr[9]);
        }

        [Fact]
        public static void Remove()
        {
            // Arrange
            var list = (IList<AnyT>)Iter;
            // Act & Assert
            Assert.Throws<NotSupportedException>(() => list.Remove(Value));
        }
    }

    // IEnumerable<T>.
    public partial class SingletonIteratorTests
    {
        [Fact]
        public static void GetEnumerator()
        {
            // Arrange
            IEnumerator<AnyT> enumerator = Iter.GetEnumerator();
            // Act & Assert
            Assert.Same(Iter, enumerator);
        }

        [Fact]
        public static void GetEnumerator_Untyped()
        {
            // Arrange
            IEnumerator enumerator = Iter.GetEnumerator();
            // Act & Assert
            Assert.Same(Iter, enumerator);
        }

        // TODO: to be improved.
        // Current is in fact constant...
        [Fact]
        public static void Current() => Assert.Same(Value, AsEnumerator.Current);
    }
}
