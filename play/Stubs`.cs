// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;

    internal static class Stubs
    {
        public static readonly Action Noop = () => { };
    }

    internal static class Stubs<T>
    {
        /// <summary>id</summary>
        public static readonly Func<T, T> Ident = x => x;

        public static readonly Action<T> Noop = _ => { };
    }

    internal static class Stubs<T1, T2>
    {
        /// <summary>const</summary>
        public static readonly Func<T1, T2, T1> Const1 = (x, _) => x;

        /// <summary>flip const</summary>
        public static readonly Func<T1, T2, T2> Const2 = (_, x) => x;
    }
}
