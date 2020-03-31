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
}

internal static class AsyncFakes<TResult>
    where TResult : notnull
{
    public static readonly Func<Task<TResult>> Null = null!;

    public static readonly Func<Task<TResult>> Any = () => throw new FakeCallException();
}

internal static class AsyncFakes<T, TResult>
    where T : notnull
    where TResult : notnull
{
    public static readonly Func<T, Task<TResult>> Null = null!;

    public static readonly Func<T, Task<TResult>> Any = x => throw new FakeCallException();

    public static Func<T, Task<TResult>> FromSync(Func<T, TResult> func)
    {
        return async x => { await Task.Yield(); return func(x); };
    }

    public static Func<T, Task<Maybe<TResult>>> FromSync(Func<T, Maybe<TResult>> func)
    {
        return async x => { await Task.Yield(); return func(x); };
    }
}
