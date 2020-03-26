// See LICENSE.txt in the project root for license information.

namespace Abc.Utilities
{
    using System;
    using System.Collections.Generic;

    using Xunit;

    using Assert = AssertEx;

    public static class SingletonIteratorTests
    {
        private static readonly AnyT Value;
        private static readonly IEnumerator<AnyT> AsEnumerator;
        private static readonly IList<AnyT> AsList;

#pragma warning disable CA1810 // Initialize reference type static fields inline
        static SingletonIteratorTests()
#pragma warning restore CA1810
        {
            Value = AnyT.Value;
            var some = Maybe.SomeOrNone(Value);
            AsEnumerator = some.GetEnumerator();
            AsList = (IList<AnyT>)some.ToEnumerable();
        }

        [Fact]
        public static void Count() => Assert.Equal(1, AsList.Count);

        [Fact]
        public static void IsReadOnly() => Assert.True(AsList.IsReadOnly);

        [Fact]
        public static void Indexer_Set()
            => Assert.Throws<NotSupportedException>(() => AsList[1] = AnyT.Value);

        [Fact]
        public static void Indexer_Get_InvalidIndex()
            => Assert.ThrowsAoorEx("index", () => AsList[1]);

        [Fact]
        public static void Indexer_Get()
            => Assert.Equal(Value, AsList[0]);

        [Fact]
        public static void Contains_OK() => Assert.True(AsList.Contains(Value));

        [Fact]
        public static void Contains_KO() => Assert.False(AsList.Contains(AnyT.Value));

        [Fact]
        public static void IndexOf_OK() => Assert.Equal(0, AsList.IndexOf(Value));

        [Fact]
        public static void IndexOf_KO() => Assert.Equal(-1, AsList.IndexOf(AnyT.Value));

        [Fact]
        public static void Add() => Assert.Throws<NotSupportedException>(() => AsList.Add(AnyT.Value));

        [Fact]
        public static void Clear() => Assert.Throws<NotSupportedException>(() => AsList.Clear());

        [Fact]
        public static void Insert() => Assert.Throws<NotSupportedException>(() => AsList.Insert(1, AnyT.Value));

        [Fact]
        public static void Remove() => Assert.Throws<NotSupportedException>(() => AsList.Remove(Value));

        [Fact]
        public static void RemoveAt() => Assert.Throws<NotSupportedException>(() => AsList.RemoveAt(0));

        // TODO: to be improved.
        // Current is in fact constant...
        [Fact]
        public static void Current() => Assert.Same(Value, AsEnumerator.Current);

        [Fact]
        public static void GetEnumerator() => Assert.Same(AsList, AsList.GetEnumerator());
    }
}
