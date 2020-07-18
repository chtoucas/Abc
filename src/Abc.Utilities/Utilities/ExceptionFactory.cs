﻿// See LICENSE in the project root for license information.

#pragma warning disable CA1303 // Do not pass literals as localized parameters.

namespace Abc.Utilities
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides static methods to create exceptions.
    /// <para>This class cannot be inherited.</para>
    /// </summary>
    internal static partial class ExceptionFactory { }

    internal partial class ExceptionFactory
    {
        public static InvalidOperationException ControlFlow =>
            new InvalidOperationException(
                "The flow of execution just reached a section of the code that should have been unreachable."
                + $"{Environment.NewLine}Most certainly signals a coding error. Please report.");

        public static readonly InvalidOperationException EmptySequence =
            new InvalidOperationException("The sequence was empty.");

        public static readonly NotSupportedException ReadOnlyCollection =
            new NotSupportedException("The collection is read-only.");
    }

    // Argument exceptions.
    internal partial class ExceptionFactory
    {
        [Pure]
        [DebuggerStepThrough]
        public static ArgumentException InvalidType(
            string paramName, Type expected, object obj)
            => new ArgumentException(
                $"The object should be of type {expected} but it is of type {obj.GetType()}.",
                paramName);
    }
}
