﻿// See LICENSE.txt in the project root for license information.

namespace Abc.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    // For IEnumerable<T?>, prefer FirstOrDefault() over FirstOrNone().
    public static partial class Qperators
    {
        /// <summary>
        /// Returns the first element of a sequence, or
        /// <see cref="Maybe{TSource}.None"/> if the sequence contains no elements.
        /// </summary>
        public static Maybe<TSource> FirstOrNone<TSource>(this IEnumerable<TSource> source)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }

            // Fast track.
            if (source is IList<TSource> list)
            {
                return list.Count > 0 ? Maybe.Of(list[0]) : Maybe<TSource>.None;
            }

            // Slow track.
            using var iter = source.GetEnumerator();

            return iter.MoveNext() ? Maybe.Of(iter.Current) : Maybe<TSource>.None;
        }

        /// <summary>
        /// Returns the first element of a sequence that satisfies the
        /// <paramref name="predicate"/>, or <see cref="Maybe{TSource}.None"/>
        /// if no such element is found.
        /// </summary>
        public static Maybe<TSource> FirstOrNone<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            if (predicate is null) { throw new ArgumentNullException(nameof(predicate)); }

            var seq = source.Where(predicate);

            using var iter = seq.GetEnumerator();

            return iter.MoveNext() ? Maybe.Of(iter.Current) : Maybe<TSource>.None;
        }
    }
}
