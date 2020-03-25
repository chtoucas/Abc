// See LICENSE.txt in the project root for license information.

namespace Abc.Utilities
{
    using System;
    using System.Collections.Generic;

    using Xunit;

    using Assert = AssertEx;

    public static class SingletonIteratorTests
    {
        private sealed class AnyT { public AnyT() { } }

        private static readonly AnyT Value;
        private static readonly IEnumerator<AnyT> AsEnumerator;
        private static readonly IEnumerable<AnyT> AsEnumerable;
        private static readonly IList<AnyT> AsList;
        //private static readonly IReadOnlyList<AnyT> AsReadOnlyList;

#pragma warning disable CA1810 // Initialize reference type static fields inline
        static SingletonIteratorTests()
#pragma warning restore CA1810
        {
            Value = new AnyT();
            var some = Maybe.SomeOrNone(Value);
            AsEnumerator = some.GetEnumerator();
            AsEnumerable = some.ToEnumerable();
            AsList = (IList<AnyT>)AsEnumerable;
            //AsReadOnlyList = (IReadOnlyList<AnyT>)AsEnumerable;
        }

        [Fact]
        public static void IsReadOnly() => Assert.True(AsList.IsReadOnly);

        [Fact]
        public static void Count() => Assert.Equal(1, AsList.Count);

        // TODO: to be improved.
        // Current is in fact constant...
        [Fact]
        public static void Current() => Assert.Equal(Value, AsEnumerator.Current);

        [Fact]
        public static void Add() => Assert.Throws<NotSupportedException>(() => AsList.Add(new AnyT()));

        [Fact]
        public static void Clear() => Assert.Throws<NotSupportedException>(() => AsList.Clear());

        [Fact]
        public static void Insert() => Assert.Throws<NotSupportedException>(() => AsList.Insert(2, new AnyT()));

        [Fact]
        public static void Remove() => Assert.Throws<NotSupportedException>(() => AsList.Remove(Value));

        [Fact]
        public static void RemoveAt() => Assert.Throws<NotSupportedException>(() => AsList.RemoveAt(1));
    }
}
