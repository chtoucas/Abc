﻿// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

namespace Abc.Edu.Linq
{
    using System;

    public interface IQuerySyntax
    {
        IQuerySyntax<T> Cast<T>();
    }

    public interface IQuerySyntax<T> : IQuerySyntax
    {
        IQuerySyntax<T> Where(Func<T, bool> predicate);

        IQuerySyntax<TResult> Select<TResult>(Func<T, TResult> selector);

        IQuerySyntax<TResult> SelectMany<TMiddle, TResult>(
            Func<T, IQuerySyntax<TMiddle>> selector,
            Func<T, TMiddle, TResult> resultSelector);

        IQuerySyntax<TResult> Join<TInner, TKey, TResult>(
            IQuerySyntax<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultSelector);

        IQuerySyntax<TResult> GroupJoin<TInner, TKey, TResult>(
            IQuerySyntax<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, IQuerySyntax<TInner>, TResult> resultSelector);

        IOrderedQuerySyntax<T> OrderBy<TKey>(Func<T, TKey> keySelector);

        IOrderedQuerySyntax<T> OrderByDescending<TKey>(Func<T, TKey> keySelector);

        IQuerySyntax<IGroupingQuerySyntax<TKey, T>> GroupBy<TKey>(Func<T, TKey> keySelector);

        IQuerySyntax<IGroupingQuerySyntax<TKey, TElement>> GroupBy<TKey, TElement>(
            Func<T, TKey> keySelector,
            Func<T, TElement> elementSelector);
    }
}
