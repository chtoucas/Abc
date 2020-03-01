// See LICENSE.txt in the project root for license information.

namespace Abc.Fx
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    // [The Haskell 98 Report](https://www.haskell.org/onlinereport/monad.html).

    // IEnumerable<T>, but a bit missleading?
    // IEquatable<T>, but a bit missleading?
    // IComparable? See ValueTuple.
    // Serializable?
    // nullable attrs.
    // Enhance and improve async methods.

    public static partial class Mayhap
    {
        /// <summary>return</summary>
        [Pure]
        public static Mayhap<T> Of<T>([AllowNull]T value)
            => Mayhap<T>.η(value);

        /// <summary>return</summary>
        [Pure]
        public static Mayhap<T> Of<T>(T? value) where T : struct
            => value.HasValue ? Mayhap<T>.Some(value.Value) : Mayhap<T>.None;

        /// <summary>Just</summary>
        [Pure]
        public static Mayhap<T> Some<T>(T value) where T : struct
            => Mayhap<T>.Some(value);

        /// <summary>join</summary>
        [Pure]
        public static Mayhap<T> Flatten<T>(this Mayhap<Mayhap<T>> @this)
            => Mayhap<T>.μ(@this);
    }

    /// <summary>
    /// Represents the Maybe monad.
    /// <para><see cref="Mayhap{T}"/> is an immutable struct.</para>
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public readonly partial struct Mayhap<T> : IEquatable<Mayhap<T>>
    {
        private static readonly IEqualityComparer<T> s_DefaultComparer
            = EqualityComparer<T>.Default;

        private readonly bool _isSome;
        private readonly T _value;

        private Mayhap([DisallowNull]T value)
        {
            _isSome = true;
            _value = value;
        }

        private string DebuggerDisplay => $"IsSome = {_isSome}";

        [Pure]
        public override string ToString()
            => SwitchIntern(x => $"Mayhap({x})", "Mayhap(None)");
    }

    // Pattern matching.
    public partial struct Mayhap<T>
    {
        [Pure]
        public TResult Switch<TResult>(Func<T, TResult> caseSome, Func<TResult> caseNone)
        {
            if (_isSome)
            {
                Require.NotNull(caseSome, nameof(caseSome));
                return caseSome(_value);
            }
            else
            {
                Require.NotNull(caseNone, nameof(caseNone));
                return caseNone();
            }
        }

        // Could be built upon the other Switch().
        [Pure]
        [return: NotNullIfNotNull("caseNone")]
        public TResult Switch<TResult>(Func<T, TResult> caseSome, TResult caseNone)
        {
            if (_isSome)
            {
                Require.NotNull(caseSome, nameof(caseSome));
                return caseSome(_value);
            }
            else
            {
                return caseNone;
            }
        }

        // Could be built upon Switch().
        internal void Do(Action<T> caseSome, Action caseNone)
        {
            if (_isSome)
            {
                Require.NotNull(caseSome, nameof(caseSome));
                caseSome(_value);
            }
            else
            {
                Require.NotNull(caseNone, nameof(caseNone));
                caseNone();
            }
        }

        [Pure]
        internal TResult SwitchIntern<TResult>(Func<T, TResult> caseSome, Func<TResult> caseNone)
            => _isSome ? caseSome(_value) : caseNone();

        [Pure]
        [return: NotNullIfNotNull("caseNone")]
        internal TResult SwitchIntern<TResult>(Func<T, TResult> caseSome, TResult caseNone)
            => _isSome ? caseSome(_value) : caseNone;
    }

    // Core monadic methods.
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
        internal static Mayhap<T> η(T value)
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

        [Pure]
        public async Task<TResult> SwitchAsync<TResult>(
            Func<T, Task<TResult>> caseSome, Task<TResult> caseNone)
        {
            if (_isSome)
            {
                Require.NotNull(caseSome, nameof(caseSome));
                return await caseSome(_value).ConfigureAwait(false);
            }
            else
            {
                Require.NotNull(caseNone, nameof(caseNone));
                return await caseNone.ConfigureAwait(false);
            }
        }
    }

    // Pseudo-interface IEnumerable<>.
    public partial struct Mayhap<T>
    {
        [Pure]
        public IEnumerator<T> GetEnumerator()
        {
            if (_isSome)
            {
                yield return _value;
            }
        }
    }

    // Interface IEquatable<>.
    public partial struct Mayhap<T>
    {
        public static bool operator ==(Mayhap<T> left, Mayhap<T> right)
            => left.Equals(right);

        public static bool operator !=(Mayhap<T> left, Mayhap<T> right)
            => !left.Equals(right);

        public static bool operator ==(Mayhap<T> left, T right)
            => left.Contains(right);

        public static bool operator ==(T left, Mayhap<T> right)
            => right.Contains(left);

        public static bool operator !=(Mayhap<T> left, T right)
            => !left.Contains(right);

        public static bool operator !=(T left, Mayhap<T> right)
            => !right.Contains(left);

        [Pure]
        public bool Equals(Mayhap<T> other)
            => SwitchIntern(x => other.Contains(x), !other._isSome);

        [Pure]
        public bool Equals(Mayhap<T> other, IEqualityComparer<T> comparer)
            => SwitchIntern(x => other.Contains(x, comparer), !other._isSome);

        [Pure]
        public bool Contains(T value)
            => SwitchIntern(x => s_DefaultComparer.Equals(x, value), false);

        [Pure]
        public bool Contains(T value, IEqualityComparer<T> comparer)
            => SwitchIntern(x => (comparer ?? s_DefaultComparer).Equals(x, value), false);

        [Pure]
        public override bool Equals(object? obj)
            => obj is Mayhap<T> maybe && Equals(maybe);

        [Pure]
        public bool Equals(object? other, IEqualityComparer<T> comparer)
            => other is Mayhap<T> maybe && Equals(maybe, comparer);

        [Pure]
        public override int GetHashCode()
            => SwitchIntern(x => x!.GetHashCode(), 0);

        [Pure]
        public int GetHashCode(IEqualityComparer<T> comparer)
            => SwitchIntern((comparer ?? s_DefaultComparer).GetHashCode, 0);
    }
}
