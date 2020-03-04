// See LICENSE.txt in the project root for license information.

namespace Abc.Fx
{
    using System;
    using System.Threading.Tasks;

    // Configurable core async methods?
    // https://devblogs.microsoft.com/dotnet/configureawait-faq/
    public partial class Mayhap
    {
        public static async Task<Mayhap<TResult>> BindAsync<T, TResult>(
            this Mayhap<T> @this,
            Func<T, Task<Mayhap<TResult>>> binder,
            bool continueOnCapturedContext)
        {
            Require.NotNull(binder, nameof(binder));

            using var iter = @this.GetEnumerator();

            return iter.MoveNext()
                ? await binder(iter.Current).ConfigureAwait(continueOnCapturedContext)
                : Mayhap<TResult>.None;
        }

        public static async Task<Mayhap<TResult>> SelectAsync<T, TResult>(
            this Mayhap<T> @this,
            Func<T, Task<TResult>> selector,
            bool continueOnCapturedContext)
        {
            Require.NotNull(selector, nameof(selector));

            using var iter = @this.GetEnumerator();

            return iter.MoveNext()
                ? Mayhap<TResult>.η(await selector(iter.Current).ConfigureAwait(continueOnCapturedContext))
                : Mayhap<TResult>.None;
        }
    }

    // Async extensions.
    public partial class Mayhap
    {
        public static async Task<Mayhap<T>> WhereAsync<T>(
            this Mayhap<T> @this,
            Func<T, Task<bool>> predicate)
        {
            Require.NotNull(predicate, nameof(predicate));

            return await @this.BindAsync(__binder).ConfigureAwait(false);

            async Task<Mayhap<T>> __binder(T x)
                => await predicate(x).ConfigureAwait(false) ? Mayhap<T>.Some(x)
                    : Mayhap<T>.None;
        }
    }
}
