// See LICENSE.txt in the project root for license information.

namespace Abc.Extensions
{
    using System;
    using System.Diagnostics.Contracts;

    // Extension methods for functions in the Kleisli category.
    public static partial class MaybeX
    {
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
            if (@this is null) { throw new ArgumentNullException(nameof(@this)); }

            return x => @this(x).Bind(other);
        }

        [Pure]
        public static Func<TSource, Maybe<TResult>> ComposeBack<TSource, TMiddle, TResult>(
            this Func<TMiddle, Maybe<TResult>> @this,
            Func<TSource, Maybe<TMiddle>> other)
        {
            if (other is null) { throw new ArgumentNullException(nameof(other)); }

            return x => other(x).Bind(@this);
        }
    }

    // Promote functions to Maybe's; see also Maybe<T>.ZipWith().
    public partial class MaybeX
    {
        [Pure]
        public static Maybe<TResult> Invoke<TSource, TResult>(
            this Func<TSource, TResult> @this,
            Maybe<TSource> maybe)
        {
            return maybe.Select(@this);
        }

        [Pure]
        public static Maybe<TResult> Invoke<T1, T2, TResult>(
            this Func<T1, T2, TResult> @this,
            Maybe<T1> first,
            Maybe<T2> second)
        {
            if (@this is null) { throw new ArgumentNullException(nameof(@this)); }

            return first.IsSome && second.IsSome
                ? Maybe.Of(@this(first.Value, second.Value))
                : Maybe<TResult>.None;
        }

        [Pure]
        public static Maybe<TResult> Invoke<T1, T2, T3, TResult>(
            this Func<T1, T2, T3, TResult> @this,
            Maybe<T1> first,
            Maybe<T2> second,
            Maybe<T3> third)
        {
            if (@this is null) { throw new ArgumentNullException(nameof(@this)); }

            return first.IsSome && second.IsSome && third.IsSome
                ? Maybe.Of(@this(first.Value, second.Value, third.Value))
                : Maybe<TResult>.None;
        }

        [Pure]
        public static Maybe<TResult> Invoke<T1, T2, T3, T4, TResult>(
            this Func<T1, T2, T3, T4, TResult> @this,
            Maybe<T1> first,
            Maybe<T2> second,
            Maybe<T3> third,
            Maybe<T4> fourth)
        {
            if (@this is null) { throw new ArgumentNullException(nameof(@this)); }

            return first.IsSome && second.IsSome && third.IsSome && fourth.IsSome
                ? Maybe.Of(@this(first.Value, second.Value, third.Value, fourth.Value))
                : Maybe<TResult>.None;
        }

        [Pure]
        public static Maybe<TResult> Invoke<T1, T2, T3, T4, T5, TResult>(
            this Func<T1, T2, T3, T4, T5, TResult> @this,
            Maybe<T1> first,
            Maybe<T2> second,
            Maybe<T3> third,
            Maybe<T4> fourth,
            Maybe<T5> fifth)
        {
            if (@this is null) { throw new ArgumentNullException(nameof(@this)); }

            return first.IsSome && second.IsSome && third.IsSome && fourth.IsSome && fifth.IsSome
                ? Maybe.Of(@this(first.Value, second.Value, third.Value, fourth.Value, fifth.Value))
                : Maybe<TResult>.None;
        }
    }

    // Extension methods for Maybe<T> where T is a function.
    public partial class MaybeX
    {
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
                ? Maybe.Of(@this.Value(first.Value, second.Value))
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
                ? Maybe.Of(@this.Value(first.Value, second.Value, third.Value))
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
                ? Maybe.Of(@this.Value(first.Value, second.Value, third.Value, fourth.Value))
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
                ? Maybe.Of(@this.Value(first.Value, second.Value, third.Value, fourth.Value, fifth.Value))
                : Maybe<TResult>.None;
        }
    }
}
