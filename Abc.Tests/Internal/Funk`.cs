// See LICENSE.txt in the project root for license information.

using System;
using System.Threading.Tasks;

using Abc;

internal static class Funk<TResult>
    where TResult : notnull
{
    public static readonly Func<TResult> Null = default!;
}

internal static class Funk<T, TResult>
    where T : notnull
    where TResult : notnull
{
    public static readonly Func<T, TResult> Null = default!;

    public static readonly Func<T, Task<TResult>> NullAsync = default!;

    // Kleisli null.
    public static readonly Func<T, Maybe<TResult>> Kull = default!;

    public static readonly Func<T, Task<Maybe<TResult>>> KullAsync = default!;

}

internal static class Funk<T1, T2, TResult>
    where T1 : notnull
    where T2 : notnull
    where TResult : notnull
{
    public static readonly Func<T1, T2, TResult> Null = default!;

    // Kleisli null.
    public static readonly Func<T1, T2, Maybe<TResult>> Kull = default!;
}
