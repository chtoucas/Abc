// See LICENSE.txt in the project root for license information.

namespace Play.Functional
{
    using System;
    using System.Collections.Generic;

    using static Require;

    public static class Ident
    {
        public static Ident<T> Of<T>(T value) where T : notnull
            => Ident<T>.η(value);

        public static Ident<T> Flatten<T>(Ident<Ident<T>> square) where T : notnull
            => Ident<T>.μ(square);

        public static T Extract<T>(Ident<T> ident) where T : notnull
            => Ident<T>.ε(ident);

        public static Ident<Ident<T>> Duplicate<T>(Ident<T> ident) where T : notnull
            => Ident<T>.δ(ident);
    }

    /// <summary>
    /// Defines the trivial monad/comonad.
    /// <para>Pretty useless.</para>
    /// </summary>
    public readonly partial struct Ident<T> : IEquatable<Ident<T>>, IEquatable<T>
        where T : notnull
    {
        private static readonly IEqualityComparer<T> s_DefaultComparer
            = EqualityComparer<T>.Default;

        private readonly T _value;

        private Ident(T value)
        {
            _value = value;
        }

        public bool Contains(T value)
            => Equals(value);

        public bool Contains(T value, IEqualityComparer<T> comparer)
            => Equals(value, comparer);

        public IEnumerator<T> GetEnumerator()
        {
            yield return _value;
        }
    }

    // Monad.
    public partial struct Ident<T>
    {
        public Ident<TResult> Bind<TResult>(Func<T, Ident<TResult>> binder)
            where TResult : notnull
        {
            NotNull(binder, nameof(binder));

            return binder(_value);
        }

        // The unit.
        internal static Ident<T> η(T value)
            => new Ident<T>(value);

        // The multiplication or composition.
        internal static Ident<T> μ(Ident<Ident<T>> square)
            => square._value;
    }

    // Comonad.
    public partial struct Ident<T>
    {
        public Ident<TResult> Extend<TResult>(Func<Ident<T>, TResult> extender)
            where TResult : notnull
        {
            NotNull(extender, nameof(extender));

            return new Ident<TResult>(extender(this));
        }

        // The counit (simpler alt, create a property Value).
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

        public static bool operator ==(Ident<T> left, T right)
            => left.Equals(right);

        public static bool operator !=(Ident<T> left, T right)
            => !left.Equals(right);

        public static bool operator ==(T left, Ident<T> right)
            => right.Equals(left);

        public static bool operator !=(T left, Ident<T> right)
            => !right.Equals(left);

        public bool Equals(Ident<T> other)
            => s_DefaultComparer.Equals(_value, other._value);

        public bool Equals(Ident<T> other, IEqualityComparer<T> comparer)
            => (comparer ?? s_DefaultComparer).Equals(_value, other._value);

        public bool Equals(T other)
            => s_DefaultComparer.Equals(_value, other);

        public bool Equals(T other, IEqualityComparer<T> comparer)
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
