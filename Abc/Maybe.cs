// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Provides static helpers and extension methods for <see cref="Maybe{T}"/>.
    /// <para>This class cannot be inherited.</para>
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
        [Pure]
        public static Maybe<T> Some<T>(T value) where T : struct
            => new Maybe<T>(value);

        /// <summary>
        /// Creates a new instance of the <see cref="Maybe{T}"/> struct from the
        /// specified nullable value.
        /// </summary>
        [Pure]
        public static Maybe<T> Of<T>([AllowNull]T value)
            => value is null ? Maybe<T>.None : new Maybe<T>(value);

        /// <summary>
        /// Creates a new instance of the <see cref="Maybe{T}"/> struct from the
        /// specified nullable value.
        /// </summary>
        [Pure]
        public static Maybe<T> Of<T>(T? value) where T : struct
            // This method makes it impossible to create a Maybe<T?> **directly**.
            => value.HasValue ? Some(value.Value) : Maybe<T>.None;

        /// <summary>
        /// Removes one level of structure, projecting the bound value into the
        /// outer level.
        /// </summary>
        [Pure]
        public static Maybe<T> Flatten<T>(this Maybe<Maybe<T>> @this)
            => @this.IsSome ? @this.Value : Maybe<T>.None;

        [Pure]
        public static Maybe<Unit> Guard(bool predicate)
            => predicate ? Unit : None;
    }

    // Extension methods for functions in the Kleisli category.
    public partial class Maybe
    {
        [Pure]
        public static Maybe<TResult> Invoke<TSource, TResult>(
            this Func<TSource, Maybe<TResult>> @this, Maybe<TSource> value)
        {
            return value.Bind(@this);
        }

        [Pure]
        public static Func<TSource, Maybe<TResult>> Compose<TSource, TMiddle, TResult>(
            this Func<TSource, Maybe<TMiddle>> @this, Func<TMiddle, Maybe<TResult>> other)
        {
            if (@this is null) { throw new ArgumentNullException(nameof(@this)); }

            return x => @this(x).Bind(other);
        }

        [Pure]
        public static Func<TSource, Maybe<TResult>> ComposeBack<TSource, TMiddle, TResult>(
            this Func<TMiddle, Maybe<TResult>> @this, Func<TSource, Maybe<TMiddle>> other)
        {
            if (other is null) { throw new ArgumentNullException(nameof(other)); }

            return x => other(x).Bind(@this);
        }
    }

    // Extension methods for Maybe<T> where T is a struct.
    public partial class Maybe
    {
        // Conversion from Maybe<T?> to  Maybe<T>.
        [Pure]
        public static Maybe<T> Squash<T>(this Maybe<T?> @this) where T : struct
            // NB: When IsSome is true, Value.HasValue is also true, therefore
            // we can safely access Value.Value.
            => @this.IsSome ? Some(@this.Value!.Value) : Maybe<T>.None;

        // Conversion from Maybe<T?> to T?.
        [Pure]
        public static T? ToNullable<T>(this Maybe<T?> @this) where T : struct
#if DEBUG
            // We have to be careful in Debug mode since the access to Value is
            // protected by a Debug.Assert.
            => @this.IsSome ? @this.Value : null;
#else
            // If the object is "none", Value is default(T?) ie null.
            => @this.Value;
#endif

        // Conversion from Maybe<T> to T?.
        [Pure]
        public static T? ToNullable<T>(this Maybe<T> @this) where T : struct
            => @this.IsSome ? @this.Value : (T?)null;
    }

    // Extension methods for Maybe<T> where T is enumerable.
    // Operations on IEnumerable<Maybe<T>>.
    // - Filtering: CollectAny (deferred).
    // - Aggregation: Any.
    public partial class Maybe
    {
        [Pure]
        public static Maybe<IEnumerable<T>> Empty<T>()
            => MaybeEnumerable_<T>.Empty;

        [Pure]
        public static IEnumerable<T> ValueOrEmpty<T>(this Maybe<IEnumerable<T>> @this)
            => @this.IsSome ? @this.Value : Enumerable.Empty<T>();

        // Maybe<IEnumerable<T>>?
        [Pure]
        public static IEnumerable<T> CollectAny<T>(IEnumerable<Maybe<T>> source)
        {
            // Check args eagerly.
            if (source is null) { throw new ArgumentNullException(nameof(source)); }

            return __iterator();

            IEnumerable<T> __iterator()
            {
                foreach (var item in source)
                {
                    if (item.IsSome) { yield return item.Value; }
                }
            }
        }

        [Pure]
        public static Maybe<T> Any<T>(IEnumerable<Maybe<T>> source)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }

            foreach (var item in source)
            {
                if (item.IsSome) { return item; }
            }

            return Maybe<T>.None;
        }

        private static class MaybeEnumerable_<T>
        {
            internal static readonly Maybe<IEnumerable<T>> Empty
                = Of(Enumerable.Empty<T>());
        }
    }

    // Extension methods for Maybe<T> where T is a function.
    public partial class Maybe
    {
        [Pure]
        public static Maybe<TResult> Invoke<TSource, TResult>(
            this Maybe<Func<TSource, TResult>> @this, Maybe<TSource> value)
        {
            return value.IsSome && @this.IsSome ? Of(@this.Value(value.Value))
                : Maybe<TResult>.None;
        }
    }

    // Extension methods for Maybe<T> where T is disposable.
    public partial class Maybe
    {
        //// Bind() with automatic resource management.
        //[Pure]
        //public static Maybe<TResult> BindDispose<TSource, TResult>(
        //    this Maybe<TSource> @this, Func<TSource, Maybe<TResult>> binder)
        //    where TSource : IDisposable
        //{
        //    if (binder is null) { throw new ArgumentNullException(nameof(binder)); }

        //    return @this.Bind(x => { using (x) { return binder(x); } });
        //}

        //// Select() with automatic resource management.
        //[Pure]
        //public static Maybe<TResult> SelectDispose<TSource, TResult>(
        //    this Maybe<TSource> @this, Func<TSource, TResult> selector)
        //    where TSource : IDisposable
        //{
        //    if (selector is null) { throw new ArgumentNullException(nameof(selector)); }

        //    return @this.Select(x => { using (x) { return selector(x); } });
        //}
    }
}
