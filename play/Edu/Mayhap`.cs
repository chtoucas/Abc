// See LICENSE.txt in the project root for license information.

namespace Abc.Edu
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an object that is either a single value of type T, or no
    /// value at all.
    /// <para><see cref="Mayhap{T}"/> is an immutable struct.</para>
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [DebuggerTypeProxy(typeof(Mayhap<>.DebugView_))]
    public readonly partial struct Mayhap<T> : IEquatable<Mayhap<T>>
    {
        private static readonly IEqualityComparer<T> s_DefaultComparer
            = EqualityComparer<T>.Default;

        private readonly bool _isSome;

        private readonly T _value;

        internal Mayhap([DisallowNull]T value)
        {
            Debug.Assert(value != null);

            _isSome = true;
            _value = value;
        }

        public bool IsNone => !_isSome;

        [ExcludeFromCodeCoverage]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        private string DebuggerDisplay => $"IsSome = {_isSome}";

        [Pure]
        public override string ToString()
            => Unwrap(x => $"Mayhap({x})", "Mayhap(None)");

        #region Core monadic methods

        [SuppressMessage("Microsoft.Design", "CA1000:Do not declare static members on generic types", Justification = "There is no such thing as a generic static property on a non-generic type.")]
        public static Mayhap<T> None { get; } = default;

        [Pure]
        public Mayhap<TResult> Bind<TResult>(Func<T, Mayhap<TResult>> binder)
        {
            if (binder is null) { throw new ArgumentNullException(nameof(binder)); }

            return _isSome ? binder(_value) : Mayhap<TResult>.None;
        }

        [Pure]
        public Mayhap<T> OrElse(Mayhap<T> other)
            => _isSome ? this : other;

        #endregion

        [ExcludeFromCodeCoverage]
        [SuppressMessage("Microsoft.Design", "CA1812:Avoid uninstantiated internal classes", Justification = "DebuggerTypeProxy")]
        private sealed class DebugView_
        {
            private readonly Mayhap<T> _inner;

            public DebugView_(Mayhap<T> inner) { _inner = inner; }

            public bool IsSome => _inner._isSome;

            public T Value => _inner._value;
        }
    }

    // Escaping the monad.
    public partial struct Mayhap<T>
    {
        [Pure]
        public TResult Unwrap<TResult>(Func<T, TResult> caseSome, Func<TResult> caseNone)
        {
            if (_isSome)
            {
                if (caseSome is null) { throw new ArgumentNullException(nameof(caseSome)); }
                return caseSome(_value);
            }
            else
            {
                if (caseNone is null) { throw new ArgumentNullException(nameof(caseNone)); }
                return caseNone();
            }
        }

        [Pure]
        public TResult Unwrap<TResult>(Func<T, TResult> caseSome, TResult caseNone)
        {
            if (_isSome)
            {
                if (caseSome is null) { throw new ArgumentNullException(nameof(caseSome)); }
                return caseSome(_value);
            }
            else
            {
                return caseNone;
            }
        }

        public void Do(Action<T> onSome, Action onNone)
        {
#pragma warning disable CA1806 // Do not ignore method results
            Unwrap(__some, __none);
#pragma warning restore CA1806

            Unit __some(T x) { onSome(x); return Unit.Default; }
            Unit __none() { onNone(); return Unit.Default; }
        }

        public void OnSome(Action<T> action)
            => Do(action, () => { });

        #region Specialized versions

        [Pure]
        [return: MaybeNull]
        public T ValueOrDefault()
            => Unwrap(x => x, default(T)!);

        [Pure]
        public T ValueOrElse([DisallowNull]T other)
            => Unwrap(x => x, other);

        [Pure]
        public T ValueOrElse(Func<T> valueFactory)
        {
            return Unwrap(x => x, __none);

            T __none()
            {
                if (valueFactory is null) { throw new ArgumentNullException(nameof(valueFactory)); }
                return valueFactory();
            }
        }

        [Pure]
        public T ValueOrThrow()
            => Unwrap(x => x, () => throw new InvalidOperationException());

        [Pure]
        public T ValueOrThrow(Func<Exception> exceptionFactory)
        {
            return Unwrap(x => x, __caseNone);

            T __caseNone()
            {
                if (exceptionFactory is null) { throw new ArgumentNullException(nameof(exceptionFactory)); }
                throw exceptionFactory(); ;
            }
        }

        #endregion
    }

    // Query Expression Pattern aka LINQ.
    public partial struct Mayhap<T>
    {
        [Pure]
        public Mayhap<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            if (selector is null) { throw new ArgumentNullException(nameof(selector)); }

            return Bind(x => Mayhap.Of(selector(x)));
        }

        [Pure]
        public Mayhap<T> Where(Func<T, bool> predicate)
        {
            if (predicate is null) { throw new ArgumentNullException(nameof(predicate)); }

            // NB: x is never null.
            return Bind(x => predicate(x) ? new Mayhap<T>(x) : None);
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

    // Standard API.
    public partial struct Mayhap<T>
    {
        [Pure]
        public Mayhap<TResult> ReplaceWith<TResult>(TResult value)
            where TResult : notnull
        {
            return Select(_ => value);
        }

        [Pure]
        public Mayhap<TResult> ContinueWith<TResult>(Mayhap<TResult> other)
        {
            return Bind(_ => other);
        }

        [Pure]
        public Mayhap<T> PassThru<TOther>(Mayhap<TOther> other)
        {
            return ZipWith(other, (x, _) => x);
        }

        [Pure]
        public Mayhap<Unit> Skip()
        {
            return ContinueWith(Mayhap.Unit);
        }

        #region ZipWith()

        [Pure]
        public Mayhap<TResult> ZipWith<TOther, TResult>(
            Mayhap<TOther> other, Func<T, TOther, TResult> zipper)
        {
            if (zipper is null) { throw new ArgumentNullException(nameof(zipper)); }

            return Bind(
                x => other.Select(
                    y => zipper(x, y)));
        }

        [Pure]
        public Mayhap<TResult> ZipWith<T1, T2, TResult>(
            Mayhap<T1> first,
            Mayhap<T2> second,
            Func<T, T1, T2, TResult> zipper)
        {
            if (zipper is null) { throw new ArgumentNullException(nameof(zipper)); }

            return Bind(
                x => first.ZipWith(
                    second, (y, z) => zipper(x, y, z)));
        }

        [Pure]
        public Mayhap<TResult> ZipWith<T1, T2, T3, TResult>(
             Mayhap<T1> first,
             Mayhap<T2> second,
             Mayhap<T3> third,
             Func<T, T1, T2, T3, TResult> zipper)
        {
            if (zipper is null) { throw new ArgumentNullException(nameof(zipper)); }

            return Bind(
                x => first.ZipWith(
                    second,
                    third,
                    (y, z, a) => zipper(x, y, z, a)));
        }

        [Pure]
        public Mayhap<TResult> ZipWith<T1, T2, T3, T4, TResult>(
            Mayhap<T1> first,
            Mayhap<T2> second,
            Mayhap<T3> third,
            Mayhap<T4> fourth,
            Func<T, T1, T2, T3, T4, TResult> zipper)
        {
            if (zipper is null) { throw new ArgumentNullException(nameof(zipper)); }

            return Bind(
                x => first.ZipWith(
                    second,
                    third,
                    fourth,
                    (y, z, a, b) => zipper(x, y, z, a, b)));
        }

        #endregion
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
            => Unwrap(x => other.Contains(x), !other._isSome);

        [Pure]
        public bool Equals(Mayhap<T> other, IEqualityComparer<T> comparer)
            => Unwrap(x => other.Contains(x, comparer), !other._isSome);

        [Pure]
        public bool Contains(T value)
            => Unwrap(x => s_DefaultComparer.Equals(x, value), false);

        [Pure]
        public bool Contains(T value, IEqualityComparer<T> comparer)
            => Unwrap(x => (comparer ?? s_DefaultComparer).Equals(x, value), false);

        [Pure]
        public override bool Equals(object? obj)
            => obj is Mayhap<T> maybe && Equals(maybe);

        [Pure]
        public bool Equals(object? other, IEqualityComparer<T> comparer)
            => other is Mayhap<T> maybe && Equals(maybe, comparer);

        [Pure]
        public override int GetHashCode()
            => Unwrap(x => x!.GetHashCode(), 0);

        [Pure]
        public int GetHashCode(IEqualityComparer<T> comparer)
            => Unwrap((comparer ?? s_DefaultComparer).GetHashCode, 0);
    }
}
