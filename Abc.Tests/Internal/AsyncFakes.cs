// See LICENSE.txt in the project root for license information.

using System;
using System.Threading.Tasks;

using Abc;

internal static class AsyncFakes
{
    private static readonly Task<Maybe<AnyResult>> s_AsyncNone
        = Task.FromResult(Maybe<AnyResult>.None);

    private static readonly Task<Maybe<AnyResult>> s_AsyncSome
        = Task.FromResult(AnyResult.Some);

    public static readonly Task<AnyResult> AsyncValue
        = Task.FromResult(AnyResult.Value);

    // Completes synchronously.
    public static Task<Maybe<AnyResult>> ReturnSome<T>(T _) => s_AsyncSome;

    // Completes synchronously.
    public static Task<Maybe<AnyResult>> ReturnNone<T>(T _) => s_AsyncNone;

    public static async Task<Maybe<AnyResult>> ReturnSomeAsync<T>(T _)
    {
        await Task.Yield();
        return AnyResult.Some;
    }

    public static async Task<Maybe<AnyResult>> ReturnNoneAsync<T>(T _)
    {
        await Task.Yield();
        return AnyResult.None;
    }

    public static Func<T, Task<TResult>> FromSync<T, TResult>(Func<T, TResult> func)
    {
        return async x => { await Task.Yield(); return func(x); };
    }

    public static Func<T, Task<Maybe<TResult>>> FromSync<T, TResult>(Func<T, Maybe<TResult>> func)
    {
        return async x => { await Task.Yield(); return func(x); };
    }
}
