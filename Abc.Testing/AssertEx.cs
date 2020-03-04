// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;

    using Xunit;

    public sealed partial class AssertEx : Assert { }

    public partial class AssertEx
    {
        // Throws an ArgumentException.
        public static void ThrowsArgEx(string argName, Action testCode)
        {
            var ex = Throws<ArgumentException>(testCode);
            Equal(argName, ex.ParamName);
        }

        // Throws an ArgumentException.
        public static void ThrowsArgEx(string argName, Func<object> testCode)
        {
            var ex = Throws<ArgumentException>(testCode);
            Equal(argName, ex.ParamName);
        }

        // Throws an ArgumentNullException.
        public static void ThrowsArgNullEx(string argName, Action testCode)
        {
            var ex = Throws<ArgumentNullException>(testCode);
            Equal(argName, ex.ParamName);
        }

        // Throws an ArgumentNullException.
        public static void ThrowsArgNullEx(string argName, Func<object> testCode)
        {
            var ex = Throws<ArgumentNullException>(testCode);
            Equal(argName, ex.ParamName);
        }
    }

    public partial class AssertEx
    {
        public static void None<T>(Maybe<T> maybe)
            => True(maybe.IsNone);

        public static void Some<T>(Maybe<T> maybe)
            => False(maybe.IsNone);

        // TODO: à améliorer.
        public static void Some<T>(T exp, Maybe<T> maybe)
        {
            //False(maybe.IsNone);
            //True(maybe.Contains(exp));

            True(maybe.IsSome);
            Equal(exp, maybe.Value);
        }
    }
}
