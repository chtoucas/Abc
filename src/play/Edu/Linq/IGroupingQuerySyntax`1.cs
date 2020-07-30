// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

namespace Abc.Edu.Linq
{
    public interface IGroupingQuerySyntax<TKey, T> : IQuerySyntax<T>
    {
        TKey Key { get; }
    }
}
