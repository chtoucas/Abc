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
        // Throws ArgumentException.
        public static void ThrowsArgexn(string argName, Action testCode)
        {
            ArgumentException ex = Throws<ArgumentException>(testCode);
            Equal(argName, ex.ParamName);
        }

        // Throws ArgumentException.
        public static void ThrowsArgexn(string argName, Func<object> testCode)
        {
            ArgumentException ex = Throws<ArgumentException>(testCode);
            Equal(argName, ex.ParamName);
        }

        // Throws ArgumentNullException.
        public static void ThrowsAnexn(string argName, Action testCode)
        {
            ArgumentNullException ex = Throws<ArgumentNullException>(testCode);
            Equal(argName, ex.ParamName);
        }

        // Throws ArgumentNullException.
        public static void ThrowsAnexn(string argName, Func<object> testCode)
        {
            ArgumentNullException ex = Throws<ArgumentNullException>(testCode);
            Equal(argName, ex.ParamName);
        }

        // Throws ArgumentOutOfRangeException.
        public static void ThrowsAoorexn(string argName, Action testCode)
        {
            ArgumentOutOfRangeException ex = Throws<ArgumentOutOfRangeException>(testCode);
            Equal(argName, ex.ParamName);
        }

        // Throws ArgumentOutOfRangeException.
        public static void ThrowsAoorexn(string argName, Func<object> testCode)
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
            // Throws ArgumentNullException.
            public static async Task ThrowsAnexn(string argName, Func<Task> testCode)
            {
                if (IsNull(nameof(testCode), testCode)) { return; }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                try
                {
                    testCode();
                }
                catch (ArgumentException)
                {
                    throw new InvalidOperationException(
                        "The specified task performs eager validation.");
                }
#pragma warning restore CS4014

                ArgumentNullException ex =
                    await ThrowsAsync<ArgumentNullException>(testCode);
                Equal(argName, ex.ParamName);
            }
        }
    }
}
