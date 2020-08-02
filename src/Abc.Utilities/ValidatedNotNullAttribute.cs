// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

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
    [DebuggerNonUserCode, _ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    internal sealed class ValidatedNotNullAttribute : Attribute { }
}
