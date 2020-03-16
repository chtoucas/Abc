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
    /// Instance methods where the result is another maybe.
    /// - Bind()                unwrap then map to another maybe
    /// - Select()              LINQ select
    /// - SelectMany()          LINQ select many
    /// - Where()               LINQ filter
    /// - Join()                LINQ join
    /// - GroupJoin()           LINQ group join
    /// - OrElse()              none-coalescing, OR gate
    /// - XorElse()             XOR gate
    /// - ZipWith()             cross join
    /// - Apply()
    /// - ContinueWith()        "right" AND gate
    /// - PassThru()            "left" AND gate
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
    /// - Unless()
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
        private readonly T _value;

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

        /// <summary>
        /// Represents a debugger type proxy for <see cref="Maybe{T}"/>.
        /// </summary>
        [ExcludeFromCodeCoverage]
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

        // Inclusive disjunction; mnemotechnic: "P otherwise Q".
        /// <remarks>
        /// Generalizes the null-coalescing operator (??) to maybe's.
        /// <code><![CDATA[
        ///   Some(1) ?? Some(2) == Some(1)
        ///   Some(1) ?? None    == Some(1)
        ///   None    ?? Some(2) == Some(2)
        ///   None    ?? None    == None
        /// ]]></code>
        /// This method can be though as an inclusive OR for maybe's, provided
        /// that an empty maybe is said to be false.
        /// </remarks>
        [Pure]
        public Maybe<T> OrElse(Maybe<T> other)
            => _isSome ? this : other;
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
        public bool TryGetValue([MaybeNullWhen(false)] out T value)
        {
            if (_isSome)
            {
                value = _value;
                return true;
            }
            else
            {
                // NULL_FORGIVING: Try-Parse pattern.
                value = default!;
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

        // Reverse of When().
        public void Unless(bool condition, Action<T>? onSome, Action? onNone)
        {
            if (!condition)
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

    // Misc methods.
    // They can all be built from Select() or Bind(), but we prefer not to
    // since this forces us to use (unnecessary) lambda functions.
    public partial struct Maybe<T>
    {
        [Pure]
        public Maybe<TResult> Apply<TResult>(Maybe<Func<T, TResult>> applicative)
        {
            return _isSome && applicative._isSome ? Maybe.Of(applicative._value(_value))
                : Maybe<TResult>.None;
        }

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

        // Conjunction; mnemotechnic "Q if P", "P and then Q".
        // ContinueWith() = flip PassThru():
        //   this.ContinueWith(other) = other.PassThru(this)
        /// <summary>
        /// Returns <paramref name="other"/> if the current instance is not
        /// empty; otherwise returns the empty maybe of type
        /// <typeparamref name="TResult"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="ContinueWith"/> is <see cref="Bind"/> with a constant
        /// binder <c>_ => other</c>.
        /// <code><![CDATA[
        ///   Some(1) ContinueWith Some(2L) == Some(2L)
        ///   Some(1) ContinueWith None     == None
        ///   None    ContinueWith Some(2L) == None
        ///   None    ContinueWith None     == None
        /// ]]></code>
        /// </remarks>
        // Compare to the nullable equiv w/ x an int? and y a long?:
        //   (x.HasValue ? y : (long?)null).
        [Pure]
        public Maybe<TResult> ContinueWith<TResult>(Maybe<TResult> other)
        {
            return _isSome ? other : Maybe<TResult>.None;
        }

        // Conjunction; mnemotechnic "P if Q".
        // PassThru() = flip ContinueWith():
        //   this.PassThru(other) = other.ContinueWith(this)
        /// <summary>
        /// Returns the current instance if <paramref name="other"/> is not
        /// empty; otherwise returns the empty maybe of type
        /// <typeparamref name="TOther"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="ContinueWith"/> is <see cref="Bind"/> with a constant
        /// binder <c>_ => this</c>.
        /// <code><![CDATA[
        ///   Some(1) PassThru Some(2L) == Some(1)
        ///   Some(1) PassThru None     == None
        ///   None    PassThru Some(2L) == None
        ///   None    PassThru None     == None
        /// ]]></code>
        /// </remarks>
        // Compare to the nullable equiv w/ x an int? and y a long?:
        //   (y.HasValue ? x : (int?)null).
        [Pure]
        public Maybe<T> PassThru<TOther>(Maybe<TOther> other)
        {
            return other._isSome ? this : None;
        }

        // Exclusive disjunction; mnemotechnic: "either P or Q, but not both".
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
            => Select(Sequence.Repeat);
    }

    // Iterable but not IEnumerable<>.
    // 1) A maybe is indeed a collection but a rather trivial one.
    // 2) Maybe<T> being a struct, I worry about hidden casts.
    // 3) Source of confusion (conflicts?) if we import System.Linq too.
    // Furthermore, this type does NOT implement the whole Query Expression
    // Pattern.
    public partial struct Maybe<T>
    {
        // REVIEW: now that we have TryGetValue(), do we need this method anymore?
        // At the same time, since we have ToEnumerable(), it seems natural to
        // have GetEnumerator() too.
        [Pure]
        public IEnumerator<T> GetEnumerator()
            => ToEnumerable().GetEnumerator();

        /// <summary>
        /// Converts the current instance to <see cref="IEnumerable{T}"/>.
        /// </summary>
        // Only useful if we want to manipulate a maybe together with another
        // sequence.
        [Pure]
        public IEnumerable<T> ToEnumerable()
            => _isSome ? Sequence.Singleton(_value) : Enumerable.Empty<T>();

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
            => _isSome ? Sequence.Repeat(_value) : Enumerable.Empty<T>();

        // See also Replicate() and the comments there.
        // Maybe<T> being a struct it is never equal to null, therefore
        // Contains(null) always returns false.
        [Pure]
        public bool Contains(T value)
            => _isSome && EqualityComparer<T>.Default.Equals(_value, value);
    }

    // REVIEW: the rules should be compatible with the one followed by for say
    // a nullable int.
    // For the comparison operators <, >, <=, and >=, if one or both operands
    // are null, the result is false; otherwise, the contained values of
    // operands are compared. Do not assume that because a particular comparison
    // (for example, <=) returns false, the opposite comparison (>) returns true.
    // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/nullable-value-types#lifted-operators

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

            // NULL_FORGIVING: when _isSome is true, _value is NOT null.
            return _isSome ? comparer.GetHashCode(_value!) : 0;
        }
    }
}
