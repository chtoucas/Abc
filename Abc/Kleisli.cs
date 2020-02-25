// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides extension methods for functions in the Kleisli category.
    /// </summary>
    public static partial class Kleisli
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
}
