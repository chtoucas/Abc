// See LICENSE.txt in the project root for license information.

using System.Threading.Tasks;

using Abc;

internal static class AsyncFakes
{
    // Completes synchronously.
    public static Task<Maybe<AnyResult>> Const<T>(T _) =>
        AnyResult.AsyncSome;

    public static async Task<Maybe<AnyResult>> ConstAsync<T>(T _)
    {
        await Task.Yield();
        return AnyResult.Some;
    }
}
