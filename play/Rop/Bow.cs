// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Runtime.CompilerServices;

    using Anexn = System.ArgumentNullException;
    using EF = Abc.Utilities.ExceptionFactory;

    // Temporary name.
    // Option: reserved keyword for VB.NET.
    // Option<T>: conflict w/ F# type.
    // Bow = "Black or white".

    public static class Bow
    {
        public static readonly Bow<Unit> Unit = Some(default(Unit));

        public static readonly Bow<Unit> Zero = Bow<Unit>.None;

        [Pure]
        // Not actually obsolete, but clearly states that we shouldn't use it.
        [Obsolete("Use SomeOrNone() instead.")]
        public static Bow<T> Of<T>(T? value) where T : struct
            => value.HasValue ? new Some<T>(value.Value) : Bow<T>.None;

        [Pure]
        public static Bow<T> Of<T>([AllowNull] T value)
            => value is null ? Bow<T>.None : new Some<T>(value);

        // Unconstrained version: see Option<T>.None.
        [Pure]
        public static Bow<T> None<T>() where T : notnull
            => Bow<T>.None;

        [Pure]
        public static Bow<T> Some<T>(T value) where T : struct
            => new Some<T>(value);

        [Pure]
        public static Bow<T> SomeOrNone<T>(T? value) where T : struct
            => value.HasValue ? new Some<T>(value.Value) : Bow<T>.None;

        [Pure]
        public static Bow<T> SomeOrNone<T>(T? value) where T : class
            => value is null ? Bow<T>.None : new Some<T>(value);
    }

    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public abstract class Bow<T>
    {
        public static readonly Bow<T> None = new None<T>();

        // Only two classes can extend this one (Some<T> and None<T>),
        // pattern matching is therefore unambiguous.
        private protected Bow() { }

        public bool IsSome => !IsNone;

        public abstract bool IsNone { get; }

        // Even w/ Some<T> this property never returns null, indeed in case of
        // error the getter throws.
        [NotNull] public abstract T Value { get; }

        [ExcludeFromCodeCoverage]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        private string DebuggerDisplay => $"IsSome = {IsSome}";

        [Pure] public abstract Maybe<T> ToMaybe();

        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Visual Basic: use an escaped name")]
        [Pure] public abstract Bow<T> OrElse(Bow<T> other);

        [Pure]
        // Code size = 31 bytes.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue([MaybeNullWhen(false)] out T value)
        {
            if (IsNone)
            {
                value = default;
                return false;
            }
            else
            {
                value = Value;
                return true;
            }
        }

        #region Query Expression Pattern

        [Pure]
        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Query Expression Pattern")]
        public abstract Bow<TResult> Select<TResult>(Func<T, TResult> selector);

        [Pure] public abstract Bow<T> Where(Func<T, bool> predicate);

        [Pure]
        public abstract Bow<TResult> SelectMany<TMiddle, TResult>(
            Func<T, Bow<TMiddle>> selector,
            Func<T, TMiddle, TResult> resultSelector);

        [Pure]
        public abstract Bow<TResult> Join<TInner, TKey, TResult>(
            Bow<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector);

        [Pure]
        public abstract Bow<TResult> Join<TInner, TKey, TResult>(
            Bow<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector,
            IEqualityComparer<TKey>? comparer);

        #endregion
    }

    public sealed class Some<T> : Bow<T>
    {
        internal Some([DisallowNull] T value)
        {
            Value = value;
        }

        public override bool IsNone => false;

        [NotNull] public override T Value { get; }

        [Pure] public override string ToString() => $"Some({Value})";

        // TODO: => new Maybe<T>(Value);
        [Pure] public override Maybe<T> ToMaybe() => Maybe.Of(Value);

        [Pure] public override Bow<T> OrElse(Bow<T> other) => this;

        [Pure]
        public override Bow<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            if (selector is null) { throw new Anexn(nameof(selector)); }

            return Bow.Of(selector(Value));
        }

        [Pure]
        public override Bow<T> Where(Func<T, bool> predicate)
        {
            if (predicate is null) { throw new Anexn(nameof(predicate)); }

            return predicate(Value) ? this : None;
        }

        [Pure]
        public override Bow<TResult> SelectMany<TMiddle, TResult>(
            Func<T, Bow<TMiddle>> selector,
            Func<T, TMiddle, TResult> resultSelector)
        {
            if (selector is null) { throw new Anexn(nameof(selector)); }
            if (resultSelector is null) { throw new Anexn(nameof(resultSelector)); }

            Bow<TMiddle> middle = selector(Value);
            if (middle.IsNone) { return Bow<TResult>.None; }

            return Bow.Of(resultSelector(Value, middle.Value));
        }

        [Pure]
        public override Bow<TResult> Join<TInner, TKey, TResult>(
            Bow<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector)
        {
            if (inner is null) { throw new Anexn(nameof(inner)); }
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

        [Pure]
        public override Bow<TResult> Join<TInner, TKey, TResult>(
            Bow<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector,
            IEqualityComparer<TKey>? comparer)
        {
            if (inner is null) { throw new Anexn(nameof(inner)); }
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

        [Pure]
        private Bow<TResult> JoinImpl<TInner, TKey, TResult>(
            Bow<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            if (!inner.IsNone)
            {
                TKey outerKey = outerKeySelector(Value);
                TKey innerKey = innerKeySelector(inner.Value);

                if (comparer.Equals(outerKey, innerKey))
                {
                    return Bow.Of(resultSelector(Value, inner.Value));
                }
            }

            return Bow<TResult>.None;
        }
    }

    public sealed class None<T> : Bow<T>
    {
        public static readonly None<T> Instance = new None<T>();

        internal None() { }

        public override bool IsNone => true;

        public override T Value { [DoesNotReturn] get => throw EF.Bow_NoValue; }

        [Pure] public override string ToString() => "None";

        [Pure] public override Maybe<T> ToMaybe() => Maybe<T>.None;

        [Pure] public override Bow<T> OrElse(Bow<T> other) => other;

        [Pure]
        public override Bow<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            return None<TResult>.Instance;
        }

        [Pure]
        public override Bow<T> Where(Func<T, bool> predicate)
        {
            return this;
        }

        [Pure]
        public override Bow<TResult> SelectMany<TMiddle, TResult>(
            Func<T, Bow<TMiddle>> selector,
            Func<T, TMiddle, TResult> resultSelector)
        {
            return None<TResult>.Instance;
        }

        [Pure]
        public override Bow<TResult> Join<TInner, TKey, TResult>(
            Bow<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector)
        {
            return None<TResult>.Instance;
        }

        [Pure]
        public override Bow<TResult> Join<TInner, TKey, TResult>(
            Bow<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector,
            IEqualityComparer<TKey>? comparer)
        {
            return None<TResult>.Instance;
        }
    }
}
