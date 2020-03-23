// See LICENSE.txt in the project root for license information.

using System;
using System.Threading.Tasks;

internal static class Funk<TResult>
    where TResult : notnull
{
    public static readonly Func<TResult> Null = default!;

    public static readonly Func<TResult> Any = () => default!;
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

    public static readonly Func<T1, T2, Task<TResult>> NullAsync = default!;

    public static readonly Func<T1, T2, Task<TResult>> AnyAsync = (x, y) => default!;
}
