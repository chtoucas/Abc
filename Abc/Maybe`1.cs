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

    using Abc.Utilities;

    using Anexn = System.ArgumentNullException;
    using EF = Abc.Utilities.ExceptionFactory;

    // Overview.
    //
    // The structure Maybe<T> is an option type for C#.
    // The intended usage for it is when T is a value type, a string,
    // a (read-only?) record, or a function. For other reference types,
    // it should be fine as long as T is an **immutable** reference type.
    //
    // Static properties.
    // - Maybe<T>.None
    // - Maybe.None
    // - Maybe.Unit
    //
    // Instance properties (no public access to the enclosed value, ie no property Value).
    // - IsNone
    //
    // Static factories (no public ctor).
    // - Maybe.Of()
    // - Maybe.Some()       specialized form of Of() but for value types
    // - Maybe.Guard()
    //
    // Instance methods where the result is another maybe.
    // - Bind()
    // - Select()           LINQ select
    // - SelectMany()       LINQ select many
    // - Where()            LINQ filter
    // - Join()             LINQ join
    // - OrElse()           coalescing
    // - ZipWith()
    // - Apply()
    // - ReplaceWith()
    // - ContinueWith()
    // - PassThru()
    // - Skip()
    // - Replicate()
    //
    // Safe escapes from a maybe.
    // - Switch()           pattern matching
    // - ValueOrXXX()       unwrap
    // - Do()               side-effects actions
    // - OnSome()           side-effects actions
    // - GetEnumerator()    iterable (implicit)
    // - Yield()            enumerable (explicit)
    // - Contains()         singleton or empty set

    // REVIEW: disposable exts, lazy exts, async exts, nullable attrs, notnull constraints.
    // https://docs.microsoft.com/en-us/dotnet/csharp/nullable-attributes
    // https://devblogs.microsoft.com/dotnet/try-out-nullable-reference-types/
    // IEquatable<T>, IComparable<T> but a bit missleading?
    // Serializable?
    // More LINQ?
    // Enhance and improve async methods.
    // Set ops (Union(), IntersectWith(), ...)
    // Struct really? Compare w/ ValueTuple
    // http://mustoverride.com/tuples_structs/
    // https://docs.microsoft.com/en-us/archive/msdn-magazine/2018/june/csharp-tuple-trouble-why-csharp-tuples-get-to-break-the-guidelines
    //
    // API
    // - ReplaceWith() -> ContinueWith()
    // - Skip(predicate)

    /// <summary>
    /// Represents an object that is either a single value of type T, or no
    /// value at all.
    /// <para><see cref="Maybe{T}"/> is a read-only struct.</para>
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [DebuggerTypeProxy(typeof(Maybe<>.DebugView_))]
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
        /// Checks whether the current instance is empty or not.
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
            if (binder is null) { throw new Anexn(nameof(binder)); }

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
                if (caseSome is null) { throw new Anexn(nameof(caseSome)); }
                return caseSome(_value);
            }
            else
            {
                if (caseNone is null) { throw new Anexn(nameof(caseNone)); }
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
                if (caseSome is null) { throw new Anexn(nameof(caseSome)); }
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
                if (onSome is null) { throw new Anexn(nameof(onSome)); }
                onSome(_value);
            }
            else
            {
                if (onNone is null) { throw new Anexn(nameof(onNone)); }
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
                if (action is null) { throw new Anexn(nameof(action)); }
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
                if (valueFactory is null) { throw new Anexn(nameof(valueFactory)); }
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
                if (exceptionFactory is null) { throw new Anexn(nameof(exceptionFactory)); }
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
            if (selector is null) { throw new Anexn(nameof(selector)); }

            return _isSome ? Maybe.Of(selector(_value)) : Maybe<TResult>.None;
        }

        [Pure]
        public Maybe<T> Where(Func<T, bool> predicate)
        {
            if (predicate is null) { throw new Anexn(nameof(predicate)); }

            return _isSome && predicate(_value) ? this : None;
        }

        // Generalizes both Bind() and ZipWith<T, TMiddle, TResult>().
        [Pure]
        public Maybe<TResult> SelectMany<TMiddle, TResult>(
            Func<T, Maybe<TMiddle>> selector,
            Func<T, TMiddle, TResult> resultSelector)
        {
            if (selector is null) { throw new Anexn(nameof(selector)); }
            if (resultSelector is null) { throw new Anexn(nameof(resultSelector)); }

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
            if (outerKeySelector is null) { throw new Anexn(nameof(outerKeySelector)); }
            if (innerKeySelector is null) { throw new Anexn(nameof(innerKeySelector)); }
            if (resultSelector is null) { throw new Anexn(nameof(resultSelector)); }

            return JoinImpl(
                inner, outerKeySelector, innerKeySelector, resultSelector,
                EqualityComparer<TKey>.Default);
        }

        [Pure]
        public Maybe<TResult> Join<TInner, TKey, TResult>(
            Maybe<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            if (outerKeySelector is null) { throw new Anexn(nameof(outerKeySelector)); }
            if (innerKeySelector is null) { throw new Anexn(nameof(innerKeySelector)); }
            if (resultSelector is null) { throw new Anexn(nameof(resultSelector)); }
            if (comparer is null) { throw new Anexn(nameof(comparer)); }

            return JoinImpl(
                inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
        }

        [Pure]
        private Maybe<TResult> JoinImpl<TInner, TKey, TResult>(
            Maybe<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            if (_isSome && inner._isSome)
            {
                var outerKey = outerKeySelector(_value);
                var innerKey = innerKeySelector(inner._value);

                if (comparer.Equals(outerKey, innerKey))
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
        //    if (outerKeySelector is null) { throw new Anexn(nameof(outerKeySelector)); }
        //    if (innerKeySelector is null) { throw new Anexn(nameof(innerKeySelector)); }
        //    if (resultSelector is null) { throw new Anexn(nameof(resultSelector)); }
        //    if (comparer is null) { throw new Anexn(nameof(comparer)); }

        //    if (_isSome && inner._isSome)
        //    {
        //        var outerKey = outerKeySelector(_value);
        //        var innerKey = innerKeySelector(inner._value);

        //        if (comparer.Equals(outerKey, innerKey))
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
            if (binder is null) { throw new Anexn(nameof(binder)); }

            return _isSome ? await binder(_value).ConfigureAwait(false)
                : Maybe<TResult>.None; ;
        }

        [Pure]
        public async Task<Maybe<TResult>> SelectAsync<TResult>(
            Func<T, Task<TResult>> selector)
        {
            if (selector is null) { throw new Anexn(nameof(selector)); }

            return _isSome ? Maybe.Of(await selector(_value).ConfigureAwait(false))
                : Maybe<TResult>.None;
        }

        [Pure]
        public async Task<Maybe<T>> OrElseAsync(Task<Maybe<T>> other)
        {
            if (other is null) { throw new Anexn(nameof(other)); }

            return _isSome ? this : await other.ConfigureAwait(false);
        }

        [Pure]
        public async Task<TResult> SwitchAsync<TResult>(
            Func<T, Task<TResult>> caseSome, Task<TResult> caseNone)
        {
            if (_isSome)
            {
                if (caseSome is null) { throw new Anexn(nameof(caseSome)); }
                return await caseSome(_value).ConfigureAwait(false);
            }
            else
            {
                if (caseNone is null) { throw new Anexn(nameof(caseNone)); }
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

        // Compare to the nullable equiv w/ int? where y is an int:
        //   (x.HasValue ? (int?)y : (int?)null).
        [Pure]
        public Maybe<TResult> ReplaceWith<TResult>(TResult value)
            where TResult : notnull
        {
            return _isSome ? Maybe.Of(value) : Maybe<TResult>.None;
        }

        // Compare to the nullable equiv w/ int? where y is an int?:
        //   (x.HasValue ? y : (int?)null).
        [Pure]
        public Maybe<TResult> ContinueWith<TResult>(Maybe<TResult> other)
        {
            return _isSome ? other : Maybe<TResult>.None;
        }

        // Compare to the nullable equiv w/ int? where (y:int?)
        //   (y.HasValue ? x : (int?)null).
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

        // See also Yield(count).
        [Pure]
        public Maybe<IEnumerable<T>> Replicate(int count)
            => _isSome ? new Maybe<IEnumerable<T>>(Enumerable.Repeat(_value, count))
                : Maybe.Empty<T>();

        // See also Yield(). Beware, infinite loop!
        [Pure]
        public Maybe<IEnumerable<T>> Replicate()
            => _isSome ? new Maybe<IEnumerable<T>>(Sequence.Forever(_value))
                : Maybe.Empty<T>();

        [Pure]
        public Maybe<TResult> ZipWith<TOther, TResult>(
            Maybe<TOther> other,
            Func<T, TOther, TResult> zipper)
        {
            if (zipper is null) { throw new Anexn(nameof(zipper)); }

            return _isSome && other._isSome
                ? Maybe.Of(zipper(_value, other._value))
                : Maybe<TResult>.None;
        }
    }

    // Iterable but not IEnumerable<>.
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

        // Beware, infinite loop!
        [Pure]
        public IEnumerable<T> Yield()
            => _isSome ? Sequence.Forever(_value) : Enumerable.Empty<T>();

        // Maybe<T> being a struct it is never equal to null, therefore
        // Contains(null) always returns false.
        [Pure]
        public bool Contains(T value)
            => _isSome && EqualityComparer<T>.Default.Equals(_value, value);
    }

    // Interface IComparable<>.
    public partial struct Maybe<T>
    {
        /// <summary>
        /// Compares the two specified instances to see if the left one is
        /// strictly less than the right one.
        /// </summary>
        public static bool operator <(Maybe<T> left, Maybe<T> right)
            => left.CompareTo(right) < 0;

        /// <summary>
        /// Compares the two specified instances to see if the left one is
        /// less than or equal to the right one.
        /// </summary>
        public static bool operator <=(Maybe<T> left, Maybe<T> right)
            => left.CompareTo(right) <= 0;

        /// <summary>
        /// Compares the two specified instances to see if the left one is
        /// strictly greater than the right one.
        /// </summary>
        public static bool operator >(Maybe<T> left, Maybe<T> right)
            => left.CompareTo(right) > 0;

        /// <summary>
        /// Compares the two specified instances to see if the left one is
        /// greater than or equal to the right one.
        /// </summary>
        public static bool operator >=(Maybe<T> left, Maybe<T> right)
            => left.CompareTo(right) >= 0;

        /// <summary>
        /// Compares this instance to a specified <see cref="Maybe{T}"/> object.
        /// </summary>
        /// <remarks>
        /// The convention is that the empty maybe is strictly less than any
        /// other maybe.
        /// </remarks>
        public int CompareTo(Maybe<T> other)
            => _isSome
                ? other._isSome ? Comparer<T>.Default.Compare(_value, other._value) : 1
                : other._isSome ? -1 : 0;

        int IComparable.CompareTo(object? obj)
        {
            if (obj is null) { return 1; }
            if (!(obj is Maybe<T> maybe))
            {
                throw EF.InvalidType(nameof(obj), typeof(Maybe<>), obj);
            }

            return CompareTo(maybe);
        }

        int IStructuralComparable.CompareTo(object? other, IComparer comparer)
        {
            if (other is null) { return 1; }
            if (!(other is Maybe<T> maybe))
            {
                throw EF.InvalidType(nameof(other), typeof(Maybe<>), other);
            }
            if (comparer is null) { throw new Anexn(nameof(comparer)); }

            return _isSome
                ? maybe._isSome ? comparer.Compare(_value, maybe._value) : 1
                : maybe._isSome ? -1 : 0;
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
        /// Determines whether this instance is equal to the specified
        /// <see cref="Maybe{T}"/>.
        /// </summary>
        [Pure]
        public bool Equals(Maybe<T> other)
            => _isSome ? other.Contains(_value) : !other._isSome;

        /// <inheritdoc />
        [Pure]
        public override bool Equals(object? obj)
            => obj is Maybe<T> maybe && Equals(maybe);

        /// <inheritdoc />
        [Pure]
        bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer)
        {
            if (other is null || !(other is Maybe<T> maybe)) { return false; }
            if (comparer is null) { throw new Anexn(nameof(comparer)); }

            return _isSome ? maybe._isSome && comparer.Equals(_value, maybe._value)
                : !maybe._isSome;
        }

        /// <inheritdoc />
        [Pure]
        public override int GetHashCode()
            => _value?.GetHashCode() ?? 0;

        /// <inheritdoc />
        [Pure]
        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            if (comparer is null) { throw new Anexn(nameof(comparer)); }

            return _isSome ? comparer.GetHashCode(_value) : 0;
        }
    }
}
