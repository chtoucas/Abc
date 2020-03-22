// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Runtime.CompilerServices;

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
        [Pure]
        // Code size = 6 bytes.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> None<T>() where T : notnull
            => Maybe<T>.None;

        /// <summary>
        /// Creates a new instance of the <see cref="Maybe{T}"/> struct from the
        /// specified value.
        /// </summary>
        [Pure]
        // Code size = 7 bytes.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<T> Some<T>(T value) where T : struct
            => new Maybe<T>(value);

        /// <summary>
        /// Creates a new instance of the <see cref="Maybe{T}"/> struct from the
        /// specified nullable value.
        /// </summary>
        /// <remarks>
        /// To create a maybe for an unconstrained type, use
        /// <see cref="Of{T}(T)"/> instead.
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
        /// To create a maybe for an unconstrained type, use
        /// <see cref="Of{T}(T)"/> instead.
        /// </remarks>
        [Pure]
        public static Maybe<T> SomeOrNone<T>(T? value) where T : class
            => value is null ? Maybe<T>.None : new Maybe<T>(value);

        [Pure]
        // Code size = 12 bytes.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<Maybe<T>> Square<T>(T value) where T : struct
            => new Maybe<Maybe<T>>(value);

        /// <remarks>
        /// To create a "square" for an unconstrained type T, use
        /// <c>Some(Of(value))</c> instead.
        /// </remarks>
        [Pure]
        public static Maybe<Maybe<T>> SquareOrNone<T>(T? value) where T : struct
            => value.HasValue ? new Maybe<Maybe<T>>(value.Value) : Maybe<Maybe<T>>.None;

        /// <remarks>
        /// To create a "square" for an unconstrained type T, use
        /// <c>Some(Of(value))</c> instead.
        /// </remarks>
        [Pure]
        public static Maybe<Maybe<T>> SquareOrNone<T>(T? value) where T : class
            => value is null ? Maybe<Maybe<T>>.None : new Maybe<Maybe<T>>(value);
    }

    // Helpers for Maybe<T> where T is a struct.
    public partial class Maybe
    {
        // Conversion from Maybe<T?> to  Maybe<T>.
        // REVIEW: for ref types, Maybe<T?> is compiled to Maybe<T>, but in VS
        // or .NET Core?
        [Pure]
        public static Maybe<T> Squash<T>(this in Maybe<T?> @this) where T : struct
            // NULL_FORGIVING: when IsSome is true, Value.HasValue is also true,
            // therefore we can safely access Value.Value.
            => @this.IsSome ? Some(@this.Value!.Value) : Maybe<T>.None;

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

    // Helpers for Maybe<Unit>.
    public partial class Maybe
    {
        /// <summary>
        /// Represents the unit for the type <see cref="Maybe{Unit}"/>.
        /// <para>This field is read-only.</para>
        /// </summary>
        public static readonly Maybe<Unit> Unit = Some(default(Unit));

        /// <summary>
        /// Represents the zero for <see cref="Maybe{Unit}.Bind"/>.
        /// <para>This field is read-only.</para>
        /// </summary>
        public static readonly Maybe<Unit> Zero = Maybe<Unit>.None;

        [Pure]
        public static Maybe<Unit> Guard(bool condition)
            => condition ? Unit : Zero;
    }

    // Helpers for Maybe<bool>.
    // 3VL (three-valued logic) logical operations.
    // It makes Maybe<bool>.None and SQL NULL very similar but only for boolean
    // logical operations, otherwise they may exhibit different behaviours.
    // For instance, with SQL-92 (NULL = NULL) evaluates to false, whereas
    // (Maybe<bool>.None == Maybe<bool>.None) evaluates to true.
    public partial class Maybe
    {
        /// <summary>
        /// Represents the unknown for the type <see cref="Maybe{Boolean}"/>.
        /// <para>This field is read-only.</para>
        /// </summary>
        public static readonly Maybe<bool> Unknown = Maybe<bool>.None;

        /// <summary>
        /// Represents the true for the type <see cref="Maybe{Boolean}"/>.
        /// <para>This field is read-only.</para>
        /// </summary>
        public static readonly Maybe<bool> True = Some(true);

        /// <summary>
        /// Represents the false for the type <see cref="Maybe{Boolean}"/>.
        /// <para>This field is read-only.</para>
        /// </summary>
        public static readonly Maybe<bool> False = Some(false);

        public static Maybe<bool> Negate(this in Maybe<bool> @this)
        {
            return @this.IsSome ? Some(!@this.Value) : Unknown;
        }

        // Compare to OrElse().
        public static Maybe<bool> Or(this in Maybe<bool> @this, Maybe<bool> other)
        {
            // true  || _     = true
            // false || true  = true
            //       || false = false
            //       || None  = None
            // None  || true  = true
            //       || false = None
            //       || None  = None

            // If one of the two values is "true", return True.
            return (@this.IsSome && @this.Value) || (other.IsSome && other.Value) ? True
                : @this.IsSome && other.IsSome ? False
                : Unknown;
        }

        // Compare to AndElse().
        public static Maybe<bool> And(this in Maybe<bool> @this, Maybe<bool> other)
        {
            // true  && true  = true
            //       && false = false
            //       && None  = None
            // false && _     = false
            // None  && true  = None
            //       && false = false
            //       && None  = None

            // If one of the two values is "false", return False.
            return (@this.IsSome && !@this.Value) || (other.IsSome && !other.Value) ? False
                : @this.IsSome && other.IsSome ? True
                : Unknown;
        }
    }

    // Helpers for Maybe<IEnumerable<T>>.
    public partial class Maybe
    {
        // Obtains the maybe of the empty sequence.
        // Beware, this is not the same as the empty maybe of type
        // IEnumerable<T>.
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

    // Helpers for Maybe<T> where T is disposable.
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

    // Helpers for functions in the Kleisli category.
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
    // Very much like lifted operators for nullables.
    // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/nullable-value-types#lifted-operators
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

            // NULL_FORGIVING: when IsSome is true, Value is NOT null.
            return first.IsSome && second.IsSome
                ? Of(@this(first.Value!, second.Value!))
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

            // NULL_FORGIVING: when IsSome is true, Value is NOT null.
            return first.IsSome && second.IsSome && third.IsSome
                ? Of(@this(first.Value!, second.Value!, third.Value!))
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

            // NULL_FORGIVING: when IsSome is true, Value is NOT null.
            return first.IsSome && second.IsSome && third.IsSome && fourth.IsSome
                ? Of(@this(first.Value!, second.Value!, third.Value!, fourth.Value!))
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

            // NULL_FORGIVING: when IsSome is true, Value is NOT null.
            return first.IsSome && second.IsSome && third.IsSome && fourth.IsSome && fifth.IsSome
                ? Of(@this(first.Value!, second.Value!, third.Value!, fourth.Value!, fifth.Value!))
                : Maybe<TResult>.None;
        }
    }

    // Helpers for Maybe<T> where T is a function.
    public partial class Maybe
    {
        #region Invoke()

        [Pure]
        public static Maybe<TResult> Invoke<TSource, TResult>(
            this Maybe<Func<TSource, TResult>> @this,
            TSource value)
        {
            // return @this.Select(f => f(value));
            // NULL_FORGIVING: when IsSome is true, Value is NOT null.
            return @this.IsSome ? Of(@this.Value!(value)) : Maybe<TResult>.None;
        }

        [Pure]
        public static Maybe<TResult> Invoke<T1, T2, TResult>(
            this Maybe<Func<T1, T2, TResult>> @this,
            T1 first,
            T2 second)
        {
            // NULL_FORGIVING: when IsSome is true, Value is NOT null.
            return @this.IsSome ? Of(@this.Value!(first, second)) : Maybe<TResult>.None;
        }

        [Pure]
        public static Maybe<TResult> Invoke<T1, T2, T3, TResult>(
            this Maybe<Func<T1, T2, T3, TResult>> @this,
            T1 first,
            T2 second,
            T3 third)
        {
            // NULL_FORGIVING: when IsSome is true, Value is NOT null.
            return @this.IsSome ? Of(@this.Value!(first, second, third))
                : Maybe<TResult>.None;
        }

        [Pure]
        public static Maybe<TResult> Invoke<T1, T2, T3, T4, TResult>(
            this Maybe<Func<T1, T2, T3, T4, TResult>> @this,
            T1 first,
            T2 second,
            T3 third,
            T4 fourth)
        {
            // NULL_FORGIVING: when IsSome is true, Value is NOT null.
            return @this.IsSome ? Of(@this.Value!(first, second, third, fourth))
                : Maybe<TResult>.None;
        }

        [Pure]
        public static Maybe<TResult> Invoke<T1, T2, T3, T4, T5, TResult>(
            this Maybe<Func<T1, T2, T3, T4, T5, TResult>> @this,
            T1 first,
            T2 second,
            T3 third,
            T4 fourth,
            T5 fifth)
        {
            // NULL_FORGIVING: when IsSome is true, Value is NOT null.
            return @this.IsSome ? Of(@this.Value!(first, second, third, fourth, fifth))
                : Maybe<TResult>.None;
        }

        #endregion

        #region Apply()

        [Pure]
        public static Maybe<TResult> Apply<TSource, TResult>(
            this Maybe<Func<TSource, TResult>> @this,
            Maybe<TSource> maybe)
        {
            // return @this.Bind(f => maybe.Select(f));
            // NULL_FORGIVING: when IsSome is true, Value is NOT null.
            return @this.IsSome && maybe.IsSome ? Of(@this.Value!(maybe.Value!))
                : Maybe<TResult>.None;
        }

        [Pure]
        public static Maybe<TResult> Apply<T1, T2, TResult>(
            this Maybe<Func<T1, T2, TResult>> @this,
            Maybe<T1> first,
            Maybe<T2> second)
        {
            // NULL_FORGIVING: when IsSome is true, Value is NOT null.
            return first.IsSome && second.IsSome && @this.IsSome
                ? Of(@this.Value!(first.Value!, second.Value!))
                : Maybe<TResult>.None;
        }

        [Pure]
        public static Maybe<TResult> Apply<T1, T2, T3, TResult>(
            this Maybe<Func<T1, T2, T3, TResult>> @this,
            Maybe<T1> first,
            Maybe<T2> second,
            Maybe<T3> third)
        {
            // NULL_FORGIVING: when IsSome is true, Value is NOT null.
            return first.IsSome && second.IsSome && third.IsSome && @this.IsSome
                ? Of(@this.Value!(first.Value!, second.Value!, third.Value!))
                : Maybe<TResult>.None;
        }

        [Pure]
        public static Maybe<TResult> Apply<T1, T2, T3, T4, TResult>(
            this Maybe<Func<T1, T2, T3, T4, TResult>> @this,
            Maybe<T1> first,
            Maybe<T2> second,
            Maybe<T3> third,
            Maybe<T4> fourth)
        {
            // NULL_FORGIVING: when IsSome is true, Value is NOT null.
            return first.IsSome && second.IsSome && third.IsSome && fourth.IsSome && @this.IsSome
                ? Of(@this.Value!(first.Value!, second.Value!, third.Value!, fourth.Value!))
                : Maybe<TResult>.None;
        }

        [Pure]
        public static Maybe<TResult> Apply<T1, T2, T3, T4, T5, TResult>(
            this Maybe<Func<T1, T2, T3, T4, T5, TResult>> @this,
            Maybe<T1> first,
            Maybe<T2> second,
            Maybe<T3> third,
            Maybe<T4> fourth,
            Maybe<T5> fifth)
        {
            // NULL_FORGIVING: when IsSome is true, Value is NOT null.
            return first.IsSome && second.IsSome && third.IsSome && fourth.IsSome && fifth.IsSome && @this.IsSome
                ? Of(@this.Value!(first.Value!, second.Value!, third.Value!, fourth.Value!, fifth.Value!))
                : Maybe<TResult>.None;
        }

        #endregion
    }
}
