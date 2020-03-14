// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Anexn = System.ArgumentNullException;

    // REVIEW: Maybe extensions & helpers; see also MaybeEx in "play".
    // - playing with the modifier "in". Currently only added to ext methods for
    //   Maybe<T> where T is a struct.
    // - Maybe<IEnumerable>; see CollectAny().

    /// <summary>
    /// Provides static helpers and extension methods for <see cref="Maybe{T}"/>.
    /// <para>This class cannot be inherited.</para>
    /// </summary>
    public static partial class Maybe { }

    // Core methods.
    public partial class Maybe
    {
        /// <summary>
        /// Represents the unit for the type <see cref="Maybe{T}"/>.
        /// <para>This field is read-only.</para>
        /// </summary>
        public static readonly Maybe<Unit> Unit = new Maybe<Unit>(Abc.Unit.Default);

        /// <summary>
        /// Represents the zero for <see cref="Maybe{T}.Bind"/>.
        /// <para>This field is read-only.</para>
        /// </summary>
        public static readonly Maybe<Unit> Zero = Maybe<Unit>.None;

        /// <summary>
        /// Creates a new instance of the <see cref="Maybe{T}"/> struct from the
        /// specified nullable value.
        /// <para>DO NOT USE, only here to prevent the creation of maybe's for
        /// a nullable value type; see <see cref="SomeOrNone{T}(T?)"/> for a
        /// better alternative.</para>
        /// </summary>
        [Pure]
        // Not actually obsolete, but clearly states that we shouldn't use it.
        [Obsolete("Use SomeOrNone() instead.")]
        public static Maybe<T> Of<T>(T? value) where T : struct
            => value.HasValue ? new Maybe<T>(value.Value) : Maybe<T>.None;

        /// <summary>
        /// Creates a new instance of the <see cref="Maybe{T}"/> struct from the
        /// specified nullable value.
        /// <para>For concrete types, <see cref="Some"/>,
        /// <see cref="SomeOrNone{T}(T?)"/> or <see cref="SomeOrNone{T}(T)"/>
        /// should be used instead.</para>
        /// </summary>
        // Unconstrained version Of SomeOrNone() and Some().
        // F# Workflow: return.
        [Pure]
        public static Maybe<T> Of<T>([AllowNull] T value)
            => value is null ? Maybe<T>.None : new Maybe<T>(value);

        /// <summary>
        /// Removes one level of structure, projecting the bound value into the
        /// outer level.
        /// </summary>
        /// <seealso cref="Maybe{T}.Duplicate"/>
        [Pure]
        public static Maybe<T> Flatten<T>(this in Maybe<Maybe<T>> @this)
            => @this.IsSome ? @this.Value : Maybe<T>.None;

    }

    // Factory methods.
    public partial class Maybe
    {
        /// <summary>
        /// Obtains an instance of the empty <see cref="Maybe{T}" />.
        /// </summary>
        /// <remarks>
        /// To obtain the empty maybe for an unconstrained type, use
        /// <see cref="Maybe{T}.None"/> instead.
        /// </remarks>
        public static Maybe<T> None<T>() where T : notnull
            => Maybe<T>.None;

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
        /// <remarks>
        /// To create a maybe for an unconstrained type, please see
        /// <see cref="Of{T}(T)"/>.
        /// </remarks>
        [Pure]
        public static Maybe<T> SomeOrNone<T>(T? value) where T : struct
            // DO NOT REMOVE THIS METHOD.
            // Prevents the creation of a Maybe<T?> **directly**.
            => value.HasValue ? new Maybe<T>(value.Value) : Maybe<T>.None;

        /// <summary>
        /// Creates a new instance of the <see cref="Maybe{T}"/> struct from the
        /// specified nullable value.
        /// </summary>
        /// <remarks>
        /// To create a maybe for an unconstrained type, please see
        /// <see cref="Of{T}(T)"/>.
        /// </remarks>
        [Pure]
        public static Maybe<T> SomeOrNone<T>(T? value) where T : class
            => value is null ? Maybe<T>.None : new Maybe<T>(value);
    }

    // Misc methods.
    public partial class Maybe
    {
        [Pure]
        public static Maybe<Unit> Guard(bool condition)
            => condition ? Unit : Zero;
    }

    // Extension methods for Maybe<T> where T is a struct.
    public partial class Maybe
    {
        // Conversion from Maybe<T?> to  Maybe<T>.
        // REVIEW: for ref types, Maybe<T?> is compiled to Maybe<T>, but in VS
        // or .NET Core?
        [Pure]
        public static Maybe<T> Squash<T>(this in Maybe<T?> @this) where T : struct
            // NULL_FORGIVING: when IsSome is true, Value.HasValue is also true,
            // therefore we can safely access Value.Value.
            => @this.IsSome ? new Maybe<T>(@this.Value!.Value) : Maybe<T>.None;

        // Conversion from Maybe<T?> to T?.
        [Pure]
        public static T? ToNullable<T>(this in Maybe<T?> @this) where T : struct
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
        public static T? ToNullable<T>(this in Maybe<T> @this) where T : struct
            => @this.IsSome ? @this.Value : (T?)null;
    }

    // Extension methods for Maybe<T> where T is enumerable.
    public partial class Maybe
    {
        [Pure]
        public static Maybe<IEnumerable<T>> EmptyEnumerable<T>()
            => MaybeEnumerable_<T>.Empty;

        private static class MaybeEnumerable_<T>
        {
            internal static readonly Maybe<IEnumerable<T>> Empty
                = Of(Enumerable.Empty<T>());
        }
    }

    // LINQ extensions for IEnumerable<Maybe<T>> but not extension methods since
    // they are not for IEnumerable<T>, also to avoid conflicts w/ the standard
    // LINQ ops.
    public partial class Maybe
    {
        // Filtering: CollectAny (deferred).
        // Behaviour:
        // - If the input sequence is empty
        //     or all maybe's in the input sequence are empty
        //   returns an empty sequence.
        // - Otherwise,
        //   returns the sequence of values.
        [Pure]
        public static IEnumerable<T> CollectAny<T>(IEnumerable<Maybe<T>> source)
        {
            return from x in source where x.IsSome select x.Value;
        }
    }

    // Extension methods for Maybe<T> where T is disposable.
    public partial class Maybe
    {
        // Bind() with automatic resource cleanup.
        // F# Workflow: use.
        [Pure]
        public static Maybe<TResult> Use<TDisposable, TResult>(
            this Maybe<TDisposable> @this,
            Func<TDisposable, Maybe<TResult>> binder)
            where TDisposable : IDisposable
        {
            if (binder is null) { throw new Anexn(nameof(binder)); }

            return @this.Bind(x => { using (x) { return binder(x); } });
        }

        // Select() with automatic resource cleanup.
        [Pure]
        public static Maybe<TResult> Use<TDisposable, TResult>(
            this Maybe<TDisposable> @this,
            Func<TDisposable, TResult> selector)
            where TDisposable : IDisposable
        {
            if (selector is null) { throw new Anexn(nameof(selector)); }

            return @this.Select(x => { using (x) { return selector(x); } });
        }
    }

    // Extension methods for functions in the Kleisli category.
    public partial class Maybe
    {
        /// <seealso cref="Maybe{T}.Bind"/>
        [Pure]
        public static Maybe<TResult> Invoke<TSource, TResult>(
            this Func<TSource, Maybe<TResult>> @this,
            Maybe<TSource> maybe)
        {
            return maybe.Bind(@this);
        }

        [Pure]
        public static Func<TSource, Maybe<TResult>> Compose<TSource, TMiddle, TResult>(
            this Func<TSource, Maybe<TMiddle>> @this,
            Func<TMiddle, Maybe<TResult>> other)
        {
            if (@this is null) { throw new Anexn(nameof(@this)); }

            return x => @this(x).Bind(other);
        }

        [Pure]
        public static Func<TSource, Maybe<TResult>> ComposeBack<TSource, TMiddle, TResult>(
            this Func<TMiddle, Maybe<TResult>> @this,
            Func<TSource, Maybe<TMiddle>> other)
        {
            if (other is null) { throw new Anexn(nameof(other)); }

            return x => other(x).Bind(@this);
        }
    }

    // Lift, promote functions to maybe's.
    public partial class Maybe
    {
        /// <seealso cref="Maybe{T}.Select"/>
        [Pure]
        public static Maybe<TResult> Lift<TSource, TResult>(
            this Func<TSource, TResult> @this,
            Maybe<TSource> maybe)
        {
            return maybe.Select(@this);
        }

        /// <seealso cref="Maybe{T}.ZipWith"/>
        [Pure]
        public static Maybe<TResult> Lift<T1, T2, TResult>(
            this Func<T1, T2, TResult> @this,
            Maybe<T1> first,
            Maybe<T2> second)
        {
            if (@this is null) { throw new Anexn(nameof(@this)); }

            return first.IsSome && second.IsSome
                ? Of(@this(first.Value, second.Value))
                : Maybe<TResult>.None;
        }

        [Pure]
        public static Maybe<TResult> Lift<T1, T2, T3, TResult>(
            this Func<T1, T2, T3, TResult> @this,
            Maybe<T1> first,
            Maybe<T2> second,
            Maybe<T3> third)
        {
            if (@this is null) { throw new Anexn(nameof(@this)); }

            return first.IsSome && second.IsSome && third.IsSome
                ? Of(@this(first.Value, second.Value, third.Value))
                : Maybe<TResult>.None;
        }

        [Pure]
        public static Maybe<TResult> Lift<T1, T2, T3, T4, TResult>(
            this Func<T1, T2, T3, T4, TResult> @this,
            Maybe<T1> first,
            Maybe<T2> second,
            Maybe<T3> third,
            Maybe<T4> fourth)
        {
            if (@this is null) { throw new Anexn(nameof(@this)); }

            return first.IsSome && second.IsSome && third.IsSome && fourth.IsSome
                ? Of(@this(first.Value, second.Value, third.Value, fourth.Value))
                : Maybe<TResult>.None;
        }

        [Pure]
        public static Maybe<TResult> Lift<T1, T2, T3, T4, T5, TResult>(
            this Func<T1, T2, T3, T4, T5, TResult> @this,
            Maybe<T1> first,
            Maybe<T2> second,
            Maybe<T3> third,
            Maybe<T4> fourth,
            Maybe<T5> fifth)
        {
            if (@this is null) { throw new Anexn(nameof(@this)); }

            return first.IsSome && second.IsSome && third.IsSome && fourth.IsSome && fifth.IsSome
                ? Of(@this(first.Value, second.Value, third.Value, fourth.Value, fifth.Value))
                : Maybe<TResult>.None;
        }
    }

    // Extension methods for Maybe<T> where T is a function.
    public partial class Maybe
    {
        /// <seealso cref="Maybe{T}.Apply"/>
        [Pure]
        public static Maybe<TResult> Invoke<TSource, TResult>(
            this Maybe<Func<TSource, TResult>> @this,
            Maybe<TSource> maybe)
        {
            return maybe.Apply(@this);
        }

        [Pure]
        public static Maybe<TResult> Invoke<T1, T2, TResult>(
            this Maybe<Func<T1, T2, TResult>> @this,
            Maybe<T1> first,
            Maybe<T2> second)
        {
            return first.IsSome && second.IsSome && @this.IsSome
                ? Of(@this.Value(first.Value, second.Value))
                : Maybe<TResult>.None;
        }

        [Pure]
        public static Maybe<TResult> Invoke<T1, T2, T3, TResult>(
            this Maybe<Func<T1, T2, T3, TResult>> @this,
            Maybe<T1> first,
            Maybe<T2> second,
            Maybe<T3> third)
        {
            return first.IsSome && second.IsSome && third.IsSome && @this.IsSome
                ? Of(@this.Value(first.Value, second.Value, third.Value))
                : Maybe<TResult>.None;
        }

        [Pure]
        public static Maybe<TResult> Invoke<T1, T2, T3, T4, TResult>(
            this Maybe<Func<T1, T2, T3, T4, TResult>> @this,
            Maybe<T1> first,
            Maybe<T2> second,
            Maybe<T3> third,
            Maybe<T4> fourth)
        {
            return first.IsSome && second.IsSome && third.IsSome && fourth.IsSome && @this.IsSome
                ? Of(@this.Value(first.Value, second.Value, third.Value, fourth.Value))
                : Maybe<TResult>.None;
        }

        [Pure]
        public static Maybe<TResult> Invoke<T1, T2, T3, T4, T5, TResult>(
            this Maybe<Func<T1, T2, T3, T4, T5, TResult>> @this,
            Maybe<T1> first,
            Maybe<T2> second,
            Maybe<T3> third,
            Maybe<T4> fourth,
            Maybe<T5> fifth)
        {
            return first.IsSome && second.IsSome && third.IsSome && fourth.IsSome && fifth.IsSome && @this.IsSome
                ? Of(@this.Value(first.Value, second.Value, third.Value, fourth.Value, fifth.Value))
                : Maybe<TResult>.None;
        }
    }
}
