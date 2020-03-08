// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;

    using Xunit;

    public sealed partial class AssertEx : Assert { }

    public partial class AssertEx
    {
        // Threw ArgumentException.
        public static void ThrowsArgEx(string argName, Action testCode)
        {
            var ex = Throws<ArgumentException>(testCode);
            Equal(argName, ex.ParamName);
        }

        // Threw ArgumentException.
        public static void ThrowsArgEx(string argName, Func<object> testCode)
        {
            var ex = Throws<ArgumentException>(testCode);
            Equal(argName, ex.ParamName);
        }

        // Threw ArgumentNullException.
        public static void ThrowsArgNullEx(string argName, Action testCode)
        {
            var ex = Throws<ArgumentNullException>(testCode);
            Equal(argName, ex.ParamName);
        }

        // Threw ArgumentNullException.
        public static void ThrowsArgNullEx(string argName, Func<object> testCode)
        {
            var ex = Throws<ArgumentNullException>(testCode);
            Equal(argName, ex.ParamName);
        }
    }
}
