// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

namespace Abc.Utilities
{
    using Xunit;

    public static class PredicatesTests
    {
        [Fact]
        public static void False()
        {
            Assert.Same(Predicates.False, Predicates.False);
            Assert.NotNull(Predicates.False);
            Assert.False(Predicates.False.Invoke());
        }

        [Fact]
        public static void True()
        {
            Assert.Same(Predicates.True, Predicates.True);
            Assert.NotNull(Predicates.True);
            Assert.True(Predicates.True.Invoke());
        }

        [Fact]
        public static void FalseT()
        {
            Assert.Same(Predicates<AnyT>.False, Predicates<AnyT>.False);
            Assert.NotNull(Predicates<AnyT>.False);
            Assert.False(Predicates<AnyT>.False.Invoke(new AnyT()));
        }

        [Fact]
        public static void TrueT()
        {
            Assert.Same(Predicates<AnyT>.True, Predicates<AnyT>.True);
            Assert.NotNull(Predicates<AnyT>.True);
            Assert.True(Predicates<AnyT>.True.Invoke(new AnyT()));
        }
    }
}
