// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    using Anexn = System.ArgumentNullException;

    // TODO: async methods.

    // Async methods.
    // NB: these async methods discard the context when they resume.
    public partial struct Maybe<T>
    {
        [Pure]
        public Task<Maybe<TResult>> BindAsync<TResult>(
            Func<T, Task<Maybe<TResult>>> binder)
        {
            // Check arg eagerly.
            if (binder is null) { throw new Anexn(nameof(binder)); }

            return BindAsyncImpl(binder);
        }

        [Pure]
        private async Task<Maybe<TResult>> BindAsyncImpl<TResult>(
            Func<T, Task<Maybe<TResult>>> binder)
        {
            return _isSome ? await binder(_value).ConfigureAwait(false)
                : Maybe<TResult>.None;
        }

        [Pure]
        public Task<Maybe<TResult>> SelectAsync<TResult>(
            Func<T, Task<TResult>> selector)
        {
            // Check arg eagerly.
            if (selector is null) { throw new Anexn(nameof(selector)); }

            return SelectAsyncImpl(selector);
        }

        [Pure]
        private async Task<Maybe<TResult>> SelectAsyncImpl<TResult>(
            Func<T, Task<TResult>> selector)
        {
            return _isSome ? Maybe.Of(await selector(_value).ConfigureAwait(false))
                : Maybe<TResult>.None;
        }

        [Pure]
        public Task<Maybe<T>> OrElseAsync(Task<Maybe<T>> other)
        {
            // Check arg eagerly.
            if (other is null) { throw new Anexn(nameof(other)); }

            return OrElseAsyncImpl(other);
        }

        [Pure]
        private async Task<Maybe<T>> OrElseAsyncImpl(Task<Maybe<T>> other)
        {
            return _isSome ? this : await other.ConfigureAwait(false);
        }

        // Do not behave like the non-async Swith(), the method throws right
        // away when "caseSome" or "caseNone" is null.
        [Pure]
        public Task<TResult> SwitchAsync<TResult>(
            Func<T, Task<TResult>> caseSome, Task<TResult> caseNone)
        {
            // Check args eagerly.
            if (caseSome is null) { throw new Anexn(nameof(caseSome)); }
            if (caseNone is null) { throw new Anexn(nameof(caseNone)); }

            return SwitchAsyncImpl(caseSome, caseNone);
        }

        [Pure]
        private async Task<TResult> SwitchAsyncImpl<TResult>(
            Func<T, Task<TResult>> caseSome, Task<TResult> caseNone)
        {
            if (_isSome)
            {
                return await caseSome(_value).ConfigureAwait(false);
            }
            else
            {
                return await caseNone.ConfigureAwait(false);
            }
        }
    }
}
