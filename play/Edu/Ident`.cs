// See LICENSE.txt in the project root for license information.

namespace Abc.Fx
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    // REVIEW: IStructuralComparable, IComparable?

    /// <summary>
    /// Provides static helpers for <see cref="Ident{T}"/>.
    /// <para>This class cannot be inherited.</para>
    /// </summary>
    public static class Ident
    {
        [Pure]
        public static Ident<T> Of<T>(T value) where T : notnull
            => Ident<T>.η(value);

        [Pure]
        public static Ident<T> Flatten<T>(Ident<Ident<T>> square) where T : notnull
            => Ident<T>.μ(square);

        [Pure]
        public static T Extract<T>(Ident<T> ident) where T : notnull
            => Ident<T>.ε(ident);

        [Pure]
        public static Ident<Ident<T>> Duplicate<T>(Ident<T> ident) where T : notnull
            => Ident<T>.δ(ident);
    }

    /// <summary>
    /// Represents the trivial monad/comonad (pretty useless).
    /// <para><see cref="Ident{T}"/> is an immutable struct.</para>
    /// </summary>
    public readonly partial struct Ident<T> : IEquatable<Ident<T>>, IStructuralEquatable
        where T : notnull
    {
        private static readonly IEqualityComparer s_DefaultComparer
            = EqualityComparer<T>.Default;

        private readonly T _value;

        private Ident(T value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override string ToString()
            => $"({_value})";

        [Pure]
        public bool Contains(T value)
            => s_DefaultComparer.Equals(_value, value);

        [Pure]
        public bool Contains(T value, IEqualityComparer comparer)
            => (comparer ?? s_DefaultComparer).Equals(_value, value);

        [Pure]
        public IEnumerator<T> GetEnumerator()
        {
            yield return _value;
        }
    }

    // It's a monad.
    public partial struct Ident<T>
    {
        [Pure]
        public Ident<TResult> Bind<TResult>(Func<T, Ident<TResult>> binder)
            where TResult : notnull
        {
            Require.NotNull(binder, nameof(binder));

            return binder(_value);
        }

        // The unit (wrap, public ctor).
        [Pure]
        internal static Ident<T> η(T value)
            => new Ident<T>(value);

        // The multiplication or composition.
        [Pure]
        internal static Ident<T> μ(Ident<Ident<T>> square)
            => square._value;
    }

    // It's a comonad.
    public partial struct Ident<T>
    {
        [Pure]
        public Ident<TResult> Extend<TResult>(Func<Ident<T>, TResult> extender)
            where TResult : notnull
        {
            Require.NotNull(extender, nameof(extender));

            return new Ident<TResult>(extender(this));
        }

        // The counit (unwrap, property Value).
        [Pure]
        internal static T ε(Ident<T> ident)
            => ident._value;

        // The comultiplication.
        [Pure]
        internal static Ident<Ident<T>> δ(Ident<T> ident)
            => new Ident<Ident<T>>(ident);
    }

    // Interfaces IEquatable<>, IStructuralEquatable.
    public partial struct Ident<T>
    {
        public static bool operator ==(Ident<T> left, Ident<T> right)
            => left.Equals(right);

        public static bool operator !=(Ident<T> left, Ident<T> right)
            => !left.Equals(right);

        public static bool operator ==(Ident<T> left, T right)
            => left.Contains(right);

        public static bool operator !=(Ident<T> left, T right)
            => !left.Contains(right);

        public static bool operator ==(T left, Ident<T> right)
            => right.Contains(left);

        public static bool operator !=(T left, Ident<T> right)
            => !right.Contains(left);

        [Pure]
        public bool Equals(Ident<T> other)
            => s_DefaultComparer.Equals(_value, other._value);
        [Pure]

        public bool Equals(Ident<T> other, IEqualityComparer comparer)
            => (comparer ?? s_DefaultComparer).Equals(_value, other._value);

        [Pure]
        public override bool Equals(object? obj)
            => obj is Ident<T> ident ? Equals(ident)
                : obj is T value ? Contains(value)
                : false;

        [Pure]
        bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer)
            => other is Ident<T> ident ? Equals(ident, comparer)
                : other is T value ? Contains(value, comparer)
                : false;

        [Pure]
        public override int GetHashCode()
            => _value.GetHashCode();

        [Pure]
        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
            => (comparer ?? s_DefaultComparer).GetHashCode(_value);
    }
}
