// See LICENSE.txt in the project root for license information.

using System.Threading.Tasks;

using Abc;

internal static class AsyncFakes
{
    // Completes synchronously.
    public static Task<Maybe<AnyResult>> ReturnSome<T>(T _) =>
        AnyResult.AsyncSome;

    // Completes synchronously.
    public static Task<Maybe<AnyResult>> ReturnNone<T>(T _) =>
        AnyResult.AsyncNone;

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
}
