﻿// See LICENSE.txt in the project root for license information.

using System.Threading.Tasks;

using Abc;

internal static class AsyncFakes
{
    public static Task<Maybe<AnyResult>> Const<T>(T _)
    {
        return AnyResult.AsyncSome;
    }

    public static async Task<Maybe<AnyResult>> ConstAsync<T>(T _)
    {
        await Task.Delay(100).ConfigureAwait(false);
        return AnyResult.Some;
    }
}