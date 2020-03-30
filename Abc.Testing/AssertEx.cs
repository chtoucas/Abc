// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Threading.Tasks;

    using Xunit;

    public sealed partial class AssertEx : Assert
    {
        private AssertEx() { }

        public static partial class Async { }

        private static bool IsNull<T>(string paramName, [ValidatedNotNull] T obj)
            where T : notnull
        {
            if (obj is null)
            {
                True(false, $"{paramName} was null.");
                return true;
            }
            return false;
        }
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

        // Threw ArgumentOutOfRangeException.
        public static void ThrowsAoorEx(string argName, Action testCode)
        {
            ArgumentOutOfRangeException ex = Throws<ArgumentOutOfRangeException>(testCode);
            Equal(argName, ex.ParamName);
        }

        // Threw ArgumentOutOfRangeException.
        public static void ThrowsAoorEx(string argName, Func<object> testCode)
        {
            ArgumentOutOfRangeException ex = Throws<ArgumentOutOfRangeException>(testCode);
            Equal(argName, ex.ParamName);
        }
    }

    // Async.
    public partial class AssertEx
    {
        public partial class Async
        {
            // Threw ArgumentNullException.
            public static async Task ThrowsArgNullEx(string argName, Func<Task> testCode)
            {
                ArgumentNullException ex =
                    await ThrowsAsync<ArgumentNullException>(testCode).ConfigureAwait(false);
                Equal(argName, ex.ParamName);
            }
        }
    }
}
