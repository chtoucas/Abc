// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Provides static helpers and extension methods for <see cref="Maybe{T}"/>.
    /// </summary>
    public static partial class Maybe { }

    // Core monadic methods.
    public partial class Maybe
    {
        /// <summary>
        /// Represents the unit for the type <see cref="Maybe{T}"/>.
        /// <para>This field is read-only.</para>
        /// </summary>
        public static readonly Maybe<Unit> Unit = Some(Abc.Unit.Default);

        /// <summary>
        /// Represents the zero for <see cref="Maybe{T}.Bind"/>.
        /// <para>This field is read-only.</para>
        /// </summary>
        public static readonly Maybe<Unit> None = Maybe<Unit>.None;

        /// <summary>
        /// Creates a new instance of the <see cref="Maybe{T}"/> struct from the
        /// specified value.
        /// </summary>
        public static Maybe<T> Some<T>(T value) where T : struct
            => new Maybe<T>(value);

        /// <summary>
        /// Creates a new instance of the <see cref="Maybe{T}"/> struct from the
        /// specified nullable value.
        /// </summary>
        public static Maybe<T> Of<T>([AllowNull]T value)
            => value is null ? Maybe<T>.None : new Maybe<T>(value);

        /// <summary>
        /// Creates a new instance of the <see cref="Maybe{T}"/> struct from the
        /// specified nullable value.
        /// </summary>
        public static Maybe<T> Of<T>(T? value) where T : struct
            // This method makes it impossible to create a Maybe<T?> **directly**.
            => value.HasValue ? Some(value.Value) : Maybe<T>.None;

        /// <summary>
        /// Removes one level of structure, projecting the bound value into the
        /// outer level.
        /// </summary>
        public static Maybe<T> Flatten<T>(this Maybe<Maybe<T>> @this)
#if MONADS_PURE
            => Maybe<T>.μ(@this);
#else
            => @this.IsSome ? @this.Value : Maybe<T>.None;
#endif

        public static Maybe<Unit> Guard(bool predicate)
            => predicate ? Unit : None;
    }

    // Operations on IEnumerable<Maybe<T>>.
    // Filtering: CollectAny (deferred).
    // Aggregation: SumAny.
    public partial class Maybe
    {
        // Maybe<IEnumerable<TSource>>?
        public static IEnumerable<TSource> CollectAny<TSource>(
            IEnumerable<Maybe<TSource>> source)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }

            return __iterator();

#if MONADS_PURE
            IEnumerable<TSource> __iterator()
            {
                var seed = Maybe.Empty<TSource>();

                return source.Aggregate(seed, (x, y) => x.ZipWith(y, Enumerable.Append))
                    .ValueOrEmpty();
            }
#else
            IEnumerable<TSource> __iterator()
            {
                foreach (var item in source)
                {
                    if (item.IsSome) { yield return item.Value; }
                }
            }
#endif
        }

        public static Maybe<TSource> SumAny<TSource>(IEnumerable<Maybe<TSource>> source)
        {
            return source.Aggregate(Maybe<TSource>.None, (m, n) => m.OrElse(n));
        }
    }

    // Extension methods when T is a func.
    public partial class Maybe
    {
        public static Maybe<TResult> Invoke<TSource, TResult>(
            this Maybe<Func<TSource, TResult>> @this, Maybe<TSource> value)
        {
#if MONADS_PURE
            return @this.Bind(x => value.Select(x));
#else
            return value.IsSome && @this.IsSome ? Of(@this.Value(value.Value))
                : Maybe<TResult>.None;
#endif
        }
    }

    // Extension methods when T is enumerable.
    public partial class Maybe
    {
        internal static Maybe<IEnumerable<T>> Empty<T>()
            => MaybeEnumerable_<T>.Empty;

        public static IEnumerable<T> ValueOrEmpty<T>(this Maybe<IEnumerable<T>> @this)
#if MONADS_PURE
            => @this.ValueOrElse(Enumerable.Empty<T>());
#else
            => @this.IsSome ? @this.Value : Enumerable.Empty<T>();
#endif

        private static class MaybeEnumerable_<T>
        {
            internal static readonly Maybe<IEnumerable<T>> Empty
                = Of(Enumerable.Empty<T>());
        }
    }

    // Extension methods when T is a struct.
    public partial class Maybe
    {
        // Conversion from Maybe<T?> to  Maybe<T>.
        public static Maybe<T> Squash<T>(this Maybe<T?> @this) where T : struct
            // NB: When IsSome is true, Value.HasValue is also true, therefore
            // we can safely access Value.Value.
#if MONADS_PURE
            => @this.Bind(x => Some(x!.Value));
#else
            => @this.IsSome ? Some(@this.Value!.Value) : Maybe<T>.None;
#endif

        // Conversion from Maybe<T?> to T?.
        public static T? ToNullable<T>(this Maybe<T?> @this) where T : struct
#if MONADS_PURE
            => @this.ValueOrDefault();
#else
#if DEBUG
            // We have to be careful in Debug mode since the access to Value is
            // protected by a Debug.Assert.
            => @this.IsSome ? @this.Value : null;
#else
            // If the object is "none", Value is default(T?) ie null.
            => @this.Value;
#endif
#endif

        // Conversion from Maybe<T> to T?.
        public static T? ToNullable<T>(this Maybe<T> @this) where T : struct
#if MONADS_PURE
            => @this.ValueOrDefault();
#else
            => @this.IsSome ? @this.Value : (T?)null;
#endif
    }

    // Extension methods when T is disposable.
    public partial class Maybe
    {
        //// Bind() with automatic resource management.
        //public static Maybe<TResult> BindDispose<TSource, TResult>(
        //    this Maybe<TSource> @this, Func<TSource, Maybe<TResult>> binder)
        //    where TSource : IDisposable
        //{
        //    if (binder is null) { throw new ArgumentNullException(nameof(binder)); }

        //    return @this.Bind(x => { using (x) { return binder(x); } });
        //}

        //// Select() with automatic resource management.
        //public static Maybe<TResult> SelectDispose<TSource, TResult>(
        //    this Maybe<TSource> @this, Func<TSource, TResult> selector)
        //    where TSource : IDisposable
        //{
        //    if (selector is null) { throw new ArgumentNullException(nameof(selector)); }

        //    return @this.Select(x => { using (x) { return selector(x); } });
        //}
    }
}
