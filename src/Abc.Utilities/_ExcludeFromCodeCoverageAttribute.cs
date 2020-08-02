// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

#pragma warning disable IDE1006 // Naming Styles

#nullable enable

namespace Abc.Utilities
{
    using System;

    [AttributeUsage(
        AttributeTargets.All,
        AllowMultiple = false,
        Inherited = false)]
    internal sealed class _ExcludeFromCodeCoverageAttribute : Attribute { }
}
