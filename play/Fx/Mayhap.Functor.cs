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
        /// <summary>(&lt;$)</summary>
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

        /// <summary>(&lt;&amp;&gt;)</summary>
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

        /// <summary>void</summary>
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
}
