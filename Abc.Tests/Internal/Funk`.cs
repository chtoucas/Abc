// See LICENSE.txt in the project root for license information.

using System;
using System.Threading.Tasks;

internal static class Funk<TResult>
    where TResult : notnull
{
    public static readonly Func<TResult> Null = default!;

    public static readonly Func<TResult> Any = () => default!;

    public static readonly Func<Task<TResult>> NullAsync = default!;

    public static readonly Func<Task<TResult>> AnyAsync = () => default!;
}

internal static class Funk<T, TResult>
    where T : notnull
    where TResult : notnull
{
    public static readonly Func<T, TResult> Null = default!;

    public static readonly Func<T, TResult> Any = x => default!;

    public static readonly Func<T, Task<TResult>> NullAsync = default!;

    public static readonly Func<T, Task<TResult>> AnyAsync = x => default!;

}

internal static class Funk<T1, T2, TResult>
    where T1 : notnull
    where T2 : notnull
    where TResult : notnull
{
    public static readonly Func<T1, T2, TResult> Null = default!;

    public static readonly Func<T1, T2, TResult> Any = (x, y) => default!;
}

internal static class Funk<T1, T2, T3, TResult>
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
    where TResult : notnull
{
    public static readonly Func<T1, T2, T3, TResult> Null = default!;
}

internal static class Funk<T1, T2, T3, T4, TResult>
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
    where T4 : notnull
    where TResult : notnull
{
    public static readonly Func<T1, T2, T3, T4, TResult> Null = default!;
}

internal static class Funk<T1, T2, T3, T4, T5, TResult>
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
    where T4 : notnull
    where T5 : notnull
    where TResult : notnull
{
    public static readonly Func<T1, T2, T3, T4, T5, TResult> Null = default!;
}
