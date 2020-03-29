// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    using Anexn = System.ArgumentNullException;

    // Async methods.
    // NB: these async methods discard the context when they resume.
    public partial struct Maybe<T>
    {
        // TODO: async methods. Check args eagerly. Copy of this. Static AsyncNone.
        [Pure]
        public async Task<Maybe<TResult>> BindAsync<TResult>(
            Func<T, Task<Maybe<TResult>>> binder)
        {
            if (binder is null) { throw new Anexn(nameof(binder)); }

            return _isSome ? await binder(_value).ConfigureAwait(false)
                : Maybe<TResult>.None;

            //var @this = this;
            //return await __bind(@this).configureawait(false);

            //task<maybe<tresult>> __bind(maybe<t> @this)
            //    // bonsang! when _issome is true, _value is not null.
            //    => @this._issome ? binder(@this._value!)
            //        : task.fromresult(maybe<tresult>.none);
        }

        [Pure]
        public async Task<Maybe<TResult>> SelectAsync<TResult>(
            Func<T, Task<TResult>> selector)
        {
            if (selector is null) { throw new Anexn(nameof(selector)); }

            return _isSome ? Maybe.Of(await selector(_value).ConfigureAwait(false))
                : Maybe<TResult>.None;
        }

        [Pure]
        public async Task<Maybe<T>> OrElseAsync(Task<Maybe<T>> other)
        {
            if (other is null) { throw new Anexn(nameof(other)); }

            return _isSome ? this : await other.ConfigureAwait(false);
        }

        // REVIEW: version w/ Func<Task<TResult>>?
        [Pure]
        public async Task<TResult> SwitchAsync<TResult>(
            Func<T, Task<TResult>> caseSome, Task<TResult> caseNone)
        {
            if (_isSome)
            {
                if (caseSome is null) { throw new Anexn(nameof(caseSome)); }
                return await caseSome(_value).ConfigureAwait(false);
            }
            else
            {
                if (caseNone is null) { throw new Anexn(nameof(caseNone)); }
                return await caseNone.ConfigureAwait(false);
            }
        }
    }
}
