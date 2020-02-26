// See LICENSE.txt in the project root for license information.

namespace Abc.Fx
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    // Data.Functor
    // ============
    //
    // Functors: uniform action over a parameterized type, generalizing the map
    // function on lists.
    // The Functor class is used for types that can be mapped over.
    //
    // https://hackage.haskell.org/package/base-4.12.0.0/docs/Data-Functor.html
    //
    // Methods
    // -------
    // Bare bone:
    // - fmap   obj.Select()
    //
    // Standard API:
    // - <$     obj.ReplaceWith()
    // - $>     obj.ReplaceWith()
    // - <$>    obj.Select()
    // - <&>    Func.Invoke()
    // - void   obj.Skip()
    //
    // NB: if we followed the signature, fmap (resp. <&>) would rather be
    // Func.Invoke() (resp. obj.Select()).
    //
    // Functor rules
    // -------------
    // First law: the identity map is a fixed point for Select.
    //   fmap id  ==  id
    // Second law: Select preserves the composition operator.
    //   fmap (f . g)  ==  fmap f . fmap g
    public partial class Mayhap
    {
        // [Functor]
        //   (<$) :: Functor f => a -> f b -> f a
        //
        //   (<$) :: a -> f b -> f a
        //   (<$) =  fmap . const
        //
        //   Replace all locations in the input with the same value. The default
        //   definition is fmap . const, but this may be overridden with a more
        //   efficient version.
        //
        // [Functor]
        //   ($>) :: Functor f => f a -> b -> f b
        //   ($>) = flip (<$)
        //
        //   Flipped version of <$.
        [Pure]
        public static Mayhap<TResult> ReplaceWith<TSource, TResult>(
            this Mayhap<TSource> @this,
            TResult value)
            where TResult : notnull
        {
            return @this.Select(_ => value);
        }

        // [Functor]
        //   (<&>) :: Functor f => f a -> (a -> b) -> f b
        //   (<&>) = flip fmap
        //
        //    Flipped version of <$>.
        [Pure]
        public static Mayhap<TResult> Invoke<TSource, TResult>(
            this Func<TSource, TResult> @this,
            Mayhap<TSource> maybe)
        {
            Require.NotNull(@this, nameof(@this));

            return maybe.Select(x => @this(x));
        }

        // [Functor]
        //   void :: Functor f => f a -> f ()
        //   void x = () <$ x
        //
        //   void value discards or ignores the result of evaluation, such as
        //   the return value of an IO action.
        [Pure]
        public static Mayhap<Unit> Skip<TSource>(this Mayhap<TSource> @this)
            => @this.Select(_ => Abc.Unit.Default);
    }

    // Control.Applicative
    // ===================
    //
    // http://hackage.haskell.org/package/base-4.12.0.0/docs/Control-Applicative.html
    //
    // Methods
    // -------
    // Bare bone (<*> or liftA2):
    // - pure
    // - <*>
    // - liftA2
    //
    // Standard API:
    // - *>
    // - <*
    // - <**>
    // - liftA
    // - liftA3
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
        /// <summary>Applicative (&lt;*&gt;)</summary>
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
            this Mayhap<Func<TSource, TResult>> @this, Mayhap<TSource> value)
        {
            return @this.Bind(func => value.Select(func));
        }

        /// <summary>Applicative (*&gt;)</summary>
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

        /// <summary>Applicative (&lt;*)</summary>
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
            return @this.ZipWith(other, (x, _) => x);
        }

        /// <summary>Applicative (&lt;**&gt;)</summary>
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

    // Control.Alternative
    // ===================
    public partial class Mayhap
    {
    }

    // Control.Monad
    // =============
    //
    // http://hackage.haskell.org/package/base-4.12.0.0/docs/Control-Monad.html
    // https://downloads.haskell.org/~ghc/latest/docs/html/libraries/base-4.13.0.0/Control-Monad.html
    //
    // Methods
    // -------
    // Bare bone:
    // - >>=
    // - >>
    // - return
    // - fail
    //
    // Standard API:
    public partial class Mayhap
    {
    }

    public partial class Mayhap
    {
        public static readonly Mayhap<Unit> Unit = Some(Abc.Unit.Default);

        public static readonly Mayhap<Unit> None = Mayhap<Unit>.None;

        [Pure]
        public static Mayhap<Unit> Guard(bool predicate)
            => predicate ? Unit : None;
    }

    // Extension methods for functions in the Kleisli category.
    public partial class Mayhap
    {
        [Pure]
        public static Mayhap<TResult> Invoke<TSource, TResult>(
            this Func<TSource, Mayhap<TResult>> @this, Mayhap<TSource> value)
        {
            return value.Bind(@this);
        }

        [Pure]
        public static Func<TSource, Mayhap<TResult>> Compose<TSource, TMiddle, TResult>(
            this Func<TSource, Mayhap<TMiddle>> @this, Func<TMiddle, Mayhap<TResult>> other)
        {
            Require.NotNull(@this, nameof(@this));

            return x => @this(x).Bind(other);
        }

        [Pure]
        public static Func<TSource, Mayhap<TResult>> ComposeBack<TSource, TMiddle, TResult>(
            this Func<TMiddle, Mayhap<TResult>> @this, Func<TSource, Mayhap<TMiddle>> other)
        {
            Require.NotNull(other, nameof(other));

            return x => other(x).Bind(@this);
        }
    }

    // Extension methods for Mayhap<T> where T is a struct.
    public partial class Mayhap
    {
        [Pure]
        public static Mayhap<T> Squash<T>(this Mayhap<T?> @this) where T : struct
            // NB: When IsSome is true, Value.HasValue is also true, therefore
            // we can safely access Value.Value.
            => @this.Bind(x => Some(x!.Value));

        [Pure]
        public static T? ToNullable<T>(this Mayhap<T?> @this) where T : struct
            => @this.ValueOrDefault();

        [Pure]
        public static T? ToNullable<T>(this Mayhap<T> @this) where T : struct
            => @this.ValueOrDefault();
    }

    // Extension methods for Mayhap<T> where T is enumerable.
    // Operations on IEnumerable<Mayhap<T>>.
    // - Filtering: CollectAny (deferred).
    // - Aggregation: Any.
    public partial class Mayhap
    {
        [Pure]
        public static Mayhap<IEnumerable<T>> Empty<T>()
            => MayhapEnumerable_<T>.Empty;

        [Pure]
        public static IEnumerable<T> ValueOrEmpty<T>(this Mayhap<IEnumerable<T>> @this)
            => @this.ValueOrElse(Enumerable.Empty<T>());

        [Pure]
        public static IEnumerable<T> CollectAny<T>(IEnumerable<Mayhap<T>> source)
        {
            var seed = MayhapEnumerable_<T>.Empty;
            var seq = source.Aggregate(seed, (x, y) => x.ZipWith(y, Enumerable.Append));
            return seq.ValueOrEmpty();
        }

        [Pure]
        public static Mayhap<T> Any<T>(IEnumerable<Mayhap<T>> source)
        {
            return source.Aggregate(Mayhap<T>.None, (m, n) => m.OrElse(n));
        }

        private static class MayhapEnumerable_<T>
        {
            internal static readonly Mayhap<IEnumerable<T>> Empty
                = Of(Enumerable.Empty<T>());
        }
    }

    // Lift/ZipWith
    //   Promote a function to a monad, scanning the monadic arguments from
    //   left to right.
    // See also Select() and ZipWith().
    public partial class Mayhap
    {
        /// <summary>Applicative liftA</summary>
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
            if (zipper is null) { throw new ArgumentNullException(nameof(zipper)); }

            return @this.Bind(
                x => other.Bind(
                    y => Of(zipper(x, y))));
        }

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
            if (zipper is null) { throw new ArgumentNullException(nameof(zipper)); }

            return @this.Bind(
                x => m1.ZipWith(m2, (y, z) => zipper(x, y, z)));
        }

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
                                x4 => Of(func(x1, x2, x3, x4))))));
        }

        [Pure]
        public static Mayhap<TResult> ZipWith<TSource, T1, T2, T3, TResult>(
            this Mayhap<TSource> @this,
             Mayhap<T1> first,
             Mayhap<T2> second,
             Mayhap<T3> third,
             Func<TSource, T1, T2, T3, TResult> zipper)
        {
            if (zipper is null) { throw new ArgumentNullException(nameof(zipper)); }

            return @this.Bind(
                x => first.ZipWith(
                    second,
                    third,
                    (y, z, a) => zipper(x, y, z, a)));
        }

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
                                    x5 => Of(func(x1, x2, x3, x4, x5)))))));
        }
    }
}
