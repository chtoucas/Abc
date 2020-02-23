// See LICENSE.txt in the project root for license information.

namespace Abc.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ANException = System.ArgumentNullException;
    using EF = Abc.Utitilies.ExceptionFactory;

    // REVIEW: Maybe<IEnumerable>, MaySum.

    /// <summary>
    /// Provides a set of extension methods for querying objects that implement
    /// <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static partial class MaybeL { }

    // Generation: RepeatAny.
    public partial class MaybeL
    {
        // Maybe<IEnumerable<TSource>>?
        public static IEnumerable<TSource> RepeatAny<TSource>(Maybe<TSource> value, int count)
        {
#if MONADS_PURE
            return value.Select(x => Enumerable.Repeat(x, count)).ValueOrEmpty();
#else
            return value.IsSome ? Enumerable.Repeat(value.Value, count)
                : Enumerable.Empty<TSource>();
#endif
        }
    }

    // Projection: SelectAny (deferred).
    public partial class MaybeL
    {
        public static IEnumerable<TResult> SelectAny<TSource, TResult>(
            this IEnumerable<TSource> source, Func<TSource, Maybe<TResult>> selector)
        {
            if (source is null) { throw new ANException(nameof(source)); }
            if (selector is null) { throw new ANException(nameof(selector)); }

            return __impl();

#if MONADS_PURE
            IEnumerable<TResult> __impl()
            {
                return CollectAny(source.Select(selector));
            }
#else
            IEnumerable<TResult> __impl()
            {
                foreach (var item in source)
                {
                    var result = selector(item);

                    if (result.IsSome) { yield return result.Value; }
                }
            }
#endif
        }
    }

    // Filtering: WhereAny (deferred).
    public partial class MaybeL
    {
        // Maybe<IEnumerable<TSource>>?
        public static IEnumerable<TSource> WhereAny<TSource>(
            this IEnumerable<TSource> source, Func<TSource, Maybe<bool>> predicate)
        {
            if (source is null) { throw new ANException(nameof(source)); }
            if (predicate is null) { throw new ANException(nameof(predicate)); }

            return __impl();

#if MONADS_PURE
            IEnumerable<TSource> __impl()
            {
                var seed = Maybe.Empty<TSource>();

                return source.Aggregate(
                    seed,
                    (x, y) => predicate(y).ZipWith(x, __zipper(y)))
                    .ValueOrEmpty();
            }

            Func<bool, IEnumerable<TSource>, IEnumerable<TSource>> __zipper(TSource item)
                => (b, seq) => b ? seq.Append(item) : seq;
#else
            IEnumerable<TSource> __impl()
            {
                foreach (var item in source)
                {
                    var result = predicate(item);

                    if (result.IsSome && result.Value) { yield return item; }
                }
            }
#endif
        }
    }

    // Aggregation: Fold, Reduce.
    public partial class MaybeL
    {
        public static Maybe<TAccumulate> MayFold<TSource, TAccumulate>(
            this IEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, Maybe<TAccumulate>> accumulator)
        {
            if (source is null) { throw new ANException(nameof(source)); }
            if (accumulator is null) { throw new ANException(nameof(accumulator)); }

            return __impl();

            // TODO: optimiser (break the loop early).
            Maybe<TAccumulate> __impl()
            {
                using var iter = source.GetEnumerator();

                var r = Maybe.Of(seed);
                while (iter.MoveNext())
                {
                    r = r.Bind(x => accumulator(x, iter.Current));
                }
                return r;
            }
        }

        public static Maybe<TAccumulate> MayFold<TSource, TAccumulate>(
            this IEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, Maybe<TAccumulate>> accumulator,
            Func<Maybe<TAccumulate>, bool> predicate)
        {
            if (source is null) { throw new ANException(nameof(source)); }
            if (accumulator is null) { throw new ANException(nameof(accumulator)); }
            if (predicate is null) { throw new ANException(nameof(predicate)); }

            return __impl();

            Maybe<TAccumulate> __impl()
            {
                using var iter = source.GetEnumerator();

                var r = Maybe.Of(seed);
                while (predicate(r) && iter.MoveNext())
                {
                    r = r.Bind(x => accumulator(x, iter.Current));
                }
                return r;
            }
        }

        public static Maybe<TSource> MayReduce<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, TSource, Maybe<TSource>> accumulator)
        {
            if (source is null) { throw new ANException(nameof(source)); }
            if (accumulator is null) { throw new ANException(nameof(accumulator)); }

            return __impl();

            Maybe<TSource> __impl()
            {
                using var iter = source.GetEnumerator();

                if (!iter.MoveNext()) { throw EF.EmptySequence; }

                var r = Maybe.Of(iter.Current);
                while (iter.MoveNext())
                {
                    r = r.Bind(x => accumulator(x, iter.Current));
                }
                return r;
            }
        }

        public static Maybe<TSource> MayReduce<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, TSource, Maybe<TSource>> accumulator,
            Func<Maybe<TSource>, bool> predicate)
        {
            if (source is null) { throw new ANException(nameof(source)); }
            if (accumulator is null) { throw new ANException(nameof(accumulator)); }
            if (predicate is null) { throw new ANException(nameof(predicate)); }

            return __impl();

            Maybe<TSource> __impl()
            {
                using var iter = source.GetEnumerator();

                if (!iter.MoveNext()) { throw EF.EmptySequence; }

                var r = Maybe.Of(iter.Current);
                while (predicate(r) && iter.MoveNext())
                {
                    r = r.Bind(x => accumulator(x, iter.Current));
                }
                return r;
            }
        }
    }

    // Concatenation: ZipAny.
    public partial class MaybeL
    {
        public static IEnumerable<TResult> ZipAny<T1, T2, TResult>(
            this IEnumerable<T1> first,
            IEnumerable<T2> second,
            Func<T1, T2, Maybe<TResult>> resultSelector)
        {
            return CollectAny(first.Zip(second, resultSelector));
        }
    }

    // Operators on IEnumerable<Maybe<T>>.
    // Filtering: CollectAny (deferred).
    // Aggregation: SumAny.
    public partial class MaybeL
    {
        // Maybe<IEnumerable<TSource>>?
        public static IEnumerable<TSource> CollectAny<TSource>(
            this IEnumerable<Maybe<TSource>> source)
        {
            if (source is null) { throw new ANException(nameof(source)); }

            return __impl();

#if MONADS_PURE
            IEnumerable<TSource> __impl()
            {
                var seed = Maybe.Empty<TSource>();

                return source.Aggregate(seed, (x, y) => x.ZipWith(y, Enumerable.Append))
                    .ValueOrEmpty();
            }
#else
            IEnumerable<TSource> __impl()
            {
                foreach (var item in source)
                {
                    if (item.IsSome) { yield return item.Value; }
                }
            }
#endif
        }

        public static Maybe<TSource> SumAny<TSource>(
            this IEnumerable<Maybe<TSource>> source)
        {
            return source.Aggregate(Maybe<TSource>.None, (m, n) => m.OrElse(n));
        }
    }
}
