﻿// See LICENSE.txt in the project root for license information.

namespace Abc.Fx
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    public partial class Mayhap
    {
        public static readonly Mayhap<Unit> Unit = Some(Abc.Unit.Default);

        public static readonly Mayhap<Unit> None = Mayhap<Unit>.None;

        [Pure]
        public static Mayhap<Unit> Guard(bool predicate)
            => predicate ? Unit : None;
    }

    // Extension methods for functions in the Kleisli category.
    public partial class Mayhap
    {
        [Pure]
        public static Mayhap<TResult> Invoke<TSource, TResult>(
            this Func<TSource, Mayhap<TResult>> @this, Mayhap<TSource> value)
        {
            return value.Bind(@this);
        }

        [Pure]
        public static Func<TSource, Mayhap<TResult>> Compose<TSource, TMiddle, TResult>(
            this Func<TSource, Mayhap<TMiddle>> @this, Func<TMiddle, Mayhap<TResult>> other)
        {
            Require.NotNull(@this, nameof(@this));

            return x => @this(x).Bind(other);
        }

        [Pure]
        public static Func<TSource, Mayhap<TResult>> ComposeBack<TSource, TMiddle, TResult>(
            this Func<TMiddle, Mayhap<TResult>> @this, Func<TSource, Mayhap<TMiddle>> other)
        {
            Require.NotNull(other, nameof(other));

            return x => other(x).Bind(@this);
        }
    }

    // Extension methods for Mayhap<T> where T is a struct.
    public partial class Mayhap
    {
        [Pure]
        public static Mayhap<T> Squash<T>(this Mayhap<T?> @this) where T : struct
            // NB: When IsSome is true, Value.HasValue is also true, therefore
            // we can safely access Value.Value.
            => @this.Bind(x => Some(x!.Value));

        [Pure]
        public static T? ToNullable<T>(this Mayhap<T?> @this) where T : struct
            => @this.ValueOrDefault();

        [Pure]
        public static T? ToNullable<T>(this Mayhap<T> @this) where T : struct
            => @this.ValueOrDefault();
    }

    // Extension methods for Mayhap<T> where T is enumerable.
    // Operations on IEnumerable<Mayhap<T>>.
    // - Filtering: CollectAny (deferred).
    // - Aggregation: Any.
    public partial class Mayhap
    {
        [Pure]
        public static Mayhap<IEnumerable<T>> Empty<T>()
            => MayhapEnumerable_<T>.Empty;

        [Pure]
        public static IEnumerable<T> ValueOrEmpty<T>(this Mayhap<IEnumerable<T>> @this)
            => @this.ValueOrElse(Enumerable.Empty<T>());

        [Pure]
        public static IEnumerable<T> CollectAny<T>(IEnumerable<Mayhap<T>> source)
        {
            var seed = MayhapEnumerable_<T>.Empty;
            var seq = source.Aggregate(seed, (x, y) => x.ZipWith(y, Enumerable.Append));
            return seq.ValueOrEmpty();
        }

        [Pure]
        public static Mayhap<T> Any<T>(IEnumerable<Mayhap<T>> source)
        {
            return source.Aggregate(Mayhap<T>.None, (m, n) => m.OrElse(n));
        }

        private static class MayhapEnumerable_<T>
        {
            internal static readonly Mayhap<IEnumerable<T>> Empty
                = Of(Enumerable.Empty<T>());
        }
    }
}