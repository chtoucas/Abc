// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

namespace Abc.Edu.Linq
{
    using System;

    public interface IOrderedQuerySyntax<T> : IQuerySyntax<T>
    {
        IOrderedQuerySyntax<T> ThenBy<TKey>(Func<T, TKey> keySelector);

        IOrderedQuerySyntax<T> ThenByDescending<TKey>(Func<T, TKey> keySelector);
    }
}
