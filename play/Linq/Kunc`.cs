// See LICENSE.txt in the project root for license information.

using System;
using System.Threading.Tasks;

using Abc;

internal static class Kunc<T, TResult>
    where T : notnull
    where TResult : notnull
{
    // Kleisli null.
    public static readonly Func<T, Maybe<TResult>> Null = default!;

    public static readonly Func<T, Maybe<TResult>> Any = x => default;

    public static readonly Func<T, Task<Maybe<TResult>>> NullAsync = default!;

}

internal static class Kunc<T1, T2, TResult>
    where T1 : notnull
    where T2 : notnull
    where TResult : notnull
{
    public static readonly Func<T1, T2, Maybe<TResult>> Null = default!;

    public static readonly Func<T1, T2, Maybe<TResult>> Any = (x, y) => default;
}
