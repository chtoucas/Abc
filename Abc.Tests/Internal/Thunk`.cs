// See LICENSE.txt in the project root for license information.

using System;

internal static class Thunk<T>
    where T : notnull
{
    public static Func<T, TResult> Const<TResult>(TResult result)
        where TResult : notnull
        => x => result;
}

internal static class Thunk<T1, T2>
    where T1 : notnull
    where T2 : notnull
{
    public static Func<T1, T2, TResult> Const<TResult>(TResult result)
        where TResult : notnull
        => (x, y) => result;
}

internal static class Thunk<T1, T2, T3>
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
{
    public static Func<T1, T2, T3, TResult> Const<TResult>(TResult result)
        where TResult : notnull
        => (x, y, z) => result;
}

internal static class Thunk<T1, T2, T3, T4>
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
    where T4 : notnull
{
    public static Func<T1, T2, T3, T4, TResult> Const<TResult>(TResult result)
        where TResult : notnull
        => (x, y, z, a) => result;
}

internal static class Thunk<T1, T2, T3, T4, T5>
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
    where T4 : notnull
    where T5 : notnull
{
    public static Func<T1, T2, T3, T4, T5, TResult> Const<TResult>(TResult result)
        where TResult : notnull
        => (x, y, z, a, b) => result;
}
