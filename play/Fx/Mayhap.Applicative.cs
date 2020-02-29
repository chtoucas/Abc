// See LICENSE.txt in the project root for license information.

namespace Abc.Fx
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    // Applicative Functor
    // ===================
    //
    // References:
    // - https://wiki.haskell.org/Applicative_functor
    // - http://hackage.haskell.org/package/base-4.12.0.0/docs/Control-Applicative.html
    //
    // Methods
    // -------
    // First, an applicative functor is a functor...
    //
    // Bare minimum (<*> or liftA2, here we choose <*>):
    // - pure       Mayhap.Pure()
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
    // If an applicative functor is also a monad, it should satisfy:
    // - pure = return
    // - (<*>) = ap
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
        /// <summary>
        /// pure
        /// <para>Embed pure expressions, ie lift a value.</para>
        /// </summary>
        [Pure]
        public static Mayhap<T> Pure<T>([AllowNull]T value)
        {
            // Default implementation when Mayhap is a monad.
            return Of(value);
        }

        /// <summary>
        /// (*&gt;)
        /// <para>Sequence actions, discarding the value of the first argument.
        /// </para>
        /// </summary>
        [Pure]
        public static Mayhap<TResult> ContinueWith<TSource, TResult>(
            this Mayhap<TSource> @this,
            Mayhap<TResult> other)
        {
            // (*>) :: f a -> f b -> f b
            // a1 *> a2 = (id <$ a1) <*> a2
            //
            // This is essentially the same as liftA2 (flip const), but if the
            // Functor instance has an optimized (<$), it may be better to use
            // that instead.Before liftA2 became a method, this definition
            // was strictly better, but now it depends on the functor.For a
            // functor supporting a sharing-enhancing (<$), this definition
            // may reduce allocation by preventing a1 from ever being fully
            // realized.In an implementation with a boring (<$) but an optimizing
            // liftA2, it would likely be better to define (*>) using liftA2.

            return @this.Bind(_ => other);
        }

        /// <summary>
        /// (&lt;*)
        /// <para>Sequence actions, discarding the value of the second argument.
        /// </para>
        /// </summary>
        [Pure]
        public static Mayhap<TSource> PassThru<TSource, TOther>(
            this Mayhap<TSource> @this,
            Mayhap<TOther> other)
        {
            // (<*) :: f a -> f b -> f a
            // (<*) = liftA2 const

#if STRICT_HASKELL
            return Lift(Stubs<TSource, TOther>.Const1).Invoke(@this, other);
#else
            return @this.ZipWith(other, Stubs<TSource, TOther>.Const1);
#endif
        }

        /// <summary>
        /// (&lt;**&gt;)
        /// <para>A variant of (&lt;*&gt;) with the arguments reversed.</para>
        /// </summary>
        [Pure]
        public static Mayhap<TResult> Apply<TSource, TResult>(
            this Mayhap<TSource> @this,
            Mayhap<Func<TSource, TResult>> applicative)
        {
            // (<**>) :: Applicative f => f a -> f (a -> b) -> f b
            // (<**>) = liftA2(\a f -> f a)
            //
            // A variant of '<*>' with the arguments reversed.

            return applicative.Bind(func => @this.Select(func));
        }
    }

    // Extension methods for Mayhap<T> where T is a func.
    public partial class Mayhap
    {
        /// <summary>
        /// (&lt;*&gt;)
        /// <para>Sequential application.</para>
        /// </summary>
        // [Monad]
        //   ap :: Monad m => m (a -> b) -> m a -> m b
        //   ap m1 m2 = do { x1 <- m1; x2 <- m2; return (x1 x2) }
        //
        //   In many situations, the liftM operations can be replaced by uses of
        //   ap, which promotes function application.
        [Pure]
        public static Mayhap<TResult> Invoke<TSource, TResult>(
            this Mayhap<Func<TSource, TResult>> @this,
            Mayhap<TSource> mayhap)
        {
            // (<*>) :: f (a -> b) -> f a -> f b
            // (<*>) = liftA2 id
            //
            // A few functors support an implementation of <*> that is more efficient
            // than the default one.

            return @this.Bind(func => mayhap.Select(func));
        }

        /// <summary>
        /// (&lt;*&gt;)
        /// <para>Sequential application.</para>
        /// </summary>
        [Pure]
        public static Mayhap<TResult> Invoke<T1, T2, TResult>(
            this Mayhap<Func<T1, T2, TResult>> @this,
            Mayhap<T1> m1,
            Mayhap<T2> m2)
        {
            throw new NotImplementedException();
        }
    }

    // Lift, promote functions to actions (ie Mayhap's).
    public partial class Mayhap
    {
        /// <summary>
        /// liftA
        /// <para>Lift a function to actions.</para>
        /// </summary>
        [Pure]
        public static Func<Mayhap<TSource>, Mayhap<TResult>> Lift<TSource, TResult>(
            Func<TSource, TResult> func)
        {
            // liftA :: Applicative f => (a -> b) -> f a -> f b
            // liftA f a = pure f <*> a
            //
            // This function may be used as a value for `fmap` in a `Functor`
            // instance.

            return m => Of(func).Invoke(m);
        }

        /// <summary>
        /// liftA2
        /// <para>Lift a binary function to actions.</para>
        /// </summary>
        [Pure]
        public static Func<Mayhap<T1>, Mayhap<T2>, Mayhap<TResult>> Lift<T1, T2, TResult>(
            Func<T1, T2, TResult> func)
        {
            // liftA2 :: (a -> b -> c) -> f a -> f b -> f c
            // liftA2 f x = (<*>) (fmap f x)
            // liftA2 f x y = f <$> x <*> y
            //
            // Some functors support an implementation of liftA2 that is more efficient
            // than the default one.In particular, if fmap is an expensive operation,
            // it is likely better to use liftA2 than to fmap over the structure and
            // then use<*>.

#if STRICT_HASKELL
            throw new NotImplementedException();
#else
            return (m1, m2) =>
                m1.Bind(
                    x1 => m2.Bind(
                        x2 => Of(func(x1, x2))));
#endif
        }

        /// <summary>
        /// liftA3
        /// <para>Lift a ternary function to actions.</para>
        /// </summary>
        [Pure]
        public static Func<Mayhap<T1>, Mayhap<T2>, Mayhap<T3>, Mayhap<TResult>>
            Lift<T1, T2, T3, TResult>(
            Func<T1, T2, T3, TResult> func)
        {
            // liftA3 :: Applicative f => (a -> b -> c -> d) -> f a -> f b -> f c -> f d
            // liftA3 f a b c = liftA2 f a b <*> c

            return (m1, m2, m3) =>
                m1.Bind(
                    x1 => m2.Bind(
                        x2 => m3.Bind(
                            x3 => Of(func(x1, x2, x3)))));
        }
    }

    // Applicative rules.
    public partial class Mayhap
    {
        internal static class ApplicativeRules
        {
            // pure id <*> v = v
            public static bool IdentityRule<T>(Mayhap<T> mayhap)
            {
                return Pure(Stubs<T>.Ident).Invoke(mayhap)
                    == mayhap;
            }

            // pure (.) <*> u <*> v <*> w = u <*> (v <*> w)
            public static bool Composition()
            {
                throw new NotImplementedException();
            }

            // pure f <*> pure x = pure (f x)
            public static bool HomomorphismRule<T1, T2>(Func<T1, T2> f, T1 value)
            {
                return Pure(f).Invoke(Pure(value))
                    == Pure(f(value));
            }

            // u <*> pure y = pure ($ y) <*> u
            public static bool Interchange()
            {
                throw new NotImplementedException();
            }
        }
    }
}
