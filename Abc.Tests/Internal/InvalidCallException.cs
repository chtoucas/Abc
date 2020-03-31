// See LICENSE.txt in the project root for license information.

#pragma warning disable CA1032 // Implement standard exception constructors
#pragma warning disable CA1303 // Do not pass literals as localized parameters.

using System;

internal sealed class InvalidCallException : InvalidOperationException
{
    public InvalidCallException() : base("Unexpected call.") { }
}
