// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

namespace Abc.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Xunit;

    using Assert = AssertEx;

    public static partial class SingletonListTests { }

    // IList<T>
    public partial class SingletonListTests
    {
        public static readonly TheoryData<int> NotSupportedIndexes =
            new TheoryData<int>
            {
                // -1 is always invalid for a list.
                -1,
                // Only 0 is actually valid but we use this data to test
                // unsupported methods.
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
            IList<AnyT> list = new SingletonList<AnyT>(new AnyT());
            // Act & Assert
            Assert.ThrowsAoorexn("index", () => list[index]);
        }

        [Fact]
        public static void Indexer_Get()
        {
            // Arrange
            var value = new AnyT();
            IList<AnyT> list = new SingletonList<AnyT>(value);
            // Act & Assert
            Assert.Equal(value, list[0]);
        }

        [Theory, MemberData(nameof(NotSupportedIndexes))]
        public static void Indexer_Set(int index)
        {
            // Arrange
            IList<AnyT> list = new SingletonList<AnyT>(new AnyT());
            // Act & Assert
            Assert.Throws<NotSupportedException>(() => list[index] = new AnyT());
        }

        [Fact]
        public static void IndexOf_OK()
        {
            // Arrange
            var value = new AnyT();
            IList<AnyT> list = new SingletonList<AnyT>(value);
            // Act & Assert
            Assert.Equal(0, list.IndexOf(value));
        }

        [Fact]
        public static void IndexOf_KO()
        {
            // Arrange
            IList<AnyT> list = new SingletonList<AnyT>(new AnyT());
            // Act & Assert
            Assert.Equal(-1, list.IndexOf(new AnyT()));
        }

        [Theory, MemberData(nameof(NotSupportedIndexes))]
        public static void Insert(int index)
        {
            // Arrange
            IList<AnyT> list = new SingletonList<AnyT>(new AnyT());
            // Act & Assert
            Assert.Throws<NotSupportedException>(() => list.Insert(index, new AnyT()));
        }

        [Theory, MemberData(nameof(NotSupportedIndexes))]
        public static void RemoveAt(int index)
        {
            // Arrange
            IList<AnyT> list = new SingletonList<AnyT>(new AnyT());
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
            ICollection<AnyT> list = new SingletonList<AnyT>(new AnyT());
            // Act & Assert
            Assert.Equal(1, list.Count);
        }

        [Fact]
        public static void IsReadOnly()
        {
            // Arrange
            ICollection<AnyT> list = new SingletonList<AnyT>(new AnyT());
            // Act & Assert
            Assert.True(list.IsReadOnly);
        }

        [Fact]
        public static void Add()
        {
            // Arrange
            ICollection<AnyT> list = new SingletonList<AnyT>(new AnyT());
            // Act & Assert
            Assert.Throws<NotSupportedException>(() => list.Add(new AnyT()));
        }

        [Fact]
        public static void Clear()
        {
            // Arrange
            ICollection<AnyT> list = new SingletonList<AnyT>(new AnyT());
            // Act & Assert
            Assert.Throws<NotSupportedException>(() => list.Clear());
        }

        [Fact]
        public static void Contains_OK()
        {
            // Arrange
            var value = new AnyT();
            ICollection<AnyT> list = new SingletonList<AnyT>(value);
            // Act & Assert
            Assert.True(list.Contains(value));
        }

        [Fact]
        public static void Contains_KO()
        {
            // Arrange
            ICollection<AnyT> list = new SingletonList<AnyT>(new AnyT());
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
            ICollection<AnyT> list = new SingletonList<AnyT>(value);

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
            ICollection<AnyT> list = new SingletonList<AnyT>(value);
            // Act & Assert
            Assert.Throws<NotSupportedException>(() => list.Remove(value));
        }
    }

    // IEnumerable<T>.
    public partial class SingletonListTests
    {
        [Fact(DisplayName = "GetEnumerator() returns a fresh iterator.")]
        public static void GetEnumerator()
        {
            // Arrange
            IEnumerable<AnyT> seq = new SingletonList<AnyT>(new AnyT());
            // Act & Assert
            Assert.NotSame(seq.GetEnumerator(), seq.GetEnumerator());
        }

        [Fact(DisplayName = "GetEnumerator() (untyped) returns a fresh iterator.")]
        public static void GetEnumerator_Untyped()
        {
            // Arrange
            IEnumerable seq = new SingletonList<AnyT>(new AnyT());
            // Act & Assert
            Assert.NotSame(seq.GetEnumerator(), seq.GetEnumerator());
        }

        [Fact]
        public static void Iterate()
        {
            // Arrange
            var value = new AnyT();
            IEnumerable<AnyT> seq = new SingletonList<AnyT>(value);
            IEnumerator<AnyT> it = seq.GetEnumerator();

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

        [Fact]
        public static void Iterate_Untyped()
        {
            // Arrange
            var value = new AnyT();
            IEnumerable seq = new SingletonList<AnyT>(value);
            IEnumerator it = seq.GetEnumerator();

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
        }
    }
}
