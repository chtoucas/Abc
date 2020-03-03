﻿// See LICENSE.txt in the project root for license information.

namespace Abc.Fx
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    // [The Haskell 98 Report](https://www.haskell.org/onlinereport/monad.html).

    // FIXME: nullable attrs.

    /// <summary>
    /// Represents the Maybe monad.
    /// <para><see cref="Mayhap{T}"/> is a read-only struct.</para>
    /// </summary>
    public readonly partial struct Mayhap<T> : IEquatable<Mayhap<T>>, IStructuralEquatable
    {
        private readonly bool _isSome;
        private readonly T _value;

        private Mayhap([DisallowNull]T value)
        {
            _isSome = true;
            _value = value;
        }

        [Pure]
        public override string ToString()
            => match(
                some: x => $"Mayhap({x})",
                none: "Mayhap(None)");

        [Pure]
        public IEnumerator<T> GetEnumerator()
        {
            if (_isSome)
            {
                yield return _value;
            }
        }

        [Pure]
        public bool Contains(T value)
            => match(
                some: x => EqualityComparer<T>.Default.Equals(x, value),
                none: false);

        [Pure]
        public bool Contains(T value, IEqualityComparer<T> comparer)
            => match(
                some: x => (comparer ?? EqualityComparer<T>.Default).Equals(x, value),
                none: false);

        [Pure]
        [return: NotNullIfNotNull("none")]
        private TResult match<TResult>(Func<T, TResult> some, TResult none)
            => _isSome ? some(_value) : none;
    }

    // Core methods.
    public partial struct Mayhap<T>
    {
#pragma warning disable CA1000 // Do not declare static members on generic types
        /// <summary>Nothing</summary>
        public static Mayhap<T> None { get; } = default;
#pragma warning restore CA1812

        /// <summary>Just</summary>
        // The unit (wrap a value, public ctor).
        //
        // [Monad]
        //   return :: a -> m a
        //   Inject a value into the monadic type.
        [Pure]
        internal static Mayhap<T> Some([DisallowNull]T value)
            => new Mayhap<T>(value);

        /// <summary>return</summary>
        [Pure]
        internal static Mayhap<T> η([AllowNull]T value)
            => value is null ? Mayhap<T>.None : Mayhap<T>.Some(value);

        /// <summary>join</summary>
        // The multiplication or composition.
        //
        // [Monad]
        //   join :: Monad m => m (m a) -> m a
        //   The join function is the conventional monad join operator. It is
        //   used to remove one level of monadic structure, projecting its bound
        //   argument into the outer level.
        [Pure]
        internal static Mayhap<T> μ(Mayhap<Mayhap<T>> square)
            => square._value;

        /// <summary>fmap / liftM</summary>
        // [Monad]
        //   fmap :: (a -> b) -> f a -> f b
        //
        // [Monad]
        //   liftM :: (Monad m) => (a1 -> r) -> m a1 -> m r
        //   liftM f m1 = do { x1 <- m1; return (f x1) }
        //
        //   Promote a function to a monad.
        [Pure]
        public Mayhap<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            Require.NotNull(selector, nameof(selector));

#if MONADS_VIA_MAP_MULTIPLY
            return _isSome ? Mayhap<TResult>.η(selector(_value)) : Mayhap<TResult>.None;
#else
            return Bind(x => Mayhap<TResult>.η(selector(x)));
#endif
        }

        /// <summary>(&gt;&gt;=)</summary>
        // [Monad]
        //   (>>=) :: forall a b. m a -> (a -> m b) -> m b
        //
        //   Sequentially compose two actions, passing any value produced by the
        //   first as an argument to the second.
        [Pure]
        public Mayhap<TResult> Bind<TResult>(Func<T, Mayhap<TResult>> binder)
        {
            Require.NotNull(binder, nameof(binder));

#if MONADS_VIA_MAP_MULTIPLY
            return Mayhap<TResult>.μ(Select(binder));
#else
            return _isSome ? binder(_value) : Mayhap<TResult>.None;
#endif
        }

        [Pure]
        public Mayhap<T> OrElse(Mayhap<T> other)
            => _isSome ? this : other;
    }

    // Async methods.
    public partial struct Mayhap<T>
    {
        [Pure]
        public async Task<Mayhap<TResult>> SelectAsync<TResult>(
            Func<T, Task<TResult>> selector)
        {
            Require.NotNull(selector, nameof(selector));

#if MONADS_VIA_MAP_MULTIPLY
            return _isSome ? Mayhap<TResult>.η(await selector(_value).ConfigureAwait(false))
                : Mayhap<TResult>.None;
#else
            return await BindAsync(__binder).ConfigureAwait(false);

            async Task<Mayhap<TResult>> __binder(T x)
                => Mayhap<TResult>.η(await selector(x).ConfigureAwait(false));
#endif
        }

        [Pure]
        public async Task<Mayhap<TResult>> BindAsync<TResult>(
            Func<T, Task<Mayhap<TResult>>> binder)
        {
            Require.NotNull(binder, nameof(binder));

#if MONADS_VIA_MAP_MULTIPLY
            return Mayhap<TResult>.μ(await SelectAsync(binder).ConfigureAwait(false));
#else
            return _isSome ? await binder(_value).ConfigureAwait(false)
                : Mayhap<TResult>.None;
#endif
        }

        [Pure]
        public async Task<Mayhap<T>> OrElseAsync(Task<Mayhap<T>> other)
        {
            Require.NotNull(other, nameof(other));

            return _isSome ? this : await other.ConfigureAwait(false);
        }
    }

    // Interface IEquatable<>.
    public partial struct Mayhap<T>
    {
        public static bool operator ==(Mayhap<T> left, Mayhap<T> right)
            => left.Equals(right);

        public static bool operator !=(Mayhap<T> left, Mayhap<T> right)
            => !left.Equals(right);

        [Pure]
        public bool Equals(Mayhap<T> other)
            => match(
                some: x => other.Equals(x),
                none: !other._isSome);

        [Pure]
        public bool Equals(Mayhap<T> other, IEqualityComparer comparer)
            => match(
                some: x => other.Equals(x, comparer),
                none: !other._isSome);

        [Pure]
        public bool Equals(T value)
            => match(
                some: x => EqualityComparer<T>.Default.Equals(x, value),
                none: false);

        [Pure]
        public bool Equals(T value, IEqualityComparer comparer)
            => match(
                some: x => (comparer ?? EqualityComparer<T>.Default).Equals(x, value),
                none: false);

        [Pure]
        public override bool Equals(object? obj)
            => obj is Mayhap<T> maybe && Equals(maybe);

        [Pure]
        bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer)
            => other is Mayhap<T> maybe && Equals(maybe, comparer);

        [Pure]
        public override int GetHashCode()
            => match(
                some: x => x!.GetHashCode(),
                none: 0);

        [Pure]
        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
            => match(
                some: x => (comparer ?? EqualityComparer<T>.Default).GetHashCode(x!),
                none: 0);
    }
}
