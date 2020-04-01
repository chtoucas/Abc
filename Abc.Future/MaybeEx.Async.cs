// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    using Anexn = System.ArgumentNullException;

    // REVIEW: Async vs async...
    // Maybe<Task<T>>, Task<Maybe<T>>, Func<Task<>> vs Task<>.

    // Async methods.
    // Not extension methods: we already have instance methods with the same
    // names.
    public partial class MaybeEx
    {
        [Pure]
        public static async Task<Maybe<TResult>> BindAsync<T, TResult>(
            Maybe<T> maybe,
            Func<T, Task<Maybe<TResult>>> binder)
        {
            if (binder is null) { throw new Anexn(nameof(binder)); }

            if (maybe.TryGetValue(out T value))
            {
                return await binder(value).ConfigureAwait(false);
            }
            else
            {
                return Maybe<TResult>.None;
            }
        }

        [Pure]
        public static async Task<Maybe<TResult>> BindAsync<T, TResult>(
            Maybe<T> maybe,
            Func<T, Task<Maybe<TResult>>> binder,
            bool continueOnCapturedContext)
        {
            if (binder is null) { throw new Anexn(nameof(binder)); }

            if (maybe.TryGetValue(out T value))
            {
                return await binder(value)
                    .ConfigureAwait(continueOnCapturedContext);
            }
            else
            {
                return Maybe<TResult>.None;
            }
        }

        [Pure]
        public static async Task<Maybe<TResult>> SelectAsync<T, TResult>(
            Maybe<T> maybe,
            Func<T, Task<TResult>> selector)
        {
            if (selector is null) { throw new Anexn(nameof(selector)); }

            if (maybe.TryGetValue(out T value))
            {
                TResult result = await selector(value).ConfigureAwait(false);
                return Maybe.Of(result);
            }
            else
            {
                return Maybe<TResult>.None;
            }
        }

        [Pure]
        public static async Task<Maybe<TResult>> SelectAsync<T, TResult>(
            Maybe<T> maybe,
            Func<T, Task<TResult>> selector,
            bool continueOnCapturedContext)
        {
            if (selector is null) { throw new Anexn(nameof(selector)); }

            if (maybe.TryGetValue(out T value))
            {
                TResult result = await selector(value)
                    .ConfigureAwait(continueOnCapturedContext);
                return Maybe.Of(result);
            }
            else
            {
                return Maybe<TResult>.None;
            }
        }

        [Pure]
        public static async Task<Maybe<T>> OrElseAsync<T>(
            Maybe<T> maybe,
            Func<Task<Maybe<T>>> other)
        {
            if (other is null) { throw new Anexn(nameof(other)); }

            return maybe.IsNone ? await other().ConfigureAwait(false) : maybe;
        }

        [Pure]
        public static async Task<Maybe<T>> OrElseAsync<T>(
            Maybe<T> maybe,
            Func<Task<Maybe<T>>> other,
            bool continueOnCapturedContext)
        {
            if (other is null) { throw new Anexn(nameof(other)); }

            return maybe.IsNone ? await other().ConfigureAwait(continueOnCapturedContext)
                : maybe;
        }

        //
        // More async methods.
        //

        [Pure]
        public static async Task<Maybe<T>> WhereAsync<T>(
            Maybe<T> maybe,
            Func<T, Task<bool>> predicate)
        {
            if (predicate is null) { throw new Anexn(nameof(predicate)); }

            return await maybe.BindAsync(__binder).ConfigureAwait(false);

            async Task<Maybe<T>> __binder(T x)
                => await predicate(x).ConfigureAwait(false) ? maybe : Maybe<T>.None;
        }
    }
}
