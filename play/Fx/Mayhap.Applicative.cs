// See LICENSE.txt in the project root for license information.

namespace Abc.Fx
{
    using System;
    using System.Diagnostics.Contracts;

    // Control.Applicative
    // ===================
    //
    // http://hackage.haskell.org/package/base-4.12.0.0/docs/Control-Applicative.html
    //
    // Methods
    // -------
    // Bare minimum (<*> or liftA2, here we choose <*>):
    // - pure       Mayhap.Of()
    // - <*>        Mayhap<Func>$.Invoke()
    // - liftA2     Mayhap.Lift()
    //
    // Standard API:
    // - *>         ext.ContinueWith()
    // - <*         ext.PassThru()
    // - <**>       ext.Apply()
    // - liftA      Mayhap.Lift()
    // - liftA3     Mayhap.Lift()
    //
    // Applicative rules
    // -----------------
    // Identity
    //   pure id <*> v = v
    // Composition
    //   pure (.) <*> u <*> v <*> w = u <*> (v <*> w)
    // Homomorphism
    //   pure f <*> pure x = pure (f x)
    // Interchange
    //   u <*> pure y = pure ($ y) <*> u
    public partial class Mayhap
    {
        /// <summary>(*&gt;)</summary>
        // [Applicative]
        //   (*>) :: f a -> f b -> f b
        //   a1 *> a2 = (id <$ a1) <*> a2
        //
        //   Sequence actions, discarding the value of the first argument.
        //   This is essentially the same as liftA2 (flip const), but if the
        //   Functor instance has an optimized(<$), it may be better to use
        //   that instead.Before liftA2 became a method, this definition
        //   was strictly better, but now it depends on the functor.For a
        //   functor supporting a sharing-enhancing (<$), this definition
        //   may reduce allocation by preventing a1 from ever being fully
        //   realized.In an implementation with a boring(<$) but an optimizing
        //   liftA2, it would likely be better to define(*>) using liftA2.
        [Pure]
        public static Mayhap<TResult> ContinueWith<TSource, TResult>(
            this Mayhap<TSource> @this,
            Mayhap<TResult> other)
        {
            return @this.Bind(_ => other);
        }

        /// <summary>(&lt;*)</summary>
        // [Applicative]
        //   (<*) :: f a -> f b -> f a
        //   (<*) = liftA2 const
        //
        //   Sequence actions, discarding the value of the second argument.
        [Pure]
        public static Mayhap<TSource> PassThru<TSource, TOther>(
            this Mayhap<TSource> @this,
            Mayhap<TOther> other)
        {
#if STRICT_HASKELL
            return Lift(Stubs<TSource, TOther>.Const1).Invoke(@this, other);
#else
            return @this.ZipWith(other, Stubs<TSource, TOther>.Const1);
#endif
        }

        /// <summary>(&lt;**&gt;)</summary>
        // [Applicative]
        //   (<**>) :: Applicative f => f a -> f (a -> b) -> f b
        //   (<**>) = liftA2(\a f -> f a)
        //
        //   A variant of '<*>' with the arguments reversed.
        [Pure]
        public static Mayhap<TResult> Apply<TSource, TResult>(
            this Mayhap<TSource> @this,
            Mayhap<Func<TSource, TResult>> applicative)
        {
            return applicative.Bind(func => @this.Select(func));
        }
    }

    // Extension methods for Mayhap<T> where T is a func.
    public partial class Mayhap
    {
        /// <summary>(&lt;*&gt;) / ap</summary>
        // [Applicative]
        //   (<*>) :: f (a -> b) -> f a -> f b
        //   (<*>) = liftA2 id
        //
        //   Sequential application.
        //   A few functors support an implementation of <*> that is more efficient
        //   than the default one.
        //
        // [Monad]
        //   ap :: Monad m => m (a -> b) -> m a -> m b
        //   ap m1 m2 = do { x1 <- m1; x2 <- m2; return (x1 x2) }
        //
        //   In many situations, the liftM operations can be replaced by uses of
        //   ap, which promotes function application.
        [Pure]
        public static Mayhap<TResult> Invoke<TSource, TResult>(
            this Mayhap<Func<TSource, TResult>> @this, Mayhap<TSource> mayhap)
        {
            return @this.Bind(func => mayhap.Select(func));
        }
    }

    // Lift, promote functions to actions (ie Mayhap's).
    public partial class Mayhap
    {
        /// <summary>liftA</summary>
        // [Applicative]
        //   liftA :: Applicative f => (a -> b) -> f a -> f b
        //   liftA f a = pure f <*> a
        //
        //   Lift a function to actions.
        //   This function may be used as a value for `fmap` in a `Functor`
        //   instance.
        [Pure]
        public static Func<Mayhap<TSource>, Mayhap<TResult>> Lift<TSource, TResult>(
            Func<TSource, TResult> func)
        {
            return m => Of(func).Invoke(m);
        }

        /// <summary>liftA2</summary>
        // [Applicative]
        //   liftA2 :: (a -> b -> c) -> f a -> f b -> f c
        //   liftA2 f x = (<*>) (fmap f x)
        //   liftA2 f x y = f <$> x <*> y
        //
        //   Lift a binary function to actions.
        //   Some functors support an implementation of liftA2 that is more efficient
        //   than the default one.In particular, if fmap is an expensive operation,
        //   it is likely better to use liftA2 than to fmap over the structure and
        //   then use<*>.
        [Pure]
        public static Func<Mayhap<T1>, Mayhap<T2>, Mayhap<TResult>> Lift<T1, T2, TResult>(
            Func<T1, T2, TResult> func)
        {
            return (m1, m2) =>
                m1.Bind(
                    x1 => m2.Bind(
                        x2 => Of(func(x1, x2))));
        }

        /// <summary>liftA3</summary>
        // [Applicative]
        //   liftA3 :: Applicative f => (a -> b -> c -> d) -> f a -> f b -> f c -> f d
        //   liftA3 f a b c = liftA2 f a b <*> c
        //
        //   Lift a ternary function to actions.
        [Pure]
        public static Func<Mayhap<T1>, Mayhap<T2>, Mayhap<T3>, Mayhap<TResult>>
            Lift<T1, T2, T3, TResult>(
            Func<T1, T2, T3, TResult> func)
        {
            return (m1, m2, m3) =>
                m1.Bind(
                    x1 => m2.Bind(
                        x2 => m3.Bind(
                            x3 => Of(func(x1, x2, x3)))));
        }
    }
}
