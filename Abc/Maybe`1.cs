// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading.Tasks;

    using EF = Utilities.ExceptionFactory;

    // API overview.
    //
    // Properties (no property Value).
    // - None
    // - IsNone
    //
    // Static methods:
    // - Of()
    // - Some()
    //
    // Instance methods.
    // - Bind()
    // - OrElse()
    // - Select()           map the value
    // - Where()            filter / value
    // - SelectMany()
    // - Join()
    // - Apply()
    // - ReplaceWith()
    // - ContinueWith()
    // - PassThru()
    // - Skip()
    // - ZipWith()
    //
    // Safe escapes from a maybe.
    // - Switch()           pattern matching
    // - ValueOrXXX()       unwrap
    // - Do()               side-effects actions
    // - OnSome()           side-effects actions
    // - GetEnumerator()    iterable (implicit)
    // - Yield()            enumerable (explicit)
    // - Contains()         set-like

    // REVIEW: API
    // - ReplaceWith() -> ContinueWith()
    // - Skip(predicate)

    // REVIEW: disposable exts, lazy exts, async exts, nullable attrs, notnull constraints.
    // https://docs.microsoft.com/en-us/dotnet/csharp/nullable-attributes
    // https://devblogs.microsoft.com/dotnet/try-out-nullable-reference-types/
    // IEquatable<T>, but a bit missleading?
    // IComparable? See ValueTuple. IStructuralComparable, IComparable?
    // Serializable?
    // Enhance and improve async methods.
    // Set ops (Union(), IntersectWith(), ...)
    // Struct really? Compare w/ ValueTuple
    // http://mustoverride.com/tuples_structs/
    // https://docs.microsoft.com/en-us/archive/msdn-magazine/2018/june/csharp-tuple-trouble-why-csharp-tuples-get-to-break-the-guidelines

    /// <summary>
    /// Represents an object that is either a single value of type T, or no
    /// value at all.
    /// <para><see cref="Maybe{T}"/> is a read-only struct. Beware, if T is a
    /// mutable reference type, it "infects" this struct too.</para>
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [DebuggerTypeProxy(typeof(Maybe<>.DebugView_))]
    // REVIEW: comparable ops.
    [SuppressMessage("Design", "CA1036:Override methods on comparable types")]
    public readonly partial struct Maybe<T>
        : IEquatable<Maybe<T>>, IStructuralEquatable,
            IComparable<Maybe<T>>, IComparable, IStructuralComparable
    {
        private readonly bool _isSome;

        /// <summary>
        /// Represents the enclosed value.
        /// <para>This field is read-only.</para>
        /// </summary>
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
        /// Checks whether the current instance is "none" or not.
        /// </summary>
        public bool IsNone => !_isSome;

        /// <summary>
        /// Checks whether the current instance does hold a value or not.
        /// </summary>
        /// <remarks>
        /// Most of the time, we don't need to access this property. We are
        /// better off using the rich API that this struct has to offer.
        /// </remarks>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal bool IsSome => _isSome;

        /// <summary>
        /// Gets the enclosed value.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal T Value { get { Debug.Assert(_isSome); return _value; } }

        [ExcludeFromCodeCoverage]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        private string DebuggerDisplay => $"IsSome = {_isSome}";

        /// <summary>
        /// Returns a string representation of the current instance.
        /// </summary>
        [Pure]
        public override string ToString()
            => _isSome ? $"Maybe({_value})" : "Maybe(None)";

        [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Maybe.Flatten()")]
        public static explicit operator Maybe<T>(Maybe<Maybe<T>> maybe)
            => maybe._isSome ? maybe._value : Maybe<T>.None;

        /// <summary>
        /// Represents a debugger type proxy for <see cref="Maybe{T}"/>.
        /// </summary>
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

    // Core methods.
    public partial struct Maybe<T>
    {
        /// <summary>
        /// Obtains an instance of <see cref="Maybe{T}" /> that does not enclose
        /// any value.
        /// <para>This static property is thread-safe.</para>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:Do not declare static members on generic types", Justification = "There is no such thing as a generic static property on a non-generic type.")]
        public static Maybe<T> None { get; } = default;

        [Pure]
        public Maybe<TResult> Bind<TResult>(Func<T, Maybe<TResult>> binder)
        {
            if (binder is null) { throw new ArgumentNullException(nameof(binder)); }

            return _isSome ? binder(_value) : Maybe<TResult>.None;
        }

        // Generalizes the null coalescing operator (??).
        [Pure]
        public Maybe<T> OrElse(Maybe<T> other)
            => _isSome ? this : other;
    }

    // Safe escapes.
    public partial struct Maybe<T>
    {
        // REVIEW: delayed throw?

        /// <summary>
        /// If the current instance encloses a value, it unwraps it using
        /// <paramref name="caseSome"/>, otherwise it executes
        /// <paramref name="caseNone"/>.
        /// </summary>
        [Pure]
        public TResult Switch<TResult>(Func<T, TResult> caseSome, Func<TResult> caseNone)
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

        /// <summary>
        /// If the current instance encloses a value, it unwraps it using
        /// <paramref name="caseSome"/>, otherwise it returns
        /// <paramref name="caseNone"/>.
        /// </summary>
        [Pure]
        [return: NotNullIfNotNull("caseNone")]
        public TResult Switch<TResult>(Func<T, TResult> caseSome, TResult caseNone)
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

        /// <summary>
        /// If the current instance encloses a value, it executes
        /// <paramref name="onSome"/>, otherwise it executes
        /// <paramref name="onNone"/>.
        /// </summary>
        public void Do(Action<T> onSome, Action onNone)
        {
            if (_isSome)
            {
                if (onSome is null) { throw new ArgumentNullException(nameof(onSome)); }
                onSome(_value);
            }
            else
            {
                if (onNone is null) { throw new ArgumentNullException(nameof(onNone)); }
                onNone();
            }
        }

        // We do not provide OnNone(action), it is much simpler to write
        //   if (maybe.IsNone) { action(); }

        /// <summary>
        /// If the current instance encloses a value, it executes
        /// <paramref name="action"/>.
        /// </summary>
        public void OnSome(Action<T> action)
        {
            if (_isSome)
            {
                if (action is null) { throw new ArgumentNullException(nameof(action)); }
                action(_value);
            }
        }

        #region Specialized versions

        /// <summary>
        /// Obtains the enclosed value if any; otherwise this method returns the
        /// default value of the <typeparamref name="T"/> type.
        /// </summary>
        [Pure]
        [return: MaybeNull]
        public T ValueOrDefault()
            => _isSome ? _value : default;

        /// <summary>
        /// Obtains the enclosed value if any; otherwise this method returns
        /// <paramref name="other"/>.
        /// </summary>
        [Pure]
        public T ValueOrElse([DisallowNull]T other)
            => _isSome ? _value : other;

        [Pure]
        public T ValueOrElse(Func<T> valueFactory)
        {
            if (_isSome)
            {
                return _value;
            }
            else
            {
                if (valueFactory is null) { throw new ArgumentNullException(nameof(valueFactory)); }
                return valueFactory();
            }
        }

        [Pure]
        public T ValueOrThrow()
            => _isSome ? _value : throw new InvalidOperationException();

        [Pure]
        public T ValueOrThrow(Func<Exception> exceptionFactory)
        {
            if (_isSome)
            {
                return _value;
            }
            else
            {
                if (exceptionFactory is null) { throw new ArgumentNullException(nameof(exceptionFactory)); }
                throw exceptionFactory();
            }
        }

        #endregion
    }

    // Query Expression Pattern aka LINQ.
    public partial struct Maybe<T>
    {
        [Pure]
        public Maybe<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            if (selector is null) { throw new ArgumentNullException(nameof(selector)); }

            return _isSome ? Maybe.Of(selector(_value)) : Maybe<TResult>.None;
        }

        [Pure]
        public Maybe<T> Where(Func<T, bool> predicate)
        {
            if (predicate is null) { throw new ArgumentNullException(nameof(predicate)); }

            return _isSome && predicate(_value) ? this : None;
        }

        // Generalizes both Bind() and ZipWith<T, TMiddle, TResult>().
        [Pure]
        public Maybe<TResult> SelectMany<TMiddle, TResult>(
            Func<T, Maybe<TMiddle>> selector,
            Func<T, TMiddle, TResult> resultSelector)
        {
            if (selector is null) { throw new ArgumentNullException(nameof(selector)); }
            if (resultSelector is null) { throw new ArgumentNullException(nameof(resultSelector)); }

            if (!_isSome) { return Maybe<TResult>.None; }

            var middle = selector(_value);
            if (!middle._isSome) { return Maybe<TResult>.None; }

            return Maybe.Of(resultSelector(_value, middle._value));
        }

        [Pure]
        public Maybe<TResult> Join<TInner, TKey, TResult>(
            Maybe<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector)
        {
            return Join(inner, outerKeySelector, innerKeySelector, resultSelector, null!);
        }

        [Pure]
        public Maybe<TResult> Join<TInner, TKey, TResult>(
            Maybe<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            if (outerKeySelector is null) { throw new ArgumentNullException(nameof(outerKeySelector)); }
            if (innerKeySelector is null) { throw new ArgumentNullException(nameof(innerKeySelector)); }
            if (resultSelector is null) { throw new ArgumentNullException(nameof(resultSelector)); }

            if (_isSome && inner._isSome)
            {
                var outerKey = outerKeySelector(_value);
                var innerKey = innerKeySelector(inner._value);

                if ((comparer ?? EqualityComparer<TKey>.Default).Equals(outerKey, innerKey))
                {
                    return Maybe.Of(resultSelector(_value, inner._value));
                }
            }

            return Maybe<TResult>.None;
        }

        //
        // GroupJoin currently disabled.
        //

        //[Pure]
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
        //        var outerKey = outerKeySelector(_value);
        //        var innerKey = innerKeySelector(inner._value);

        //        if ((comparer ?? EqualityComparer<TKey>.Default).Equals(outerKey, innerKey))
        //        {
        //            return Maybe.Of(resultSelector(_value, inner));
        //        }
        //    }

        //    return Maybe<TResult>.None;
        //}
    }

    // Async methods.
    public partial struct Maybe<T>
    {
        [Pure]
        public async Task<Maybe<TResult>> BindAsync<TResult>(
            Func<T, Task<Maybe<TResult>>> binder)
        {
            if (binder is null) { throw new ArgumentNullException(nameof(binder)); }

            return _isSome ? await binder(_value).ConfigureAwait(false)
                : Maybe<TResult>.None; ;
        }

        [Pure]
        public async Task<Maybe<TResult>> SelectAsync<TResult>(
            Func<T, Task<TResult>> selector)
        {
            if (selector is null) { throw new ArgumentNullException(nameof(selector)); }

            return _isSome ? Maybe.Of(await selector(_value).ConfigureAwait(false))
                : Maybe<TResult>.None;
        }

        [Pure]
        public async Task<Maybe<T>> OrElseAsync(Task<Maybe<T>> other)
        {
            if (other is null) { throw new ArgumentNullException(nameof(other)); }

            return _isSome ? this : await other.ConfigureAwait(false);
        }

        [Pure]
        public async Task<TResult> SwitchAsync<TResult>(
            Func<T, Task<TResult>> caseSome, Task<TResult> caseNone)
        {
            if (_isSome)
            {
                if (caseSome is null) { throw new ArgumentNullException(nameof(caseSome)); }
                return await caseSome(_value).ConfigureAwait(false);
            }
            else
            {
                if (caseNone is null) { throw new ArgumentNullException(nameof(caseNone)); }
                return await caseNone.ConfigureAwait(false);
            }
        }
    }

    // Misc methods.
    public partial struct Maybe<T>
    {
        [Pure]
        public Maybe<TResult> Apply<TResult>(Maybe<Func<T, TResult>> applicative)
        {
            return _isSome && applicative._isSome ? Maybe.Of(applicative._value(_value))
                : Maybe<TResult>.None;
        }

        [Pure]
        public Maybe<TResult> ReplaceWith<TResult>(TResult value)
            where TResult : notnull
        {
            return _isSome ? Maybe.Of(value) : Maybe<TResult>.None;
        }

        [Pure]
        public Maybe<TResult> ContinueWith<TResult>(Maybe<TResult> other)
        {
            return _isSome ? other : Maybe<TResult>.None;
        }

        [Pure]
        public Maybe<T> PassThru<TOther>(Maybe<TOther> other)
        {
            return other._isSome ? this : None;
        }

        [Pure]
        public Maybe<Unit> Skip()
        {
            return _isSome ? Maybe.Unit : Maybe.None;
        }

        [Pure]
        public Maybe<TResult> ZipWith<TOther, TResult>(
            Maybe<TOther> other,
            Func<T, TOther, TResult> zipper)
        {
            if (zipper is null) { throw new ArgumentNullException(nameof(zipper)); }

            return _isSome && other._isSome
                ? Maybe.Of(zipper(_value, other._value))
                : Maybe<TResult>.None;
        }
    }

    // Iterable but not enumerable.
    // 1) A maybe is a indeed collection but a rather trivial one.
    // 2) Maybe<T> being a struct, I worry about hidden casts.
    // 3) Source of confusion (conflicts?) if we import the System.Linq namespace.
    public partial struct Maybe<T>
    {
        [Pure]
        public IEnumerator<T> GetEnumerator()
            => Yield(1).GetEnumerator();

        [Pure]
        public IEnumerable<T> Yield(int count)
            => _isSome ? Enumerable.Repeat(_value, count) : Enumerable.Empty<T>();

        // REVIEW: Optimize Yield().
        // Beware, infinite loop!
        [Pure]
        public IEnumerable<T> Yield()
        {
            if (_isSome)
            {
                while (true)
                {
                    yield return _value;
                }
            }
        }

        // Maybe<T> being a struct it is never equal to null, therefore
        // Contains(null) always returns false.
        [Pure]
        public bool Contains(T value)
            => _isSome && EqualityComparer<T>.Default.Equals(_value, value);
    }

    // Interface IComparable<>.
    public partial struct Maybe<T>
    {
        public int CompareTo(Maybe<T> other)
            => Comparer<T>.Default.Compare(_value, other._value);

        int IComparable.CompareTo(object? other)
        {
            if (other is null) { return 1; }
            if (!(other is Maybe<T> maybe)) { throw EF.InvalidComparaison(nameof(other)); }

            return Comparer<T>.Default.Compare(_value, maybe._value);
        }

        int IStructuralComparable.CompareTo(object? other, IComparer comparer)
        {
            if (other is null) { return 1; }
            if (!(other is Maybe<T> maybe)) { throw EF.InvalidComparaison(nameof(other)); }

            return (comparer ?? Comparer<T>.Default).Compare(_value, maybe._value);
        }
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
        [Pure]
        public bool Equals(Maybe<T> other)
            => _isSome ? other.Contains(_value) : !other._isSome;

        /// <inheritdoc />
        [Pure]
        public override bool Equals(object? obj)
            => obj is Maybe<T> maybe && Equals(maybe);

        [Pure]
        bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer)
        {
            if (!(other is Maybe<T> maybe)) { return false; }

            return _isSome
                ? maybe._isSome
                    && (comparer ?? EqualityComparer<T>.Default).Equals(_value, maybe._value)
                : !maybe._isSome;
        }

        /// <inheritdoc />
        [Pure]
        public override int GetHashCode()
            => _value?.GetHashCode() ?? 0;

        [Pure]
        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
            => _isSome ? (comparer ?? EqualityComparer<T>.Default).GetHashCode(_value) : 0;
    }
}
