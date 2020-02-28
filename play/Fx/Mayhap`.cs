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
            => _isSome ? $"Mayhap({_value})" : "Mayhap(None)";

        [Pure]
        public TResult Match<TResult>(Func<T, TResult> caseSome, Func<TResult> caseNone)
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

        [Pure]
        [return: NotNullIfNotNull("caseNone")]
        public TResult Match<TResult>(Func<T, TResult> caseSome, TResult caseNone)
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
    }

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

        /// <summary>(&lt;&amp;&gt;) / fmap / liftM</summary>
        // [Functor]
        //   (<&>) :: Functor f => f a -> (a -> b) -> f b | infixl 1 |
        //   (<&>) = flip fmap
        //
        //    Flipped version of <$>.
        //
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

    // Query Expression Pattern aka LINQ.
    public partial struct Mayhap<T>
    {
        [Pure]
        public Mayhap<T> Where(Func<T, bool> predicate)
        {
            if (predicate is null) { throw new ArgumentNullException(nameof(predicate)); }

            // NB: x is never null.
            return Bind(x => predicate(x) ? Some(x) : None);
        }

        [Pure]
        public Mayhap<TResult> SelectMany<TMiddle, TResult>(
            Func<T, Mayhap<TMiddle>> selector,
            Func<T, TMiddle, TResult> resultSelector)
        {
            if (selector is null) { throw new ArgumentNullException(nameof(selector)); }
            if (resultSelector is null) { throw new ArgumentNullException(nameof(resultSelector)); }

            return Bind(
                x => selector(x).Select(
                    middle => resultSelector(x, middle)));
        }

        [Pure]
        public Mayhap<TResult> Join<TInner, TKey, TResult>(
            Mayhap<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector)
        {
            return Join(inner, outerKeySelector, innerKeySelector, resultSelector, null!);
        }

        [Pure]
        public Mayhap<TResult> Join<TInner, TKey, TResult>(
            Mayhap<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            if (outerKeySelector is null) { throw new ArgumentNullException(nameof(outerKeySelector)); }
            if (innerKeySelector is null) { throw new ArgumentNullException(nameof(innerKeySelector)); }
            if (resultSelector is null) { throw new ArgumentNullException(nameof(resultSelector)); }

            var keyLookup = __getKeyLookup(inner, innerKeySelector, comparer);

            return SelectMany(__valueSelector, resultSelector);

            Mayhap<TInner> __valueSelector(T outer) => keyLookup(outerKeySelector(outer));

            static Func<TKey, Mayhap<TInner>> __getKeyLookup(
               Mayhap<TInner> inner,
               Func<TInner, TKey> innerKeySelector,
               IEqualityComparer<TKey>? comparer)
            {
                return outerKey =>
                    inner.Select(innerKeySelector)
                        .Where(innerKey =>
                            (comparer ?? EqualityComparer<TKey>.Default)
                                .Equals(innerKey, outerKey))
                        .ContinueWith(inner);
            }
        }

        //
        // GroupJoin currently disabled.
        //

        //[Pure]
        //public Mayhap<TResult> GroupJoin<TInner, TKey, TResult>(
        //    Mayhap<TInner> inner,
        //    Func<T, TKey> outerKeySelector,
        //    Func<TInner, TKey> innerKeySelector,
        //    Func<T, Mayhap<TInner>, TResult> resultSelector,
        //    IEqualityComparer<TKey> comparer)
        //{
        //    if (outerKeySelector is null) { throw new ArgumentNullException(nameof(outerKeySelector)); }
        //    if (innerKeySelector is null) { throw new ArgumentNullException(nameof(innerKeySelector)); }
        //    if (resultSelector is null) { throw new ArgumentNullException(nameof(resultSelector)); }

        //    if (_isSome && inner._isSome)
        //    {
        //        var outerKey = outerKeySelector(_value);
        //        var innerKey = innerKeySelector(inner._value);

        //        if ((comparer ?? EqualityComparer<TKey>.Default).Equals(outerKey, innerKey))
        //        {
        //            return Mayhap.Of(resultSelector(_value, inner));
        //        }
        //    }

        //    return Mayhap<TResult>.None;
        //}
    }

    // Async methods.
    public partial struct Mayhap<T>
    {
        [Pure]
        public async Task<Mayhap<TResult>> BindAsync<TResult>(
            Func<T, Task<Mayhap<TResult>>> binder)
        {
            if (binder is null) { throw new ArgumentNullException(nameof(binder)); }

            return _isSome ? await binder(_value).ConfigureAwait(false)
                : Mayhap<TResult>.None; ;
        }

        [Pure]
        public async Task<Mayhap<TResult>> SelectAsync<TResult>(
            Func<T, Task<TResult>> selector)
        {
            if (selector is null) { throw new ArgumentNullException(nameof(selector)); }

            return _isSome ? Mayhap.Of(await selector(_value).ConfigureAwait(false))
                : Mayhap<TResult>.None;
        }
    }

    // Iterable.
    public partial struct Mayhap<T>
    {
        //[Pure]
        //public IEnumerable<T> ToEnumerable()
        //{
        //    if (_isSome)
        //    {
        //        yield return _value;
        //    }
        //}

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
            => Match(x => other.Contains(x), !other._isSome);

        [Pure]
        public bool Equals(Mayhap<T> other, IEqualityComparer<T> comparer)
            => Match(x => other.Contains(x, comparer), !other._isSome);

        [Pure]
        public bool Contains(T value)
            => Match(x => s_DefaultComparer.Equals(x, value), false);

        [Pure]
        public bool Contains(T value, IEqualityComparer<T> comparer)
            => Match(x => (comparer ?? s_DefaultComparer).Equals(x, value), false);

        [Pure]
        public override bool Equals(object? obj)
            => obj is Mayhap<T> maybe && Equals(maybe);

        [Pure]
        public bool Equals(object? other, IEqualityComparer<T> comparer)
            => other is Mayhap<T> maybe && Equals(maybe, comparer);

        [Pure]
        public override int GetHashCode()
            => Match(x => x!.GetHashCode(), 0);

        [Pure]
        public int GetHashCode(IEqualityComparer<T> comparer)
            => Match((comparer ?? s_DefaultComparer).GetHashCode, 0);
    }
}
