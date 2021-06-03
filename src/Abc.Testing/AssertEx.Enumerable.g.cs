// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

namespace Abc
{
    using System;
    using System.Collections.Generic;

    using Anexn = System.ArgumentNullException;

    public partial class AssertEx
    {
        /// <summary>
        /// Verifies that a call to <c>MoveNext</c> throws an
        /// <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <exception cref="Anexn"><paramref name="seq"/> is null.</exception>
        public static void ThrowsOnNext<T>(IEnumerable<T> seq)
        {
            if (seq is null) { throw new Anexn(nameof(seq)); }

            using var iter = seq.GetEnumerator();
            Throws<InvalidOperationException>(() => iter.MoveNext());
        }

        /// <summary>
        /// Verifies that a call to <c>MoveNext</c> throws an
        /// <see cref="InvalidOperationException"/> after exactly
        /// <paramref name="count"/> iterations.
        /// </summary>
        /// <exception cref="Anexn"><paramref name="seq"/> is null.</exception>
        public static void ThrowsAfter<T>(IEnumerable<T> seq, int count)
        {
            if (seq is null) { throw new Anexn(nameof(seq)); }

            int i = 0;
            using var iter = seq.GetEnumerator();
            while (i < count) { True(iter.MoveNext()); i++; }
            Throws<InvalidOperationException>(() => iter.MoveNext());
        }

        /// <summary>
        /// Verifies that a call to <c>MoveNext</c> sets <paramref name="called"/>
        /// to <see langword="true"/>.
        /// </summary>
        /// <exception cref="Anexn"><paramref name="seq"/> is null.</exception>
        public static void CalledOnNext<T>(IEnumerable<T> seq, ref bool called)
        {
            if (seq is null) { throw new Anexn(nameof(seq)); }

            using var iter = seq.GetEnumerator();
            True(iter.MoveNext());
            True(called);
        }

        /// <summary>
        /// Verifies that a call to <c>MoveNext</c> only sets <paramref name="called"/>
        /// to <see langword="true"/> after exactly <paramref name="count"/>
        /// iterations.
        /// </summary>
        /// <exception cref="Anexn"><paramref name="seq"/> is null.</exception>
        public static void CalledAfter<T>(IEnumerable<T> seq, int count, ref bool called)
        {
            if (seq is null) { throw new Anexn(nameof(seq)); }

            int i = 0;
            using var iter = seq.GetEnumerator();
            while (i < count)
            {
                True(iter.MoveNext());
                False(called);
                i++;
            }
            True(iter.MoveNext());
            True(called);
        }
    }
}
