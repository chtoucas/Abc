// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Threading.Tasks;

    using Xunit;

    public sealed partial class AssertEx : Assert
    {
        private AssertEx() { }
    }

    public partial class AssertEx
    {
        // Threw ArgumentException.
        public static void ThrowsArgEx(string argName, Action testCode)
        {
            ArgumentException ex = Throws<ArgumentException>(testCode);
            Equal(argName, ex.ParamName);
        }

        // Threw ArgumentException.
        public static void ThrowsArgEx(string argName, Func<object> testCode)
        {
            ArgumentException ex = Throws<ArgumentException>(testCode);
            Equal(argName, ex.ParamName);
        }

        // Threw ArgumentNullException.
        public static void ThrowsArgNullEx(string argName, Action testCode)
        {
            ArgumentNullException ex = Throws<ArgumentNullException>(testCode);
            Equal(argName, ex.ParamName);
        }

        // Threw ArgumentNullException.
        public static void ThrowsArgNullEx(string argName, Func<object> testCode)
        {
            ArgumentNullException ex = Throws<ArgumentNullException>(testCode);
            Equal(argName, ex.ParamName);
        }

#pragma warning disable CA1034 // Nested types should not be visible
        public static class Async
#pragma warning restore CA1034
        {
            // Threw ArgumentNullException.
            public static void ThrowsArgNullEx(string argName, Func<Task> testCode)
            {
                Task<ArgumentNullException> ex = ThrowsAsync<ArgumentNullException>(testCode);
                Equal(argName, ex.Result.ParamName);
            }
        }
    }
}
