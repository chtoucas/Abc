// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable

using System.Collections;
using System.Collections.Generic;

// Local additions to EnumerableTests.

namespace System.Linq.Tests
{
    public abstract partial class EnumerableTests
    {
        protected static IEnumerable<T> EmptySource<T>()
        {
            yield break;
        }
    }
}