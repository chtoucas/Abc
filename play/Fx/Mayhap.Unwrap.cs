// See LICENSE.txt in the project root for license information.

namespace Abc.Fx
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    // Pattern matching.
    public partial class Mayhap
    {
        public static void OnSome<T>(this Mayhap<T> @this, Action<T> action)
            => @this.Do(action, Stubs.Noop);

        public static void OnNone<T>(this Mayhap<T> @this, Action action)
            => @this.Do(Stubs<T>.Noop, action);

        [Pure]
        [return: MaybeNull]
        public static T ValueOrDefault<T>(this Mayhap<T> @this)
            => @this.SwitchIntern(Stubs<T>.Ident, (T)(default!));

        [Pure]
        public static T ValueOrElse<T>(this Mayhap<T> @this, [DisallowNull]T other)
            => @this.SwitchIntern(Stubs<T>.Ident, other);

        [Pure]
        public static T ValueOrElse<T>(this Mayhap<T> @this, Func<T> valueFactory)
        {
            return @this.SwitchIntern(Stubs<T>.Ident, __caseNone);

            T __caseNone()
            {
                Require.NotNull(valueFactory, nameof(valueFactory));
                return valueFactory();
            }
        }

        [Pure]
        public static T ValueOrThrow<T>(this Mayhap<T> @this)
            => @this.SwitchIntern(Stubs<T>.Ident, () => throw new InvalidOperationException());

        [Pure]
        public static T ValueOrThrow<T>(this Mayhap<T> @this, Func<Exception> exceptionFactory)
        {
            return @this.SwitchIntern(Stubs<T>.Ident, __caseNone);

            T __caseNone()
            {
                Require.NotNull(exceptionFactory, nameof(exceptionFactory));
                throw exceptionFactory();
            }
        }
    }
}
