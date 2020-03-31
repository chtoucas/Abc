﻿// See LICENSE.txt in the project root for license information.

using System;

internal static class Funk<TResult>
    where TResult : notnull
{
    public static readonly Func<TResult> Null = null!;

    public static readonly Func<TResult> Any = () => throw new FakeCallException();
}

internal static class Funk<T, TResult>
    where T : notnull
    where TResult : notnull
{
    public static readonly Func<T, TResult> Null = null!;

    public static readonly Func<T, TResult> Any = _ => throw new FakeCallException();

}

internal static class Funk<T1, T2, TResult>
    where T1 : notnull
    where T2 : notnull
    where TResult : notnull
{
    public static readonly Func<T1, T2, TResult> Null = null!;

    public static readonly Func<T1, T2, TResult> Any = (x, y) => throw new FakeCallException();
}

internal static class Funk<T1, T2, T3, TResult>
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
    where TResult : notnull
{
    public static readonly Func<T1, T2, T3, TResult> Null = null!;
}

internal static class Funk<T1, T2, T3, T4, TResult>
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
    where T4 : notnull
    where TResult : notnull
{
    public static readonly Func<T1, T2, T3, T4, TResult> Null = null!;
}

internal static class Funk<T1, T2, T3, T4, T5, TResult>
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
    where T4 : notnull
    where T5 : notnull
    where TResult : notnull
{
    public static readonly Func<T1, T2, T3, T4, T5, TResult> Null = null!;
}
