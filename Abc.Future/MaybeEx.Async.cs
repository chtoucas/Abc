// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    using Anexn = System.ArgumentNullException;

    // Async vs async...
    // Maybe<Task<T>>, Task<Maybe<T>>.
    // https://devblogs.microsoft.com/dotnet/configureawait-faq/
    // https://ericlippert.com/2020/03/10/passing-awaited-tasks/

    // Async methods.
    // Not extension methods: we already have instance methods with the same
    // names.
    // NB: these async methods discard the context when they resume.
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
        public static async Task<Maybe<T>> OrElseAsync<T>(
            Maybe<T> maybe,
            Task<Maybe<T>> other)
        {
            if (other is null) { throw new Anexn(nameof(other)); }

            return maybe.IsNone ? await other.ConfigureAwait(false) : maybe;
        }

        [Pure]
        public static async Task<TResult> SwitchAsync<T, TResult>(
            Maybe<T> maybe,
            Func<T, Task<TResult>> caseSome, Task<TResult> caseNone)
        {
            if (maybe.TryGetValue(out T value))
            {
                if (caseSome is null) { throw new Anexn(nameof(caseSome)); }
                return await caseSome(value).ConfigureAwait(false);
            }
            else
            {
                if (caseNone is null) { throw new Anexn(nameof(caseNone)); }
                return await caseNone.ConfigureAwait(false);
            }
        }

        //
        // Configurable.
        //

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
