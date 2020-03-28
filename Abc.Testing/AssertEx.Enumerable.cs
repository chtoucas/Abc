// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Collections.Generic;

    public partial class AssertEx
    {
        public static void ThrowsOnNext<T>(IEnumerable<T> seq)
        {
            if (IsNull(nameof(seq), seq)) { return; }

            using var iter = seq.GetEnumerator();
            Throws<InvalidOperationException>(() => iter.MoveNext());
        }

        public static void ThrowsAfter<T>(IEnumerable<T> seq, int count)
        {
            if (IsNull(nameof(seq), seq)) { return; }

            int i = 0;
            using var iter = seq.GetEnumerator();
            while (i < count) { True(iter.MoveNext()); i++; }
            Throws<InvalidOperationException>(() => iter.MoveNext());
        }

        public static void CalledOnNext<T>(IEnumerable<T> seq, ref bool notCalled)
        {
            if (IsNull(nameof(seq), seq)) { return; }

            using var iter = seq.GetEnumerator();
            iter.MoveNext();
            False(notCalled);
        }

        public static void CalledAfter<T>(IEnumerable<T> seq, int count, ref bool notCalled)
        {
            if (IsNull(nameof(seq), seq)) { return; }

            int i = 0;
            using var iter = seq.GetEnumerator();
            while (i < count) { True(iter.MoveNext()); i++; }
            iter.MoveNext();
            False(notCalled);
        }
    }
}
