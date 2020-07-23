// See LICENSE in the project root for license information.

namespace Abc.Utilities
{
    using System;
    using System.Collections.Generic;

    using Xunit;

    using Assert = AssertEx;

    public static partial class SingletonListTests { }

    // IList<T>
    public partial class SingletonListTests
    {
        public static readonly TheoryData<int> IndexesForNotSupportedMethod
            = new TheoryData<int>
            {
                // -1 is always invalid for a list.
                -1,
                // Only 0 is actually valid but we use this data to test not
                // supported methods.
                0, 1, 100, 1000, Int32.MaxValue
            };

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(Int32.MaxValue)]
        public static void Indexer_Get_InvalidIndex(int index)
        {
            // Arrange
            var iter = new SingletonList<AnyT>(new AnyT());
            var list = (IList<AnyT>)iter;
            // Act & Assert
            Assert.ThrowsAoorexn("index", () => list[index]);
        }

        [Fact]
        public static void Indexer_Get()
        {
            // Arrange
            var value = new AnyT();
            var iter = new SingletonList<AnyT>(value);
            var list = (IList<AnyT>)iter;
            // Act & Assert
            Assert.Equal(value, list[0]);
        }

        [Theory, MemberData(nameof(IndexesForNotSupportedMethod))]
        public static void Indexer_Set(int index)
        {
            // Arrange
            var iter = new SingletonList<AnyT>(new AnyT());
            var list = (IList<AnyT>)iter;
            // Act & Assert
            Assert.Throws<NotSupportedException>(() => list[index] = new AnyT());
        }

        [Fact]
        public static void IndexOf_OK()
        {
            // Arrange
            var value = new AnyT();
            var iter = new SingletonList<AnyT>(value);
            var list = (IList<AnyT>)iter;
            // Act & Assert
            Assert.Equal(0, list.IndexOf(value));
        }

        [Fact]
        public static void IndexOf_KO()
        {
            // Arrange
            var iter = new SingletonList<AnyT>(new AnyT());
            var list = (IList<AnyT>)iter;
            // Act & Assert
            Assert.Equal(-1, list.IndexOf(new AnyT()));
        }

        [Theory, MemberData(nameof(IndexesForNotSupportedMethod))]
        public static void Insert(int index)
        {
            // Arrange
            var iter = new SingletonList<AnyT>(new AnyT());
            var list = (IList<AnyT>)iter;
            // Act & Assert
            Assert.Throws<NotSupportedException>(() => list.Insert(index, new AnyT()));
        }

        [Theory, MemberData(nameof(IndexesForNotSupportedMethod))]
        public static void RemoveAt(int index)
        {
            // Arrange
            var iter = new SingletonList<AnyT>(new AnyT());
            var list = (IList<AnyT>)iter;
            // Act & Assert
            Assert.Throws<NotSupportedException>(() => list.RemoveAt(index));
        }
    }

    // ICollection<T>
    public partial class SingletonListTests
    {
        [Fact]
        public static void Count()
        {
            // Arrange
            var iter = new SingletonList<AnyT>(new AnyT());
            var list = (IList<AnyT>)iter;
            // Act & Assert
            Assert.Equal(1, list.Count);
        }

        [Fact]
        public static void IsReadOnly()
        {
            // Arrange
            var iter = new SingletonList<AnyT>(new AnyT());
            var list = (IList<AnyT>)iter;
            // Act & Assert
            Assert.True(list.IsReadOnly);
        }

        [Fact]
        public static void Add()
        {
            // Arrange
            var iter = new SingletonList<AnyT>(new AnyT());
            var list = (IList<AnyT>)iter;
            // Act & Assert
            Assert.Throws<NotSupportedException>(() => list.Add(new AnyT()));
        }

        [Fact]
        public static void Clear()
        {
            // Arrange
            var iter = new SingletonList<AnyT>(new AnyT());
            var list = (IList<AnyT>)iter;
            // Act & Assert
            Assert.Throws<NotSupportedException>(() => list.Clear());
        }

        [Fact]
        public static void Contains_OK()
        {
            // Arrange
            var value = new AnyT();
            var iter = new SingletonList<AnyT>(value);
            var list = (IList<AnyT>)iter;
            // Act & Assert
            Assert.True(list.Contains(value));
        }

        [Fact]
        public static void Contains_KO()
        {
            // Arrange
            var iter = new SingletonList<AnyT>(new AnyT());
            var list = (IList<AnyT>)iter;
            // Act & Assert
            Assert.False(list.Contains(new AnyT()));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        public static void CopyTo(int index)
        {
            // Arrange
            var value = new AnyT();
            var iter = new SingletonList<AnyT>(value);

            var list = (IList<AnyT>)iter;
            var arr = new AnyT[10]
            {
                new AnyT(),
                new AnyT(),
                new AnyT(),
                new AnyT(),
                new AnyT(),
                new AnyT(),
                new AnyT(),
                new AnyT(),
                new AnyT(),
                new AnyT(),
            };

            // Act
            list.CopyTo(arr, index);

            // Assert
            for (int i = 0; i < 10; i++)
            {
                if (i == index)
                {
                    Assert.Same(value, arr[index]);
                }
                else
                {
                    Assert.NotSame(value, arr[i]);
                }
            }
        }

        [Fact]
        public static void Remove()
        {
            // Arrange
            var value = new AnyT();
            var iter = new SingletonList<AnyT>(value);
            var list = (IList<AnyT>)iter;
            // Act & Assert
            Assert.Throws<NotSupportedException>(() => list.Remove(value));
        }
    }

    // IEnumerable<T>.
    public partial class SingletonListTests
    {
        [Fact(DisplayName = "GetEnumerator() returns a new iterator.")]
        public static void GetEnumerator()
        {
            // Arrange
            var iter = new SingletonList<AnyT>(new AnyT());
            // Act & Assert
            Assert.NotSame(iter.GetEnumerator(), iter.GetEnumerator());
        }

        [Fact]
        public static void Iterate()
        {
            // Arrange
            var value = new AnyT();
            var iter = new SingletonList<AnyT>(value);
            IEnumerator<AnyT> it = iter.GetEnumerator();

            // Act & Assert
            // Even before the first MoveNext(), Current already returns Value.
            Assert.Same(value, it.Current);

            Assert.True(it.MoveNext());
            Assert.Same(value, it.Current);
            Assert.False(it.MoveNext());

            it.Reset();

            Assert.True(it.MoveNext());
            Assert.Same(value, it.Current);
            Assert.False(it.MoveNext());

            // Dispose() does nothing.
            it.Dispose();
            Assert.False(it.MoveNext());

            it.Reset();

            Assert.True(it.MoveNext());
            Assert.Same(value, it.Current);
            Assert.False(it.MoveNext());
        }
    }
}
