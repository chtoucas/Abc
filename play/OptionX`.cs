// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Runtime.CompilerServices;

    public static class OptionX
    {
        public static readonly OptionX<Unit> Unit = Some(default(Unit));

        public static readonly OptionX<Unit> Zero = OptionX<Unit>.None;

        [Pure]
        // Not actually obsolete, but clearly states that we shouldn't use it.
        [Obsolete("Use SomeOrNone() instead.")]
        public static OptionX<T> Of<T>(T? value) where T : struct
            => value.HasValue ? new OptionX<T>(value.Value) : OptionX<T>.None;

        [Pure]
        public static OptionX<T> Of<T>([AllowNull] T value)
            => value is null ? OptionX<T>.None : new OptionX<T>(value);

        // Unconstrained version: see Option<T>.None.
        [Pure]
        public static OptionX<T> None<T>() where T : notnull
            => OptionX<T>.None;

        [Pure]
        public static OptionX<T> Some<T>(T value) where T : struct
            => new OptionX<T>(value);

        [Pure]
        public static OptionX<T> SomeOrNone<T>(T? value) where T : struct
            => value.HasValue ? new OptionX<T>(value.Value) : OptionX<T>.None;

        [Pure]
        public static OptionX<T> SomeOrNone<T>(T? value) where T : class
            => value is null ? OptionX<T>.None : new OptionX<T>(value);
    }

    public sealed class OptionX<T>
    {
        public static readonly OptionX<T> None = new OptionX<T>();

        private readonly bool _isSome;
        [MaybeNull] [AllowNull] private readonly T _value;

        private OptionX()
        {
            _isSome = false;
            _value = default;
        }

        internal OptionX([DisallowNull] T value)
        {
            _isSome = true;
            _value = value;
        }

        public bool IsNone => !_isSome;

        [Pure]
        // Code size = 31 bytes.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue([MaybeNullWhen(false)] out T value)
        {
            if (_isSome)
            {
                value = _value!;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
    }
}
