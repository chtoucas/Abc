// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using System.Collections.Generic;
    using System.Linq;

    using Xunit;

    using Assert = AssertEx;

    public static partial class QperatorsTests
    {
        private static readonly IEnumerable<int> Null = null!;
        private static readonly IEnumerable<int> Empty = Enumerable.Empty<int>();
    }

    // ElementAtOrNone.
    public partial class QperatorsTests
    {
        [Fact]
        public static void ElementAtOrNone_InvalidArg()
        {
            Assert.ThrowsArgNullEx("source", () => Qperators.ElementAtOrNone(Null, 1));
        }
    }

    // FirstOrNone.
    public partial class QperatorsTests
    {
        [Fact]
        public static void FirstOrNone_InvalidArg()
        {
            Assert.ThrowsArgNullEx("source", () => Qperators.FirstOrNone(Null));

            Assert.ThrowsArgNullEx("source", () => Qperators.FirstOrNone(Null, Funk<int, bool>.Any));
            Assert.ThrowsArgNullEx("predicate", () => Qperators.FirstOrNone(Empty, Funk<int, bool>.Null));
        }
    }

    // FoldAny.
    public partial class QperatorsTests
    {
        [Fact]
        public static void FoldAny_InvalidArg()
        {
            Assert.ThrowsArgNullEx("source", () => Qperators.FoldAny(Null, 1, Kunc<int, int, int>.Any));
            Assert.ThrowsArgNullEx("accumulator", () => Qperators.FoldAny(Empty, 1, Kunc<int, int, int>.Null));

            Assert.ThrowsArgNullEx("source", () => Qperators.FoldAny(Null, 1, Kunc<int, int, int>.Any, Funk<Maybe<int>, bool>.Any));
            Assert.ThrowsArgNullEx("accumulator", () => Qperators.FoldAny(Empty, 1, Kunc<int, int, int>.Null, Funk<Maybe<int>, bool>.Any));
            Assert.ThrowsArgNullEx("predicate", () => Qperators.FoldAny(Empty, 1, Kunc<int, int, int>.Any, Funk<Maybe<int>, bool>.Null));
        }
    }

    // LastOrNone.
    public partial class QperatorsTests
    {
        [Fact]
        public static void LastOrNone_InvalidArg()
        {
            Assert.ThrowsArgNullEx("source", () => Qperators.LastOrNone(Null));

            Assert.ThrowsArgNullEx("source", () => Qperators.LastOrNone(Null, Funk<int, bool>.Any));
            Assert.ThrowsArgNullEx("predicate", () => Qperators.LastOrNone(Empty, Funk<int, bool>.Null));
        }
    }

    // ReduceAny.
    public partial class QperatorsTests
    {
        [Fact]
        public static void ReduceAny_InvalidArg()
        {
            Assert.ThrowsArgNullEx("source", () => Qperators.ReduceAny(Null, Kunc<int, int, int>.Any));
            Assert.ThrowsArgNullEx("accumulator", () => Qperators.ReduceAny(Empty, Kunc<int, int, int>.Null));

            Assert.ThrowsArgNullEx("source", () => Qperators.ReduceAny(Null, Kunc<int, int, int>.Any, Funk<Maybe<int>, bool>.Any));
            Assert.ThrowsArgNullEx("accumulator", () => Qperators.ReduceAny(Empty, Kunc<int, int, int>.Null, Funk<Maybe<int>, bool>.Any));
            Assert.ThrowsArgNullEx("predicate", () => Qperators.ReduceAny(Empty, Kunc<int, int, int>.Any, Funk<Maybe<int>, bool>.Null));
        }
    }

    // SelectAny .
    public partial class QperatorsTests
    {
        [Fact]
        public static void SelectAny_InvalidArg()
        {
            Assert.ThrowsArgNullEx("source", () => Qperators.SelectAny(Null, Kunc<int, int>.Any));
            Assert.ThrowsArgNullEx("selector", () => Qperators.SelectAny(Empty, Kunc<int, int>.Null));
        }
    }

    // SingleOrNone.
    public partial class QperatorsTests
    {
        [Fact]
        public static void SingleOrNone_InvalidArg()
        {
            Assert.ThrowsArgNullEx("source", () => Qperators.SingleOrNone(Null));

            Assert.ThrowsArgNullEx("source", () => Qperators.SingleOrNone(Null, Funk<int, bool>.Any));
            Assert.ThrowsArgNullEx("predicate", () => Qperators.SingleOrNone(Empty, Funk<int, bool>.Null));
        }
    }

    // WhereAny.
    public partial class QperatorsTests
    {
        [Fact]
        public static void WhereAny_InvalidArg()
        {
            Assert.ThrowsArgNullEx("source", () => Qperators.WhereAny(Null, Kunc<int, bool>.Any));
            Assert.ThrowsArgNullEx("predicate", () => Qperators.WhereAny(Empty, Kunc<int, bool>.Null));
        }
    }

    // ZipAny.
    public partial class QperatorsTests
    {
        [Fact]
        public static void ZipAny_InvalidArg()
        {
            Assert.ThrowsArgNullEx("first", () => Qperators.ZipAny(Null, Empty, Kunc<int, int, int>.Any));
            Assert.ThrowsArgNullEx("second", () => Qperators.ZipAny(Empty, Null, Kunc<int, int, int>.Any));
            Assert.ThrowsArgNullEx("resultSelector", () => Qperators.ZipAny(Empty, Empty, Kunc<int, int, int>.Null));
        }
    }
}
