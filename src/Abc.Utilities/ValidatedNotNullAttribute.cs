// See LICENSE in the project root for license information.

#nullable enable

namespace Abc.Utilities
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Decorating a parameter with this attribute informs the Code Analysis
    /// tool that the method is validating the parameter against null value.
    /// <para>This class cannot be inherited.</para>
    /// </summary>
    /// <remarks>
    /// Using this attribute suppresses the CA1062 warning.
    /// </remarks>
#if !ABC_UTILITIES_ENABLE_CODE_COVERAGE
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
    [DebuggerNonUserCode]
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    internal sealed class ValidatedNotNullAttribute : Attribute { }
}
