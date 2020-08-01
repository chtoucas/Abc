// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

#if NETFRAMEWORK // Enumerable.Append & Enumerable.Prepend
namespace Abc.Edu.Fx
{
    using System.Collections.Generic;

    using Abc.Utilities;

    public static class EnumerableX
    {
        public static IEnumerable<TSource> Append<TSource>(
            this IEnumerable<TSource> source,
            TSource element)
        {
            Guard.NotNull(source, nameof(source));

            return iterator();

            IEnumerable<TSource> iterator()
            {
                foreach (var item in source)
                {
                    yield return item;
                }

                yield return element;
            }
        }

        public static IEnumerable<TSource> Prepend<TSource>(
            this IEnumerable<TSource> source,
            TSource element)
        {
            Guard.NotNull(source, nameof(source));

            return iterator();

            IEnumerable<TSource> iterator()
            {
                yield return element;

                foreach (var item in source)
                {
                    yield return item;
                }
            }
        }
    }
}
#endif