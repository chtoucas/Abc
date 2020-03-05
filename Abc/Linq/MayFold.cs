﻿// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using Anexn = System.ArgumentNullException;

    // Aggregation: MayFold.
    public partial class Qperators
    {
        [Pure]
        public static Maybe<TAccumulate> MayFold<TSource, TAccumulate>(
            this IEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, Maybe<TAccumulate>> accumulator)
        {
            if (source is null) { throw new Anexn(nameof(source)); }
            if (accumulator is null) { throw new Anexn(nameof(accumulator)); }

            // TODO: optimiser (break the loop early).
            using var iter = source.GetEnumerator();

            var r = Maybe.Of(seed);
            while (iter.MoveNext())
            {
                r = r.Bind(x => accumulator(x, iter.Current));
            }
            return r;
        }

        [Pure]
        public static Maybe<TAccumulate> MayFold<TSource, TAccumulate>(
            this IEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, Maybe<TAccumulate>> accumulator,
            Func<Maybe<TAccumulate>, bool> predicate)
        {
            if (source is null) { throw new Anexn(nameof(source)); }
            if (accumulator is null) { throw new Anexn(nameof(accumulator)); }
            if (predicate is null) { throw new Anexn(nameof(predicate)); }

            using var iter = source.GetEnumerator();

            var r = Maybe.Of(seed);
            while (predicate(r) && iter.MoveNext())
            {
                r = r.Bind(x => accumulator(x, iter.Current));
            }
            return r;
        }
    }
}
