// See LICENSE.txt in the project root for license information.

namespace Play.Edu
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Provides static helpers for <see cref="Ident{T}"/>.
    /// <para>This class cannot be inherited.</para>
    /// </summary>
    public static class Ident
    {
        public static Ident<T> Of<T>([DisallowNull]T value)
            => Ident<T>.η(value);

        public static Ident<T> Flatten<T>(Ident<Ident<T>> square)
            => Ident<T>.μ(square);

        [return: NotNull]
        public static T Extract<T>(Ident<T> ident)
            => Ident<T>.ε(ident);

        public static Ident<Ident<T>> Duplicate<T>(Ident<T> ident)
            => Ident<T>.δ(ident);

        public static ValueTuple x => new ValueTuple();
    }

    /// <summary>
    /// Defines the trivial monad/comonad (pretty useless).
    /// <para><see cref="Ident{T}"/> is an immutable struct.</para>
    /// </summary>
    public readonly partial struct Ident<T> : IEquatable<Ident<T>>, IEquatable<T>
    {
        private static readonly IEqualityComparer<T> s_DefaultComparer
            = EqualityComparer<T>.Default;

        private readonly T _value;

        private Ident([DisallowNull]T value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public bool Contains([AllowNull]T value)
            => Equals(value);

        public bool Contains([AllowNull]T value, IEqualityComparer<T> comparer)
            => Equals(value, comparer);

        public IEnumerator<T> GetEnumerator()
        {
            yield return _value;
        }
    }

    // It's a monad.
    public partial struct Ident<T>
    {
        public Ident<TResult> Bind<TResult>(Func<T, Ident<TResult>> binder)
        {
            Require.NotNull(binder, nameof(binder));

            return binder(_value);
        }

        // The unit (wrap, public ctor).
        internal static Ident<T> η([DisallowNull]T value)
            => new Ident<T>(value);

        // The multiplication or composition.
        internal static Ident<T> μ(Ident<Ident<T>> square)
            => square._value;
    }

    // It's a comonad.
    public partial struct Ident<T>
    {
        public Ident<TResult> Extend<TResult>(Func<Ident<T>, TResult> extender)
            where TResult : notnull
        {
            Require.NotNull(extender, nameof(extender));

            return new Ident<TResult>(extender(this));
        }

        // The counit (unwrap, property Value).
        [return: NotNull]
        internal static T ε(Ident<T> ident)
            => ident._value;

        // The comultiplication.
        internal static Ident<Ident<T>> δ(Ident<T> ident)
            => new Ident<Ident<T>>(ident);
    }

    // Interface IEquatable<>.
    public partial struct Ident<T>
    {
        public static bool operator ==(Ident<T> left, Ident<T> right)
            => left.Equals(right);

        public static bool operator !=(Ident<T> left, Ident<T> right)
            => !left.Equals(right);

        public static bool operator ==(Ident<T> left, [AllowNull]T right)
            => left.Equals(right);

        public static bool operator !=(Ident<T> left, [AllowNull]T right)
            => !left.Equals(right);

        public static bool operator ==([AllowNull]T left, Ident<T> right)
            => right.Equals(left);

        public static bool operator !=([AllowNull]T left, Ident<T> right)
            => !right.Equals(left);

        public bool Equals(Ident<T> other)
            => s_DefaultComparer.Equals(_value, other._value);

        public bool Equals(Ident<T> other, IEqualityComparer<T> comparer)
            => (comparer ?? s_DefaultComparer).Equals(_value, other._value);

        public bool Equals([AllowNull]T other)
            => s_DefaultComparer.Equals(_value, other);

        public bool Equals([AllowNull]T other, IEqualityComparer<T> comparer)
            => (comparer ?? s_DefaultComparer).Equals(_value, other);

        public override bool Equals(object? obj)
            => obj is Ident<T> ident ? Equals(ident)
                : obj is T value ? Equals(value)
                : false;

        public bool Equals(object? other, IEqualityComparer<T> comparer)
            => other is Ident<T> ident ? Equals(ident, comparer)
                : other is T value ? Equals(value, comparer)
                : false;

        public override int GetHashCode()
            => _value?.GetHashCode() ?? 0;

        public int GetHashCode(IEqualityComparer<T> comparer)
            => (comparer ?? s_DefaultComparer).GetHashCode(_value);
    }
}
