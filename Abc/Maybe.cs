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
        /// Gets the unit for the type <see cref="Maybe{T}"/>.
        /// </summary>
        public static Maybe<Unit> Unit { get; } = Of(Abc.Unit.Default);

        /// <summary>
        /// Gets the zero for <see cref="Maybe{T}.Bind"/>.
        /// </summary>
        public static Maybe<Unit> None { get; } = Maybe<Unit>.None;

        /// <summary>
        /// Creates a new instance of the <see cref="Maybe{T}"/> struct from the
        /// specified value.
        /// </summary>
        /// <param name="value">A value to be wrapped in an object of type
        /// <see cref="Maybe{T}"/>.</param>
        /// <typeparam name="T">The underlying type of <paramref name="value"/>.
        /// </typeparam>
        public static Maybe<T> Of<T>([AllowNull]T value)
            => value is null ? Maybe<T>.None : new Maybe<T>(value);

        /// <summary>
        /// Creates a new instance of the <see cref="Maybe{T}"/> struct from the
        /// specified value.
        /// </summary>
        /// <param name="value">A value to be wrapped in an object of type
        /// <see cref="Maybe{T}"/>.</param>
        /// <typeparam name="T">The underlying type of <paramref name="value"/>.
        /// </typeparam>
        public static Maybe<T> Of<T>(T? value) where T : struct
            // This method makes it impossible to create a Maybe<T?> **directly**.
            => value.HasValue ? new Maybe<T>(value.Value) : Maybe<T>.None;

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
        public static IEnumerable<T> ValueOrEmpty<T>(this Maybe<IEnumerable<T>> @this)
#if MONADS_PURE
            => @this.ValueOrElse(Enumerable.Empty<T>());
#else
            => @this.IsSome ? @this.Value : Enumerable.Empty<T>();
#endif
    }

    // Extension methods when T is a struct.
    public partial class Maybe
    {
        // Conversion from Maybe<T?> to  Maybe<T>.
        public static Maybe<T> Squash<T>(this Maybe<T?> @this) where T : struct
            // NB: When IsSome is true, Value.HasValue is also true, therefore
            // we can safely access Value.Value.
#if MONADS_PURE
            => @this.Bind(x => new Maybe<T>(x!.Value));
#else
            => @this.IsSome ? new Maybe<T>(@this.Value!.Value) : Maybe<T>.None;
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
