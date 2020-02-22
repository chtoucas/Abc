// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;

    /// <summary>
    /// Provides extension methods for functions in the Kleisli category:
    /// <see cref="Func{TSource, TResult}"/> where <c>TResult</c> is of type
    /// <see cref="Maybe{T}"/>.
    /// </summary>
    public static partial class Kleisli
    {
        public static Maybe<TResult> Invoke<TSource, TResult>(
            this Func<TSource, Maybe<TResult>> @this, Maybe<TSource> value)
        {
            return value.Bind(@this);
        }

        public static Func<TSource, Maybe<TResult>> Compose<TSource, TMiddle, TResult>(
            this Func<TSource, Maybe<TMiddle>> @this, Func<TMiddle, Maybe<TResult>> other)
        {
            if (@this is null) { throw new ArgumentNullException(nameof(@this)); }

            return x => @this(x).Bind(other);
        }

        public static Func<TSource, Maybe<TResult>> ComposeBack<TSource, TMiddle, TResult>(
            this Func<TMiddle, Maybe<TResult>> @this, Func<TSource, Maybe<TMiddle>> other)
        {
            if (other is null) { throw new ArgumentNullException(nameof(other)); }

            return x => other(x).Bind(@this);
        }
    }
}
