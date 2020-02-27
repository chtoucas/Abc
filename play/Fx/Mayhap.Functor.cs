// See LICENSE.txt in the project root for license information.

namespace Abc.Fx
{
    using System;
    using System.Diagnostics.Contracts;

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
    // Bare minimum:
    // - fmap   Mayhap.Map()
    //
    // Standard API:
    // - <$     Mayhap.ReplaceWith()
    // - $>     obj.ReplaceWith()
    // - <$>    func.Invoke()
    // - <&>    obj.Select()
    // - void   obj.Skip()
    //
    // Functor rules
    // -------------
    // First law: the identity map is a fixed point for Select.
    //   fmap id  ==  id
    // Second law: Select preserves the composition operator.
    //   fmap (f . g)  ==  fmap f . fmap g
    public partial class Mayhap
    {
        /// <summary>fmap</summary>
        // [Functor]
        //   fmap :: (a -> b) -> f a -> f b
        [Pure]
        public static Mayhap<TResult> Map<TSource, TResult>(
            Func<TSource, TResult> mapper,
            Mayhap<TSource> mayhap)
        {
            return mayhap.Select(mapper);
        }

        /// <summary>(&lt;$)</summary>
        // [Functor]
        //   (<$) :: Functor f => a -> f b -> f a | infixl 4 |
        //
        //   (<$) :: a -> f b -> f a
        //   (<$) =  fmap . const
        //
        //   Replace all locations in the input with the same value. The default
        //   definition is fmap . const, but this may be overridden with a more
        //   efficient version.
        [Pure]
        public static Mayhap<TResult> ReplaceWith<TSource, TResult>(
            TResult value,
            Mayhap<TSource> mayhap)
            where TResult : notnull
        {
#if STRICT_HASKELL
            return Map(_ => value, mayhap);
#else
            return mayhap.Select(_ => value);
#endif
        }

        /// <summary>($&gt;)</summary>
        // [Functor]
        //   ($>) :: Functor f => f a -> b -> f b | infixl 4 |
        //   ($>) = flip (<$)
        //
        //   Flipped version of <$.
        [Pure]
        public static Mayhap<TResult> ReplaceWith<TSource, TResult>(
            this Mayhap<TSource> @this,
            TResult value)
            where TResult : notnull
        {
#if STRICT_HASKELL
            return ReplaceWith(value, @this);
#else
            return @this.Select(_ => value);
#endif
        }

        /// <summary>void</summary>
        // [Functor]
        //   void :: Functor f => f a -> f ()
        //   void x = () <$ x
        //
        //   void value discards or ignores the result of evaluation, such as
        //   the return value of an IO action.
        [Pure]
        public static Mayhap<Unit> Skip<TSource>(
            this Mayhap<TSource> @this)
        {
#if STRICT_HASKELL
            return ReplaceWith(Abc.Unit.Default, @this);
#else
            return @this.Select(_ => Abc.Unit.Default);
#endif
        }
    }

    // Extension methods for Mayhap<T> where T is a func.
    public partial class Mayhap
    {
        /// <summary>(&lt;$&gt;></summary>
        // [Functor]
        //   (<$>) :: Functor f => (a -> b) -> f a -> f b | infixl 4 |
        //   (<$>) = fmap
        //
        //   An infix synonym for fmap.
        //   The name of this operator is an allusion to $.
        [Pure]
        public static Mayhap<TResult> Invoke<TSource, TResult>(
            this Func<TSource, TResult> @this,
            Mayhap<TSource> mayhap)
        {
#if STRICT_HASKELL
            return Map(@this, mayhap);
#else
            Require.NotNull(@this, nameof(@this));

            return mayhap.Select(x => @this(x));
#endif
        }
    }
}
