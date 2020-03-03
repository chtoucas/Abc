// See LICENSE.txt in the project root for license information.

namespace Abc.Fx
{
    using System;
    using System.Diagnostics.Contracts;

    // Monad
    // =====
    //
    // References:
    // - http://hackage.haskell.org/package/base-4.12.0.0/docs/Control-Monad.html
    // - https://downloads.haskell.org/~ghc/latest/docs/html/libraries/base-4.13.0.0/Control-Monad.html
    //
    // Methods
    // -------
    // Bare minimum:
    // - >>=        obj.Bind()
    // - >>
    // - return
    // - fail
    //
    // Standard API:
    public partial class Mayhap
    {
    }

    // ZipWith
    //   Promote a function to a monad, scanning the monadic arguments from
    //   left to right.
    public partial class Mayhap
    {
        /// <summary>liftM2</summary>
        // [Monad]
        //   liftM2 :: Monad m => (a1 -> a2 -> r) -> m a1 -> m a2 -> m r
        //   liftM2 f m1 m2 = do { x1 <- m1; x2 <- m2; return (f x1 x2) }
        //
        //   Promote a function to a monad, scanning the monadic arguments from
        //   left to right.
        [Pure]
        public static Mayhap<TResult> ZipWith<TSource, TOther, TResult>(
            this Mayhap<TSource> @this,
            Mayhap<TOther> other,
            Func<TSource, TOther, TResult> zipper)
        {
            Require.NotNull(zipper, nameof(zipper));

            return @this.Bind(
                x => other.Bind(
                    y => Mayhap<TResult>.η(zipper(x, y))));
        }

        /// <summary>liftM3</summary>
        // [Monad]
        //   liftM3 :: (Monad m) => (a1 -> a2 -> a3 -> r) -> m a1 -> m a2 -> m a3 -> m r
        //   liftM3 f m1 m2 m3 = do { x1 <- m1; x2 <- m2; x3 <- m3; return (f x1 x2 x3) }
        //
        //   Promote a function to a monad, scanning the monadic arguments from
        //   left to right.
        [Pure]
        public static Mayhap<TResult> ZipWith<TSource, T1, T2, TResult>(
            this Mayhap<TSource> @this,
            Mayhap<T1> m1,
            Mayhap<T2> m2,
            Func<TSource, T1, T2, TResult> zipper)
        {
            Require.NotNull(zipper, nameof(zipper));

            return @this.Bind(
                x => m1.ZipWith(m2, (y, z) => zipper(x, y, z)));
        }

        /// <summary>liftM4</summary>
        // [Monad]
        //   liftM4 :: (Monad m) => (a1 -> a2 -> a3 -> a4 -> r) -> m a1 -> m a2 -> m a3 -> m a4 -> m r
        //   liftM4 f m1 m2 m3 m4 = do { x1 <- m1; x2 <- m2; x3 <- m3; x4 <- m4; return (f x1 x2 x3 x4) }
        [Pure]
        public static Func<Mayhap<T1>, Mayhap<T2>, Mayhap<T3>, Mayhap<T4>, Mayhap<TResult>>
            Lift<T1, T2, T3, T4, TResult>(
            Func<T1, T2, T3, T4, TResult> func)
        {
            return (m1, m2, m3, m4) =>
                m1.Bind(
                    x1 => m2.Bind(
                        x2 => m3.Bind(
                            x3 => m4.Bind(
                                x4 => Mayhap<TResult>.η(func(x1, x2, x3, x4))))));
        }

        [Pure]
        public static Mayhap<TResult> ZipWith<TSource, T1, T2, T3, TResult>(
            this Mayhap<TSource> @this,
             Mayhap<T1> first,
             Mayhap<T2> second,
             Mayhap<T3> third,
             Func<TSource, T1, T2, T3, TResult> zipper)
        {
            Require.NotNull(zipper, nameof(zipper));

            return @this.Bind(
                x => first.ZipWith(
                    second,
                    third,
                    (y, z, a) => zipper(x, y, z, a)));
        }

        /// <summary>liftM5</summary>
        // [Monad]
        //   liftM5 :: (Monad m) => (a1 -> a2 -> a3 -> a4 -> a5 -> r) -> m a1 -> m a2 -> m a3 -> m a4 -> m a5 -> m r
        //   liftM5 f m1 m2 m3 m4 m5 = do { x1 <- m1; x2 <- m2; x3 <- m3; x4 <- m4; x5 <- m5; return (f x1 x2 x3 x4 x5) }
        [Pure]
        public static Func<Mayhap<T1>, Mayhap<T2>, Mayhap<T3>, Mayhap<T4>, Mayhap<T5>, Mayhap<TResult>>
            Lift<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func)
        {
            return (m1, m2, m3, m4, m5) =>
                m1.Bind(
                    x1 => m2.Bind(
                        x2 => m3.Bind(
                            x3 => m4.Bind(
                                x4 => m5.Bind(
                                    x5 => Mayhap<TResult>.η(func(x1, x2, x3, x4, x5)))))));
        }
    }
}
