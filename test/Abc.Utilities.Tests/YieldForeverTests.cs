// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

namespace Abc.Utilities
{
    using System.Collections;
    using System.Collections.Generic;

    using Xunit;

    using Assert = AssertEx;

    public static class YieldForeverTests
    {
        [Fact(DisplayName = "GetEnumerator() always returns the same iterator.")]
        public static void GetEnumerator()
        {
            // Arrange
            using var seq = new YieldForever<AnyT>(new AnyT());
            // Act
            IEnumerator<AnyT> it = seq.GetEnumerator();
            // Assert
            Assert.Same(seq, it);
        }

        [Fact(DisplayName = "GetEnumerator() (untyped) always returns the same iterator.")]
        public static void GetEnumerator_Untyped()
        {
            // Arrange
            using var seq = new YieldForever<AnyT>(new AnyT());
            // Act
            IEnumerable enumerable = seq;
            IEnumerator it = enumerable.GetEnumerator();
            // Assert
            Assert.Same(seq, it);
        }

        [Fact]
        public static void Current()
        {
            // Arrange
            var value = new AnyT();
            using var seq = new YieldForever<AnyT>(value);

            // Act
            IEnumerator<AnyT> it = seq.GetEnumerator();

            // Assert
            // Even before the first MoveNext(), Current already returns "value".
            Assert.Same(value, it.Current);

            for (int i = 0; i < 100; i++)
            {
                Assert.True(it.MoveNext());
                Assert.Same(value, it.Current);
            }
        }

        [Fact]
        public static void Current_Untyped()
        {
            // Arrange
            var value = new AnyT();
            using var seq = new YieldForever<AnyT>(value);

            // Act
            IEnumerator it = seq.GetEnumerator();

            // Assert
            // Even before the first MoveNext(), Current already returns "value".
            Assert.Same(value, it.Current);

            for (int i = 0; i < 100; i++)
            {
                Assert.True(it.MoveNext());
                Assert.Same(value, it.Current);
            }
        }

        [Fact]
        public static void MoveNext()
        {
            // Arrange
            using var seq = new YieldForever<AnyT>(new AnyT());
            // Act
            IEnumerator it = seq.GetEnumerator();
            // Assert
            Assert.True(it.MoveNext());
        }

        [Fact]
        public static void Reset()
        {
            // Arrange
            var value = new AnyT();
            using var seq = new YieldForever<AnyT>(value);

            // Act
            IEnumerator it = seq.GetEnumerator();

            // Assert
            for (int i = 0; i < 100; i++)
            {
                Assert.True(it.MoveNext());
                Assert.Same(value, it.Current);
            }

            // Reset() does nothing.
            it.Reset();

            for (int i = 0; i < 100; i++)
            {
                Assert.True(it.MoveNext());
                Assert.Same(value, it.Current);
            }
        }

        [Fact]
        public static void Dispose()
        {
            // Arrange
            var value = new AnyT();
            using var seq = new YieldForever<AnyT>(value);

            // Act
            IEnumerator<AnyT> it = seq.GetEnumerator();

            // Assert
            for (int i = 0; i < 100; i++)
            {
                Assert.True(it.MoveNext());
                Assert.Same(value, it.Current);
            }

            // Dispose() does nothing.
            it.Dispose();

            for (int i = 0; i < 100; i++)
            {
                Assert.True(it.MoveNext());
                Assert.Same(value, it.Current);
            }
        }
    }
}
