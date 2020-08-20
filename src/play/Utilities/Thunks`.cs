// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

namespace Abc.Utilities
{
    using System;

    internal static class Thunks<T1, T2>
    {
        /// <summary>const</summary>
        public static readonly Func<T1, T2, T1> Const1 = (x, _) => x;

        /// <summary>flip const</summary>
        public static readonly Func<T1, T2, T2> Const2 = (_, x) => x;
    }
}
