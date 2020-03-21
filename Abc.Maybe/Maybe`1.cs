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
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using Abc.Utilities;

    using Anexn = System.ArgumentNullException;
    using EF = Abc.Utilities.ExceptionFactory;

    // REVIEW: Maybe type
    // - nullable attrs, notnull constraint.
    //   It would make a lot of sense to add a notnull constraint on the T of
    //   Maybe<T>, but it worries me a bit (I need to gain more experience with
    //   the new NRT). It would allow to warn a user trying to create a
    //   Maybe<int?> or a Maybe<string?>, maybe I managed to entirely avoid the
    //   ability to do so, but I am not sure.
    //   https://docs.microsoft.com/en-us/dotnet/csharp/nullable-attributes
    //   https://devblogs.microsoft.com/dotnet/try-out-nullable-reference-types/
    //   https://devblogs.microsoft.com/dotnet/nullable-reference-types-in-csharp/
    //   https://devblogs.microsoft.com/dotnet/embracing-nullable-reference-types/
    // - IEquatable<T> (T == Maybe<T>), IComparable<T> but a bit missleading?
    //   Overloads w/ IEqualityComparer<T>.
    // - Move Join() and GroupJoin() to Maybe? We need compelling examples.
    // - Serializable? Binary serialization only.
    //   https://docs.microsoft.com/en-us/dotnet/standard/serialization/binary-serialization
    // - Set ops POV.
    // - Struct really? Explain and compare to ValueTuple
    //   https://docs.microsoft.com/en-gb/dotnet/csharp/tuples
    //   http://mustoverride.com/tuples_structs/
    //   https://docs.microsoft.com/en-us/archive/msdn-magazine/2018/june/csharp-tuple-trouble-why-csharp-tuples-get-to-break-the-guidelines
    //   https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/value-options
    //   https://github.com/fsharp/fslang-design/blob/master/FSharp.Core-4.5.0.0/FS-1057-valueoption.md

    /// <summary>
    /// Represents an object that is either a single value of type
    /// <typeparamref name="T"/>, or no value at all.
    /// <para><see cref="Maybe{T}"/> is an immutable struct (but see caveats
    /// in the section remarks).</para>
    /// </summary>
    ///
    /// <_text><![CDATA[
    /// Overview.
    ///
    /// The structure Maybe<T> is an option type for C#.
    ///
    /// The intended usage is when T is a value type, a string, a (read-only?)
    /// record, or a function. For other reference types, it should be fine as
    /// long as T is an **immutable** reference type.
    ///
    /// Static properties.
    /// - Maybe<T>.None         the empty maybe of type T
    /// - Maybe.None            the empty maybe of type Unit
    /// - Maybe.Unit            the unit for Maybe<T>
    ///
    /// Instance properties.
    /// - IsNone                is this the empty maybe?
    ///
    /// Static factories (no public ctor).
    /// - Maybe.None<T>()       the empty maybe of type T
    /// - Maybe.Some()          factory method for value types
    /// - Maybe.SomeOrNone()    factory method for nullable value or reference types
    /// - Maybe.Of()            unconstrained factory method
    /// - Maybe.Guard()
    ///
    /// Operators:
    /// - equality == and !=
    /// - comparison <, >, <=, and >=
    /// - bitwise logical |, & and ^ (and compound assignment |=, &= and ^=)
    /// - conversions from and to the underlying type T
    ///
    /// Instance methods where the result is another maybe.
    /// - Bind()                unwrap then map to another maybe
    /// - Select()              LINQ select
    /// - SelectMany()          LINQ select many
    /// - Where()               LINQ filter
    /// - Join()                LINQ join
    /// - GroupJoin()           LINQ group join
    /// - ZipWith()             cross join
    /// - OrElse()              logical OR; "none"-coalescing
    /// - AndThen()             logical AND
    /// - XorElse()             logical XOR
    /// - Skip()
    /// - Replicate()
    /// - Duplicate()
    ///
    /// Escape the maybe (no public access to the enclosed value if any,
    /// ie no property Value).
    /// - Switch()              pattern matching
    /// - TryGetValue()         try unwrap
    /// - ValueOrDefault()      unwrap
    /// - ValueOrElse()         unwrap if possible, otherwise use a replacement
    /// - ValueOrthrow()        unwrap if possible, otherwise throw
    ///
    /// Set and enumerable related methods.
    /// - GetEnumerator()       iterable (implicit)
    /// - ToEnumerable()        convert to an enumerable
    /// - Yield()               enumerable (explicit)
    /// - Contains()            singleton or empty set?
    ///
    /// Side effects.
    /// - Do()
    /// - OnSome()
    /// - When()
    ///
    /// Async versions of the core methods.
    /// - BindAsync()           async binding
    /// - SelectAsync()         async mapping
    /// - OrElseAsync()         async coalescing
    /// - SwitchAsync()         async pattern matching
    ///
    /// We also have several extension methods for specific types of T, eg
    /// structs, functions or enumerables; see the static class Maybe.
    /// ]]></_text>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [DebuggerTypeProxy(typeof(Maybe<>.DebugView_))]
    public readonly partial struct Maybe<T>
        : IEquatable<Maybe<T>>, IStructuralEquatable,
            IComparable<Maybe<T>>, IComparable, IStructuralComparable
    {
        // We use explicit backing fields to find quickly all occurences of the
        // corresponding properties outside the struct.

        private readonly bool _isSome;

        /// <summary>
        /// Represents the enclosed value.
        /// <para>This field is read-only.</para>
        /// </summary>
        [MaybeNull] [AllowNull] private readonly T _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Maybe{T}" /> struct
        /// from the specified value.
        /// </summary>
        internal Maybe([DisallowNull] T value)
        {
            Debug.Assert(value != null);

            _isSome = true;
            _value = value;
        }

        /// <summary>
        /// Checks whether the current instance is empty or not.
        /// </summary>
        // We expose this property to ease extensibility, see MaybeEx in "play",
        // but this not mandatory, in fact everything should work fine without
        // it.
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
        /// <para>You MUST check IsSome before calling this property.</para>
        /// </summary>
        // REVIEW: [MaybeNull]
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

#pragma warning disable CA2225 // Operator overloads have named alternates

        // Implicit conversion to Maybe<T> for equality comparison, very much
        // like what we have will nullable values: (int?)1 == 1 works.
        // Friendly name: Maybe.Of(value).
        // NB: maybe = Some(x) == y is equivalent to maybe.Contains(y).
        public static implicit operator Maybe<T>([AllowNull] T value)
            => Maybe.Of(value);

        // REVIEW: explicit conversion.
        // Friendly name: value.ValueOrThrow().
        // ??? exception or null ???
        // with null, we can write maybe == null, which is odd for a struct
        // but at the same time we can write Maybe<string> maybe = s where s is
        // in fact "null".
        [return: MaybeNull]
        public static explicit operator T(Maybe<T> value)
            => value._isSome ? value.Value : throw new InvalidCastException();

#pragma warning restore CA2225

        /// <summary>
        /// Represents a debugger type proxy for <see cref="Maybe{T}"/>.
        /// </summary>
        [ExcludeFromCodeCoverage]
        private sealed class DebugView_
        {
            private readonly Maybe<T> _inner;

            public DebugView_(Maybe<T> inner) { _inner = inner; }

            public bool IsSome => _inner._isSome;

            [MaybeNull] public T Value => _inner._value;
        }
    }

    // Core monadic methods.
    // - Maybe.Of() aka "return"
    // - Bind()
    // We could have chosen Select() and Maybe.Flatten(), aka "map" and "join",
    // instead. Maybe is a MonadPlus (or better a MonadOr) too, so OrElse() is
    // also part of the monadic methods.
    public partial struct Maybe<T>
    {
        /// <summary>
        /// Represents the empty <see cref="Maybe{T}" />, it does not enclose
        /// any value.
        /// <para>This field is read-only.</para>
        /// </summary>
        /// <seealso cref="Maybe.None{T}"/>
        public static readonly Maybe<T> None = default;

        [Pure]
        public Maybe<TResult> Bind<TResult>(Func<T, Maybe<TResult>> binder)
        {
            if (binder is null) { throw new Anexn(nameof(binder)); }

            return _isSome ? binder(_value) : Maybe<TResult>.None;
        }
    }

    // Safe escapes.
    // Actually, only ValueOrThrow() is truely safe, the other can only be
    // verified by the compiler and under special conditions (C# 8.0 and
    // .NET Core 3.0 or above).
    // We do not throw ArgumentNullException right away, we delay arg check
    // until it is strictly necessary.
    public partial struct Maybe<T>
    {
        /// <summary>
        /// If the current instance encloses a value, it unwraps it using
        /// <paramref name="caseSome"/>, otherwise it executes
        /// <paramref name="caseNone"/>.
        /// </summary>
        [Pure]
        public TResult Switch<TResult>(Func<T, TResult> caseSome, Func<TResult> caseNone)
            where TResult : notnull
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
        public TResult Switch<TResult>(Func<T, TResult> caseSome, TResult caseNone)
            where TResult : notnull
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

        [Pure]
        // Code size = 31 bytes.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue([MaybeNullWhen(false)] out T value)
        {
            if (_isSome)
            {
                value = _value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// Obtains the enclosed value if any; otherwise this method returns the
        /// default value of type <typeparamref name="T"/>.
        /// </summary>
        /// <seealso cref="TryGetValue"/>
        [Pure]
        [return: MaybeNull]
        public T ValueOrDefault()
            => _isSome ? _value : default;

        /// <summary>
        /// Obtains the enclosed value if any; otherwise this method returns
        /// <paramref name="other"/>.
        /// </summary>
        /// <seealso cref="TryGetValue"/>
        [Pure]
        // It does work with null but then one should really use ValueOrDefault().
        public T ValueOrElse([DisallowNull] T other)
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
            => _isSome ? _value : throw EF.Maybe_NoValue;

        [Pure]
        public T ValueOrThrow(Exception exception)
        {
            if (_isSome)
            {
                return _value;
            }
            else
            {
                if (exception is null) { throw new Anexn(nameof(exception)); }
                throw exception;
            }
        }
    }

    // Side effects.
    // Do() and Some() are specialized forms of Switch(), they do not return
    // anything (a Unit in fact). They could return "this" but I prefer not
    // to, this way it's clear that they are supposed to produce side effects.
    // We do not provide OnNone(action), since it is much simpler to write:
    //   if (maybe.IsNone) { action(); }
    // We do not throw ArgumentNullException right away, we delay arg check
    // until it is strictly necessary.
    public partial struct Maybe<T>
    {
        /// <summary>
        /// If the current instance encloses a value, it executes
        /// <paramref name="onSome"/>, otherwise it executes
        /// <paramref name="onNone"/>.
        /// </summary>
        /// <seealso cref="When"/>
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

        /// <summary>
        /// If the current instance encloses a value, it executes
        /// <paramref name="action"/>.
        /// </summary>
        /// <seealso cref="GetEnumerator"/>
        public void OnSome(Action<T> action)
        {
            if (_isSome)
            {
                if (action is null) { throw new Anexn(nameof(action)); }
                action(_value);
            }
        }

        // Enhanced versions of Do().
        // Beware, contrary to Do(), they do not throw for null actions.
        public void When(bool condition, Action<T>? onSome, Action? onNone)
        {
            if (condition)
            {
                if (_isSome)
                {
                    onSome?.Invoke(_value);
                }
                else
                {
                    onNone?.Invoke();
                }
            }
        }
    }

    // Query Expression Pattern.
    public partial struct Maybe<T>
    {
        /// <example>
        /// Query expression syntax:
        /// <code><![CDATA[
        ///   from x in maybe
        ///   select selector(x)
        /// ]]></code>
        /// </example>
        [Pure]
        public Maybe<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            if (selector is null) { throw new Anexn(nameof(selector)); }

            return _isSome ? Maybe.Of(selector(_value)) : Maybe<TResult>.None;
        }

        /// <example>
        /// Query expression syntax:
        /// <code><![CDATA[
        ///   from x in maybe
        ///   where predicate(x)
        ///   select x
        /// ]]></code>
        /// </example>
        [Pure]
        public Maybe<T> Where(Func<T, bool> predicate)
        {
            if (predicate is null) { throw new Anexn(nameof(predicate)); }

            return _isSome && predicate(_value) ? this : None;
        }

        /// <seealso cref="ZipWith"/>
        /// <example>
        /// Query expression syntax:
        /// <code><![CDATA[
        ///   from x in maybe
        ///   from y in selector(x)
        ///   select resultSelector(x, y)
        /// ]]></code>
        /// </example>
        /// <remarks>
        /// <see cref="SelectMany"/> generalizes both <see cref="ZipWith"/> and
        /// <see cref="Bind"/>. Namely, <see cref="ZipWith"/> is
        /// <see cref="SelectMany"/> with a constant selector <c>_ => other</c>:
        /// <code><![CDATA[
        ///   from x in maybe
        ///   from y in other
        ///   select zipper(x, y)
        /// ]]></code>
        /// Lesson: don't use <see cref="SelectMany"/> when <see cref="ZipWith"/>
        /// would do the job but without a (hidden) lambda function. As for
        /// <see cref="Bind"/>, it is <see cref="SelectMany"/> with a constant
        /// result selector <c>(_, y) => y</c>:
        /// <code><![CDATA[
        ///   from x in maybe
        ///   from y in binder(x)
        ///   select y
        /// ]]></code>
        /// Lesson: don't use <see cref="SelectMany"/> when <see cref="Bind"/>
        /// suffices.
        /// </remarks>
        [Pure]
        public Maybe<TResult> SelectMany<TMiddle, TResult>(
            Func<T, Maybe<TMiddle>> selector,
            Func<T, TMiddle, TResult> resultSelector)
        {
            if (selector is null) { throw new Anexn(nameof(selector)); }
            if (resultSelector is null) { throw new Anexn(nameof(resultSelector)); }

            if (!_isSome) { return Maybe<TResult>.None; }

            Maybe<TMiddle> middle = selector(_value);
            if (!middle._isSome) { return Maybe<TResult>.None; }

            return Maybe.Of(resultSelector(_value, middle._value));
        }

        /// <example>
        /// Query expression syntax:
        /// <code><![CDATA[
        ///   from x in outer
        ///   join y in inner
        ///     on outerKeySelector(x) equals innerKeySelector(y)
        ///   select resultSelector(x, y)
        /// ]]></code>
        /// </example>
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
                inner,
                outerKeySelector,
                innerKeySelector,
                resultSelector,
                EqualityComparer<TKey>.Default);
        }

        // No query expression syntax.
        // If "comparer" is null, the default equality comparer is used instead.
        [Pure]
        public Maybe<TResult> Join<TInner, TKey, TResult>(
            Maybe<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector,
            IEqualityComparer<TKey>? comparer)
        {
            if (outerKeySelector is null) { throw new Anexn(nameof(outerKeySelector)); }
            if (innerKeySelector is null) { throw new Anexn(nameof(innerKeySelector)); }
            if (resultSelector is null) { throw new Anexn(nameof(resultSelector)); }

            return JoinImpl(
                inner,
                outerKeySelector,
                innerKeySelector,
                resultSelector,
                comparer ?? EqualityComparer<TKey>.Default);
        }

        /// <example>
        /// Query expression syntax:
        /// <code><![CDATA[
        ///   from x in outer
        ///   join y in inner
        ///     on outerKeySelector(x) equals innerKeySelector(y)
        ///     into Z
        ///   select resultSelector(x, Z)
        /// ]]></code>
        /// </example>
        [Pure]
        public Maybe<TResult> GroupJoin<TInner, TKey, TResult>(
            Maybe<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, Maybe<TInner>, TResult> resultSelector)
        {
            if (outerKeySelector is null) { throw new Anexn(nameof(outerKeySelector)); }
            if (innerKeySelector is null) { throw new Anexn(nameof(innerKeySelector)); }
            if (resultSelector is null) { throw new Anexn(nameof(resultSelector)); }

            return GroupJoinImpl(
                inner,
                outerKeySelector,
                innerKeySelector,
                resultSelector,
                EqualityComparer<TKey>.Default);
        }

        // No query expression syntax.
        // If comparer is null, the default equality comparer is used instead.
        [Pure]
        public Maybe<TResult> GroupJoin<TInner, TKey, TResult>(
            Maybe<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, Maybe<TInner>, TResult> resultSelector,
            IEqualityComparer<TKey>? comparer)
        {
            if (outerKeySelector is null) { throw new Anexn(nameof(outerKeySelector)); }
            if (innerKeySelector is null) { throw new Anexn(nameof(innerKeySelector)); }
            if (resultSelector is null) { throw new Anexn(nameof(resultSelector)); }

            return GroupJoinImpl(
                inner,
                outerKeySelector,
                innerKeySelector,
                resultSelector,
                comparer ?? EqualityComparer<TKey>.Default);
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
                TKey outerKey = outerKeySelector(_value);
                TKey innerKey = innerKeySelector(inner._value);

                if (comparer.Equals(outerKey, innerKey))
                {
                    return Maybe.Of(resultSelector(_value, inner._value));
                }
            }

            return Maybe<TResult>.None;
        }

        [Pure]
        private Maybe<TResult> GroupJoinImpl<TInner, TKey, TResult>(
            Maybe<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, Maybe<TInner>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            if (_isSome && inner._isSome)
            {
                TKey outerKey = outerKeySelector(_value);
                TKey innerKey = innerKeySelector(inner._value);

                if (comparer.Equals(outerKey, innerKey))
                {
                    return Maybe.Of(resultSelector(_value, inner));
                }
            }

            return Maybe<TResult>.None;
        }
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
                : Maybe<TResult>.None;
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

    // "Bitwise" logical operations.
    // We have OrElse() & co, so it seems reasonnable to have the operators too.
    // It is not particulary recommended to have all the ops we can, it is even
    // considered to be bad practice, but I wish to keep them for two reasons:
    // - even if they are not true logical ops, we named the corresponding
    //   methods in a way that emphasizes the proximity w/ logical operations.
    // - most people won't even realize that they exist...
    //
    // We don't offer boolean logical operations. This would be confusing,
    // moreover don't have the expected properties. For instance, they are
    // non-abelian, and I haven't even check associativity.
    // There is only one case where it could make sense, Maybe<bool>, but
    // then it would be odd to have:
    //   Some(false) && Some(true) -> Some(true)
    // instead of Some(false).
    //
    // The methods are independent of Select()/Bind(). Maybe this can be done in
    // conjunction w/ OrElse(), but I haven't check this out.
    // References:
    // - https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/boolean-logical-operators
    public partial struct Maybe<T>
    {
#pragma warning disable CA2225 // Operator overloads have named alternates

        // Overloading true and false is necessary if we wish to support the
        // boolean logical AND (&&) and OR (||),
        //   x && y is evaluated as false(x) ? x : (x & y)
        //   x || y is evaluated as  true(x) ? x : (x | y)
        // but we don't really want to do it, don't we?
        // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#user-defined-conditional-logical-operators
        // Other logical op?
        // The logical negation (!), it "should" return a Maybe<T>, but then
        // what is the negation of None?
        //   public static bool operator !(Maybe<T> value) => !value._isSome;
        // See also the internal method ToBoolean() below and 3VL for Maybe<bool>.
#if false // Only kept to be sure that I won't try it again... do NOT enable.

        public static bool operator true(Maybe<T> value)
            => value._isSome;

        public static bool operator false(Maybe<T> value)
            => !value._isSome;

#endif

        // Bitwise logical OR.
        public static Maybe<T> operator |(Maybe<T> left, Maybe<T> right)
            => left.OrElse(right);

        // Bitwise logical AND.
        public static Maybe<T> operator &(Maybe<T> left, Maybe<T> right)
            => left.AndThen(right);

        // Bitwise logical XOR.
        public static Maybe<T> operator ^(Maybe<T> left, Maybe<T> right)
            => left.XorElse(right);

        // I know, this is just IsSome, but I wish to emphasize a boolean context.
        [InternalForTesting]
        internal bool ToBoolean()
            => _isSome;

#pragma warning restore CA2225

        /// <remarks>
        /// Generalizes the null-coalescing operator (??) to maybe's, it returns
        /// the first non-empty value (if any).
        /// <code><![CDATA[
        ///   Some(1) OrElse Some(2) == Some(1)
        ///   Some(1) OrElse None    == Some(1)
        ///   None    OrElse Some(2) == Some(2)
        ///   None    OrElse None    == None
        ///
        ///   Some(1) ?? Some(2) == Some(1)
        ///   Some(1) ?? None    == Some(1)
        ///   None    ?? Some(2) == Some(2)
        ///   None    ?? None    == None
        /// ]]></code>
        /// This method can be though as an inclusive OR for maybe's, provided
        /// that an empty maybe is said to be false.
        /// </remarks>
        //
        // Inclusive disjunction; mnemotechnic: "P otherwise Q".
        // "Plus" operation for maybe's.
        [Pure]
        public Maybe<T> OrElse(Maybe<T> other)
            => _isSome ? this : other;

        /// <summary>
        /// Continues with <paramref name="other"/> if the current instance is
        /// not empty; otherwise returns the empty maybe of type
        /// <typeparamref name="TResult"/>.
        /// </summary>
        /// <remarks>
        /// <code><![CDATA[
        ///   Some(1) AndThen Some(2L) == Some(2L)
        ///   Some(1) AndThen None     == None
        ///   None    AndThen Some(2L) == None
        ///   None    AndThen None     == None
        /// ]]></code>
        /// This method can be though as an AND for maybe's, provided that an
        /// empty maybe is said to be false.
        /// </remarks>
        //
        // Conjunction; mnemotechnic "Q if P", "P and then Q".
        // Compare to the nullable equiv w/ x an int? and y a long?:
        //   (x.HasValue ? y : (long?)null).
        [Pure]
        public Maybe<TResult> AndThen<TResult>(Maybe<TResult> other)
        {
            return _isSome ? other : Maybe<TResult>.None;
        }

        // Exclusive disjunction; mnemotechnic: "either P or Q, but not both".
        // XorElse() = flip XorElse():
        //   this.XorElse(other) = other.XorElse(this)
        /// <remarks>
        /// <code><![CDATA[
        ///   Some(1) XorElse Some(2) == None
        ///   Some(1) XorElse None    == Some(1)
        ///   None    XorElse Some(2) == Some(2)
        ///   None    XorElse None    == None
        /// ]]></code>
        /// This method can be though as an exclusive OR for maybe's, provided
        /// that an empty maybe is said to be false.
        /// </remarks>
        [Pure]
        public Maybe<T> XorElse(Maybe<T> other)
            => _isSome ? other._isSome ? None : this
                : other;
    }

    // Misc methods.
    // Some can be built from Select() or Bind(), but we prefer not to since
    // this forces us to use (unnecessary) lambda functions.
    public partial struct Maybe<T>
    {
        /// <remarks>
        /// <para>
        /// <see cref="ZipWith"/> is <see cref="Select"/> with two maybe's,
        /// it is also a special case of <see cref="SelectMany"/>; see the
        /// comments there. Roughly, <see cref="ZipWith"/> unwraps two maybe's,
        /// then applies a zipper, and eventually wraps the result.
        /// </para>
        /// <para>
        /// Compare to the F# computation expressions for an hypothetical maybe
        /// workflow:
        /// <code><![CDATA[
        ///   maybe {
        ///     let! x = this;          // Unwrap this
        ///     let! y = other;         // Unwrap other
        ///     return zipper(x, y);    // Zip then wrap (return = wrap)
        ///   }
        /// ]]></code>
        /// F# users are lucky, the special syntax even extends to more than two
        /// maybe's. Nothing similar in C#. Adding ZipWith's with more parameters
        /// is a possibility but it looks artificial; for a better(?) solution
        /// see the Lift methods in <see cref="Maybe"/>.
        /// </para>
        /// </remarks>
        // F# Workflow: let!.
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

        // TODO: naming
        // Skip() -> Void(), Unit(), Discard()?
        // Duplicate() -> Square()

        [Pure]
        public Maybe<Unit> Skip()
            => _isSome ? Maybe.Unit : Maybe.Zero;

        /// <seealso cref="Maybe.Flatten"/>
        [Pure]
        public Maybe<Maybe<T>> Duplicate()
            => new Maybe<Maybe<T>>(this);

        /// <seealso cref="Yield(int)"/>
        /// <remarks>
        /// The difference with <see cref="Yield(int)"/> is in the treatment of
        /// an empty maybe. <see cref="Yield(int)"/> for an empty maybe returns
        /// an empty sequence, whereas this method returns an empty maybe (no
        /// sequence at all).
        /// </remarks>
        [Pure]
        public Maybe<IEnumerable<T>> Replicate(int count)
            // Identical to:
            //   Select(x => Enumerable.Repeat(x, count));
            => _isSome ? Maybe.Of(Enumerable.Repeat(_value, count))
                : Maybe<IEnumerable<T>>.None;

        // Beware, infinite loop!
        /// <seealso cref="Yield()"/>
        /// <remarks>
        /// The difference with <see cref="Yield()"/> is in the treatment of
        /// an empty maybe. <see cref="Yield()"/> for an empty maybe returns
        /// an empty sequence, whereas this method returns an empty maybe (no
        /// sequence at all).
        /// </remarks>
        [Pure]
        public Maybe<IEnumerable<T>> Replicate()
        {
            return Select(__selector);

            static IEnumerable<T> __selector(T value)
                // NULL_FORGIVING: Select() guarantees that "value" won't be null.
                => new NeverEndingIterator<T>(value!);
        }
    }

    // Iterable but not IEnumerable<>.
    // 1) A maybe is indeed a collection but a rather trivial one.
    // 2) Maybe<T> being a struct, I worry about hidden casts.
    // 3) Source of confusion (conflicts?) if we import System.Linq too.
    // Furthermore, this type does NOT implement the whole Query Expression
    // Pattern.
    public partial struct Maybe<T>
    {
        // Supporting "foreach" is a bit odd but sometimes rather than:
        //   foreach (var x in maybe) { action(x); }
        // I prefer to write:
        //   maybe.OnSome(action);
        // or even better ("using" is not necessary here):
        //   var iter = maybe.GetEnumerator();
        //   if (iter.MoveNext()) { onSome(iter.Current); } else { onNone(); }
        // It is less weird if we realize that a maybe is just a set (singleton
        // or empty).
        // See also TryGetValue().
        [Pure]
        public IEnumerator<T> GetEnumerator()
            // NULL_FORGIVING: when _isSome is true, _value is NOT null.
            => _isSome ? new SingletonIterator<T>(_value!) : EmptyIterator<T>.Instance;

        /// <summary>
        /// Converts the current instance to <see cref="IEnumerable{T}"/>.
        /// </summary>
        // Only useful if we want to manipulate a maybe together with another
        // sequence.
        [Pure]
        public IEnumerable<T> ToEnumerable()
            // NULL_FORGIVING: when _isSome is true, _value is NOT null.
            => _isSome ? new SingletonIterator<T>(_value!) : Enumerable.Empty<T>();

        // REVIEW: Yield() is not a good name (it's not the F# yield).

        // Yield break or yield return "count" times.
        /// See also <seealso cref="Replicate(int)"/> and the comments there.
        [Pure]
        public IEnumerable<T> Yield(int count)
            => _isSome ? Enumerable.Repeat(_value, count) : Enumerable.Empty<T>();

        // Beware, infinite loop!
        /// See also <seealso cref="Replicate()"/> and the comments there.
        [Pure]
        public IEnumerable<T> Yield()
            // NULL_FORGIVING: when _isSome is true, _value is NOT null.
            => _isSome ? new NeverEndingIterator<T>(_value!) : Enumerable.Empty<T>();

        // See also Replicate() and the comments there.
        // Maybe<T> being a struct it is never equal to null, therefore
        // Contains(null) always returns false.
        [Pure]
        public bool Contains(T value)
            => _isSome && EqualityComparer<T>.Default.Equals(_value, value);

        [Pure]
        public bool Contains(T value, IEqualityComparer<T> comparer)
        {
            if (comparer is null) { throw new Anexn(nameof(comparer)); }

            return _isSome && comparer.Equals(_value, value);
        }
    }

    // Interface IComparable<>.
    // The comparison operators behave like the ones for nullable value types:
    // if one of the operand is empty, return false, otherwise compare the
    // values.
    public partial struct Maybe<T>
    {
        /// <summary>
        /// Compares the two specified instances to see if the left one is
        /// strictly less than the right one.
        /// <para>Beware, if either operand is empty, this operator will return
        /// false.</para>
        /// </summary>
        /// <remarks>
        /// <para>The weird behaviour with empty maybe's is the same one
        /// implemented by nullables.</para>
        /// <para>For proper sorting, one MUST use
        /// <see cref="CompareTo(Maybe{T})"/> or <see cref="MaybeComparer{T}"/>
        /// as they produce a consistent total ordering.</para>
        /// </remarks>
        public static bool operator <(Maybe<T> left, Maybe<T> right)
            // Beware, this is NOT the same as
            //   left.CompareTo(right) < 0;
            => left._isSome && right._isSome
                ? Comparer<T>.Default.Compare(left.Value, left.Value) < 0
                : false;

        /// <summary>
        /// Compares the two specified instances to see if the left one is
        /// less than or equal to the right one.
        /// <para>Beware, if either operand is empty, this operator will return
        /// false.</para>
        /// </summary>
        /// <remarks>
        /// <para>The weird behaviour with empty maybe's is the same one
        /// implemented by nullables.</para>
        /// <para>For proper sorting, one MUST use
        /// <see cref="CompareTo(Maybe{T})"/> or <see cref="MaybeComparer{T}"/>
        /// as they produce a consistent total ordering.</para>
        /// </remarks>
        public static bool operator <=(Maybe<T> left, Maybe<T> right)
            // Beware, this is NOT the same as
            //   left.CompareTo(right) <= 0;
            => left._isSome && right._isSome
                ? Comparer<T>.Default.Compare(left.Value, left.Value) <= 0
                : false;

        /// <summary>
        /// Compares the two specified instances to see if the left one is
        /// strictly greater than the right one.
        /// <para>Beware, if either operand is empty, this operator will return
        /// false.</para>
        /// </summary>
        /// <remarks>
        /// <para>The weird behaviour with empty maybe's is the same one
        /// implemented by nullables.</para>
        /// <para>For proper sorting, one MUST use
        /// <see cref="CompareTo(Maybe{T})"/> or <see cref="MaybeComparer{T}"/>
        /// as they produce a consistent total ordering.</para>
        /// </remarks>
        public static bool operator >(Maybe<T> left, Maybe<T> right)
            // Beware, this is NOT the same as
            //   left.CompareTo(right) > 0;
            => left._isSome && right._isSome
                ? Comparer<T>.Default.Compare(left.Value, left.Value) > 0
                : false;

        /// <summary>
        /// Compares the two specified instances to see if the left one is
        /// greater than or equal to the right one.
        /// <para>Beware, if either operand is empty, this operator will return
        /// false.</para>
        /// </summary>
        /// <remarks>
        /// <para>The weird behaviour with empty maybe's is the same one
        /// implemented by nullables.</para>
        /// <para>For proper sorting, one MUST use
        /// <see cref="CompareTo(Maybe{T})"/> or <see cref="MaybeComparer{T}"/>
        /// as they produce a consistent total ordering.</para>
        /// </remarks>
        public static bool operator >=(Maybe<T> left, Maybe<T> right)
            // Beware, this is NOT the same as
            //   left.CompareTo(right) >= 0;
            => left._isSome && right._isSome
                ? Comparer<T>.Default.Compare(left.Value, left.Value) >= 0
                : false;

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
            => _isSome
                ? other._isSome && EqualityComparer<T>.Default.Equals(_value, other._value)
                : !other._isSome;

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

            // NULL_FORGIVING: when _isSome is true, _value is NOT null.
            return _isSome ? comparer.GetHashCode(_value!) : 0;
        }
    }
}
