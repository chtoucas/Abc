// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

#if MONADS_PURE
    using Abc.Utilities;
#endif

    // REVIEW: disposable exts, async exts, nullable attrs, notnull constraints.
    // Maybe<T> where T : notnull ??? <- only works if nullable is enabled.
    // https://docs.microsoft.com/en-us/dotnet/csharp/nullable-attributes
    // https://devblogs.microsoft.com/dotnet/try-out-nullable-reference-types/
    // IEquatable<T>?

    // TODO: voir les derniers ajouts dans
    // http://hackage.haskell.org/package/base-4.12.0.0/docs/Control-Monad.html
    // https://downloads.haskell.org/~ghc/latest/docs/html/libraries/base-4.13.0.0/Control-Monad.html
    // https://www.haskell.org/onlinereport/monad.html

    /// <summary>
    /// Represents an object that is either a single value of type T, or no
    /// value at all.
    /// <para><see cref="Maybe{T}"/> is an immutable struct.</para>
    /// </summary>
    /// <typeparam name="T">The underlying type of the value.</typeparam>
    [DebuggerDisplay("IsSome = {IsSome}")]
    [DebuggerTypeProxy(typeof(Maybe<>.DebugView_))]
    [SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix")]
    public readonly partial struct Maybe<T> : IEquatable<Maybe<T>>, IEnumerable<T>
    {
        // We use an explicit backing-field to be able to quickly check that,
        // with MONADS_PURE, IsSome is only used by AssertEx in Abc.Testing.
        private readonly bool _isSome;

        /// <summary>
        /// Represents the enclosed value.
        /// <para>This field is read-only.</para>
        /// </summary>
        // We should NEVER use this field directly, use the property Value instead.
        private readonly T _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Maybe{T}" /> struct
        /// from the specified value.
        /// </summary>
        internal Maybe([DisallowNull]T value)
        {
            Debug.Assert(value != null);

            _isSome = true;
            _value = value;
        }

        /// <summary>
        /// Checks whether the object does hold a value or not.
        /// </summary>
        /// <remarks>
        /// Most of the time, we don't need to access this property. We are
        /// better off using the rich API that this struct has to offer.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#if MONADS_PURE
        [InternalForTesting]
#endif
        internal bool IsSome => _isSome;

        /// <summary>
        /// Gets the enclosed value.
        /// </summary>
        /// <remarks>
        /// Any access to this property MUST be protected by checking before that
        /// <see cref="_isSome"/> is true.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#if MONADS_PURE
        [InternalForTesting]
#endif
        internal T Value { get { Debug.Assert(_isSome); return _value; } }

        /// <summary>
        /// Returns a string representation of the current instance.
        /// </summary>
        public override string ToString()
            => _isSome ? $"Maybe({Value})" : "Maybe(None)";

        #region Core monadic methods

        // Maybe<T> is a Monad:
        // - Maybe.Of() aka Maybe<T>.η(), or simply the ctor
        // - Maybe.Flatten() aka Maybe<T>.μ()
        // - Maybe<T>.Bind()
        // Maybe<T> is a MonadOr:
        // - Maybe<T>.None
        // - Maybe<T>.OrElse()

        /// <summary>
        /// Obtains an instance of <see cref="Maybe{T}" /> that does not enclose
        /// any value.
        /// <para>This static property is thread-safe.</para>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:Do not declare static members on generic types", Justification = "There is no such thing as a generic static property on a non-generic type.")]
        public static Maybe<T> None { get; } = default;

        public Maybe<TResult> Bind<TResult>(Func<T, Maybe<TResult>> binder)
        {
            if (binder is null) { throw new ArgumentNullException(nameof(binder)); }

            return _isSome ? binder(Value) : Maybe<TResult>.None;
        }

#if MONADS_PURE

        [DebuggerHidden]
        internal static Maybe<T> μ(Maybe<Maybe<T>> square)
            => square._isSome ? square.Value : None;

#endif

        public Maybe<T> OrElse(Maybe<T> other)
            => _isSome ? this : other;

        #endregion

        /// <summary>
        /// Represents a debugger type proxy for <see cref="Maybe{T}"/>.
        /// </summary>
        /// <remarks>
        /// Ensures that <see cref="Maybe{T}.Value"/> does NOT throw in the
        /// debugger.
        /// </remarks>
        [ExcludeFromCodeCoverage]
        [SuppressMessage("Microsoft.Design", "CA1812:Avoid uninstantiated internal classes", Justification = "DebuggerTypeProxy")]
        private sealed class DebugView_
        {
            private readonly Maybe<T> _inner;

            public DebugView_(Maybe<T> inner) { _inner = inner; }

            public bool IsSome => _inner._isSome;

            public T Value => _inner._value;
        }
    }

    // Pattern matching to escape the monad.
    public partial struct Maybe<T>
    {
        // REVIEW: delayed throw?

        /// <summary>
        /// If the current instance encloses a value, it executes
        /// <paramref name="some"/>, otherwise it executes
        /// <paramref name="none"/>.
        /// </summary>
        public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none)
        {
            if (_isSome)
            {
                if (some is null) { throw new ArgumentNullException(nameof(some)); }
                return some(Value);
            }
            else
            {
                if (none is null) { throw new ArgumentNullException(nameof(none)); }
                return none();
            }
        }

        /// <summary>
        /// If the current instance encloses a value, it executes
        /// <paramref name="onSome"/>, otherwise it executes
        /// <paramref name="onNone"/>.
        /// </summary>
        public void Match(Action<T> onSome, Action onNone)
        {
#if MONADS_PURE
            Match(__caseSome, __caseNone);

            Unit __caseSome(T x) { onSome(x); return Unit.Default; }
            Unit __caseNone() { onNone(); return Unit.Default; }
#else
            if (_isSome)
            {
                if (onSome is null) { throw new ArgumentNullException(nameof(onSome)); }
                onSome(Value);
            }
            else
            {
                if (onNone is null) { throw new ArgumentNullException(nameof(onNone)); }
                onNone();
            }
#endif
        }

        /// <summary>
        /// If the current instance encloses a value, it executes
        /// <paramref name="action"/>.
        /// </summary>
        public void OnSome(Action<T> action)
        {
#if MONADS_PURE
            Match(action, Stubs.Noop);
#else
            if (_isSome)
            {
                if (action is null) { throw new ArgumentNullException(nameof(action)); }
                action(Value);
            }
#endif
        }

        /// <summary>
        /// If the current instance does NOT enclose a value, it executes
        /// <paramref name="action"/>.
        /// </summary>
        public void OnNone(Action action)
        {
#if MONADS_PURE
            Match(Stubs<T>.Ignore, action);
#else
            if (!_isSome)
            {
                if (action is null) { throw new ArgumentNullException(nameof(action)); }
                action();
            }
#endif
        }

        #region Specialized versions

        /// <summary>
        /// Obtains the enclosed value if any; otherwise this method returns the
        /// default value of the <typeparamref name="T"/> type.
        /// </summary>
        [return: MaybeNull]
        public T ValueOrDefault()
#if MONADS_PURE
            => Match(Stubs<T>.Ident, Stubs<T>.ReturnsDefault);
#else
            => _isSome ? Value : default;
#endif

        /// <summary>
        /// Obtains the enclosed value if any; otherwise this method returns
        /// <paramref name="other"/>.
        /// </summary>
        public T ValueOrElse([DisallowNull]T other)
#if MONADS_PURE
            => Match(Stubs<T>.Ident, () => other);
#else
            => _isSome ? Value : other;
#endif

        public T ValueOrElse(Func<T> valueFactory)
        {
#if MONADS_PURE
            return Match(Stubs<T>.Ident, __caseNone);

            T __caseNone()
            {
                if (valueFactory is null) { throw new ArgumentNullException(nameof(valueFactory)); }
                return valueFactory();
            }
#else
            if (_isSome)
            {
                return Value;
            }
            else
            {
                if (valueFactory is null) { throw new ArgumentNullException(nameof(valueFactory)); }
                return valueFactory();
            }
#endif
        }

        public T ValueOrThrow()
#if MONADS_PURE
            => Match(Stubs<T>.Ident, () => throw new InvalidOperationException());
#else
            => _isSome ? Value : throw new InvalidOperationException();
#endif

        public T ValueOrThrow(Func<Exception> exceptionFactory)
        {
#if MONADS_PURE
            return Match(Stubs<T>.Ident, __caseNone);

            T __caseNone()
            {
                if (exceptionFactory is null) { throw new ArgumentNullException(nameof(exceptionFactory)); }
                throw exceptionFactory(); ;
            }
#else
            if (_isSome)
            {
                return Value;
            }
            else
            {
                if (exceptionFactory is null) { throw new ArgumentNullException(nameof(exceptionFactory)); }
                throw exceptionFactory();
            }
#endif
        }

        public bool Contains(T value)
#if MONADS_PURE
            => Match(x => EqualityComparer<T>.Default.Equals(x, value), Predicates.False);
#else
            => _isSome && EqualityComparer<T>.Default.Equals(Value, value);
#endif

        public bool Contains(T value, IEqualityComparer<T> comparer)
#if MONADS_PURE
            => Match(x => (comparer ?? EqualityComparer<T>.Default).Equals(x, value), Predicates.False);
#else
            => _isSome && (comparer ?? EqualityComparer<T>.Default).Equals(Value, value);
#endif

        #endregion
    }

    // Interface IEnumerable<>.
    public partial struct Maybe<T>
    {
        // REVIEW: IEnumerable<T> or not?

        public IEnumerator<T> GetEnumerator()
        {
            if (_isSome)
            {
                yield return Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }

    // Standard API.
    public partial struct Maybe<T>
    {
        public Maybe<TResult> ReplaceWith<TResult>(TResult value)
            where TResult : notnull
        {
#if MONADS_PURE
            return Select(_ => value);
#else
            return _isSome ? Maybe.Of(value) : Maybe<TResult>.None;
#endif
        }

        public Maybe<TResult> ContinueWith<TResult>(Maybe<TResult> other)
        {
#if MONADS_PURE
            return Bind(_ => other);
#else
            return _isSome ? other : Maybe<TResult>.None;
#endif
        }

        public Maybe<T> PassThru<TOther>(Maybe<TOther> other)
        {
#if MONADS_PURE
            return ZipWith(other, (x, _) => x);
#else
            return other._isSome ? this : None;
#endif
        }

        // REVIEW: Skip(predicate).
        public Maybe<Unit> Skip()
        {
#if MONADS_PURE
            return ContinueWith(Maybe.Unit);
#else
            return _isSome ? Maybe.Unit : Maybe.None;
#endif
        }

        #region ZipWith()

        public Maybe<TResult> ZipWith<TOther, TResult>(
            Maybe<TOther> other, Func<T, TOther, TResult> zipper)
        {
            if (zipper is null) { throw new ArgumentNullException(nameof(zipper)); }

#if MONADS_PURE
            return Bind(
                x => other.Select(
                    y => zipper(x, y)));
#else
            return _isSome && other._isSome
                ? Maybe.Of(zipper(Value, other.Value))
                : Maybe<TResult>.None;
#endif
        }

        public Maybe<TResult> ZipWith<T1, T2, TResult>(
            Maybe<T1> first,
            Maybe<T2> second,
            Func<T, T1, T2, TResult> zipper)
        {
            if (zipper is null) { throw new ArgumentNullException(nameof(zipper)); }

#if MONADS_PURE
            return Bind(
                x => first.ZipWith(
                    second, (y, z) => zipper(x, y, z)));
#else
            return _isSome && first._isSome && second._isSome
                ? Maybe.Of(zipper(Value, first.Value, second.Value))
                : Maybe<TResult>.None;
#endif
        }

        public Maybe<TResult> ZipWith<T1, T2, T3, TResult>(
             Maybe<T1> first,
             Maybe<T2> second,
             Maybe<T3> third,
             Func<T, T1, T2, T3, TResult> zipper)
        {
            if (zipper is null) { throw new ArgumentNullException(nameof(zipper)); }

#if MONADS_PURE
            return Bind(
                x => first.ZipWith(
                    second,
                    third,
                    (y, z, a) => zipper(x, y, z, a)));
#else
            return _isSome && first._isSome && second._isSome && third._isSome
                ? Maybe.Of(zipper(Value, first.Value, second.Value, third.Value))
                : Maybe<TResult>.None;
#endif
        }

        public Maybe<TResult> ZipWith<T1, T2, T3, T4, TResult>(
            Maybe<T1> first,
            Maybe<T2> second,
            Maybe<T3> third,
            Maybe<T4> fourth,
            Func<T, T1, T2, T3, T4, TResult> zipper)
        {
            if (zipper is null) { throw new ArgumentNullException(nameof(zipper)); }

#if MONADS_PURE
            return Bind(
                x => first.ZipWith(
                    second,
                    third,
                    fourth,
                    (y, z, a, b) => zipper(x, y, z, a, b)));
#else
            return _isSome && first._isSome && second._isSome && third._isSome && fourth._isSome
                ? Maybe.Of(zipper(Value, first.Value, second.Value, third.Value, fourth.Value))
                : Maybe<TResult>.None;
#endif
        }

        #endregion

        #region Query Expression Pattern

        public Maybe<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            if (selector is null) { throw new ArgumentNullException(nameof(selector)); }

#if MONADS_PURE
            return Bind(x => Maybe.Of(selector(x)));
#else
            return _isSome ? Maybe.Of(selector(Value)) : Maybe<TResult>.None;
#endif
        }

        public Maybe<T> Where(Func<T, bool> predicate)
        {
            if (predicate is null) { throw new ArgumentNullException(nameof(predicate)); }

#if MONADS_PURE
            // NB: x is never null.
            return Bind(x => predicate(x) ? new Maybe<T>(x) : None);
#else
            return _isSome && predicate(Value) ? this : None;
#endif
        }

        // Generalizes both Bind() and ZipWith<T, TMiddle, TResult>().
        public Maybe<TResult> SelectMany<TMiddle, TResult>(
            Func<T, Maybe<TMiddle>> selector,
            Func<T, TMiddle, TResult> resultSelector)
        {
            if (selector is null) { throw new ArgumentNullException(nameof(selector)); }
            if (resultSelector is null) { throw new ArgumentNullException(nameof(resultSelector)); }

#if MONADS_PURE
            return Bind(
                x => selector(x).Select(
                    middle => resultSelector(x, middle)));
#else
            if (!_isSome) { return Maybe<TResult>.None; }

            var middle = selector(Value);
            if (!middle._isSome) { return Maybe<TResult>.None; }

            return Maybe.Of(resultSelector(Value, middle.Value));
#endif
        }

        public Maybe<TResult> Join<TInner, TKey, TResult>(
            Maybe<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector)
        {
            return Join(inner, outerKeySelector, innerKeySelector, resultSelector, null);
        }

        public Maybe<TResult> Join<TInner, TKey, TResult>(
            Maybe<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector,
            IEqualityComparer<TKey>? comparer)
        {
            if (outerKeySelector is null) { throw new ArgumentNullException(nameof(outerKeySelector)); }
            if (innerKeySelector is null) { throw new ArgumentNullException(nameof(innerKeySelector)); }
            if (resultSelector is null) { throw new ArgumentNullException(nameof(resultSelector)); }

#if MONADS_PURE
            var keyLookup = __getKeyLookup(inner, innerKeySelector, comparer);

            return SelectMany(__valueSelector, resultSelector);

            Maybe<TInner> __valueSelector(T outer) => keyLookup(outerKeySelector(outer));

            static Func<TKey, Maybe<TInner>> __getKeyLookup(
               Maybe<TInner> inner,
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
#else
            if (_isSome && inner._isSome)
            {
                var outerKey = outerKeySelector(Value);
                var innerKey = innerKeySelector(inner.Value);

                if ((comparer ?? EqualityComparer<TKey>.Default).Equals(outerKey, innerKey))
                {
                    return Maybe.Of(resultSelector(Value, inner.Value));
                }
            }

            return Maybe<TResult>.None;
#endif
        }

        //
        // GroupJoin currently disabled.
        //

        //public Maybe<TResult> GroupJoin<TInner, TKey, TResult>(
        //    Maybe<TInner> inner,
        //    Func<T, TKey> outerKeySelector,
        //    Func<TInner, TKey> innerKeySelector,
        //    Func<T, Maybe<TInner>, TResult> resultSelector,
        //    IEqualityComparer<TKey> comparer)
        //{
        //    if (outerKeySelector is null) { throw new ArgumentNullException(nameof(outerKeySelector)); }
        //    if (innerKeySelector is null) { throw new ArgumentNullException(nameof(innerKeySelector)); }
        //    if (resultSelector is null) { throw new ArgumentNullException(nameof(resultSelector)); }

        //    if (_isSome && inner._isSome)
        //    {
        //        var outerKey = outerKeySelector(Value);
        //        var innerKey = innerKeySelector(inner.Value);

        //        if ((comparer ?? EqualityComparer<TKey>.Default).Equals(outerKey, innerKey))
        //        {
        //            return Maybe.Of(resultSelector(Value, inner));
        //        }
        //    }

        //    return Maybe<TResult>.None;
        //}

        #endregion

        #region Async methods

        public async Task<Maybe<TResult>> BindAsync<TResult>(
            Func<T, Task<Maybe<TResult>>> binder)
        {
            if (binder is null) { throw new ArgumentNullException(nameof(binder)); }

            return _isSome ? await binder(Value).ConfigureAwait(false)
                : Maybe<TResult>.None; ;
        }

        public async Task<Maybe<TResult>> SelectAsync<TResult>(
            Func<T, Task<TResult>> selector)
        {
            if (selector is null) { throw new ArgumentNullException(nameof(selector)); }

            return _isSome ? Maybe.Of(await selector(Value).ConfigureAwait(false))
                : Maybe<TResult>.None;
        }

        #endregion
    }

    // Interface IEquatable<>.
    public partial struct Maybe<T>
    {
        /// <summary>
        /// Determines whether two specified instances of <see cref="Maybe{T}"/>
        /// are equal.
        /// </summary>
        public static bool operator ==(Maybe<T> left, Maybe<T> right)
            => left.Equals(right);

        /// <summary>
        /// Determines whether two specified instances of <see cref="Maybe{T}"/>
        /// are not equal.
        /// </summary>
        public static bool operator !=(Maybe<T> left, Maybe<T> right)
            => !left.Equals(right);

        /// <summary>
        /// Determines whether this instance is equal to the value of the
        /// specified <see cref="Maybe{T}"/>.
        /// </summary>
        public bool Equals(Maybe<T> other)
            => _isSome
              ? other._isSome
                  && EqualityComparer<T>.Default.Equals(Value, other.Value)
              : !other._isSome;

        public bool Equals(Maybe<T> other, IEqualityComparer<T> comparer)
            => _isSome
                ? other._isSome
                    && (comparer ?? EqualityComparer<T>.Default).Equals(Value, other.Value)
                : !other._isSome;

        /// <inheritdoc />
        public override bool Equals(object obj)
            => obj is Maybe<T> maybe && Equals(maybe);

        public bool Equals(object other, IEqualityComparer<T> comparer)
            => other is Maybe<T> maybe && Equals(maybe, comparer);

        /// <inheritdoc />
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;

        public int GetHashCode(IEqualityComparer<T> comparer)
            => _isSome
                ? (comparer ?? EqualityComparer<T>.Default).GetHashCode(Value)
                : 0;
    }
}
