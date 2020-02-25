// See LICENSE.txt in the project root for license information.

namespace Play
{
    using System;

    /// <summary>
    /// Decorating a parameter with this attribute informs the Code Analysis
    /// tool that the method is validating the parameter against null value.
    /// </summary>
    /// <remarks>
    /// Using this attribute suppresses the CA1062 warning.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    internal sealed class ValidatedNotNullAttribute : Attribute { }

    internal static class Require
    {
        public static void NotNull<T>([ValidatedNotNull]T value, string paramName)
            where T : class
        {
            if (value is null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
