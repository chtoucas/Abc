// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using Abc.Utilities;

    using Anexn = System.ArgumentNullException;

    // REVIEW: lazy extensions.

    // Experimental helpers & extension methods for Maybe<T>.
    public static partial class MaybeEx { }

    // Async methods.
    public partial class MaybeEx
    {
        // Configurable core async methods?
        // https://devblogs.microsoft.com/dotnet/configureawait-faq/
        // Do not use the enumerator.

        [Pure]
        public static async Task<Maybe<TResult>> BindAsync<T, TResult>(
            this Maybe<T> @this,
            Func<T, Task<Maybe<TResult>>> binder,
            bool continueOnCapturedContext)
        {
            Require.NotNull(binder, nameof(binder));

            using var iter = @this.GetEnumerator();

            if (iter.MoveNext())
            {
                return await binder(iter.Current)
                    .ConfigureAwait(continueOnCapturedContext);
            }
            else
            {
                return Maybe<TResult>.None;
            }
        }

        [Pure]
        public static async Task<Maybe<TResult>> SelectAsync<T, TResult>(
            this Maybe<T> @this,
            Func<T, Task<TResult>> selector,
            bool continueOnCapturedContext)
        {
            Require.NotNull(selector, nameof(selector));

            using var iter = @this.GetEnumerator();

            if (iter.MoveNext())
            {
                TResult result = await selector(iter.Current)
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
            this Maybe<T> @this,
            Func<T, Task<bool>> predicate)
        {
            Require.NotNull(predicate, nameof(predicate));

            return await @this.BindAsync(__binder).ConfigureAwait(false);

            async Task<Maybe<T>> __binder(T x)
                => await predicate(x).ConfigureAwait(false) ? @this : Maybe<T>.None;
        }
    }

    // Side effects.
    public partial class MaybeEx
    {
        public static void OnNone<T>(this Maybe<T> @this, Action action)
        {
            if (@this.IsNone)
            {
                if (action is null) { throw new Anexn(nameof(action)); }
                action();
            }
        }

        // Fluent version of Do(). Beware, does not throw for null actions.
        // The version with exceptions does not really deserve to be included:
        //   @this.Do(caseSome, caseNone);
        //   return @this;
        [Pure]
        public static Maybe<T> OnSomeOrNone<T>(
            this Maybe<T> @this, Action<T>? caseSome, Action? caseNone)
        {
            if (@this.IsNone)
            {
                caseNone?.Invoke();
            }
            else
            {
                // To be replaced by @this.Value if promoted to the main proj.
                caseSome?.Invoke(@this.ValueOrDefault());
            }
            return @this;
        }
    }

    // Misc methods.
    public partial class MaybeEx
    {
        // These are not the "when" and "unless" of the Maybe monad but rather
        // specialized versions of Where() when the state of the maybe and the
        // value it encloses are not taken into account.

        [Pure]
        public static Maybe<T> When<T>(this Maybe<T> @this, bool predicate)
            => predicate ? @this : Maybe<T>.None;

        // Reverse of When().
        [Pure]
        public static Maybe<T> Unless<T>(this Maybe<T> @this, bool predicate)
            => predicate ? Maybe<T>.None : @this;
    }

    // Helpers related to IEnumerable<>.
    // TODO: to be optimized if moved to main proj.
    public static partial class MaybeEx
    {
        [Pure]
        public static Maybe<IEnumerable<T>> EmptyIfNone<T>(this Maybe<IEnumerable<T>> @this)
            => @this.OrElse(Maybe.Empty<T>());

        // What it should do:
        // - if the input sequence is empty, returns the maybe of an empty
        //   sequence.
        // - if all maybe's in the input sequence are empty, returns the empty
        //   maybe of type IEnumerable<T>.
        // - otherwise, returns the maybe of the sequence of wrapped values.
        // See also CollectAny().
        [Pure]
        public static Maybe<IEnumerable<T>> ToMaybe<T>(IEnumerable<Maybe<T>> source)
        {
            return source.Aggregate(
                //Maybe.Empty<T>(),
                Maybe<IEnumerable<T>>.None,
                (x, y) => x.ZipWith(y, Enumerable.Append));
        }

        #region Sum()

        [Pure]
        public static Maybe<T> Sum<T>(IEnumerable<Maybe<T>> source, Func<T, T, T> add, T zero)
        {
            Require.NotNull(add, nameof(add));

            Maybe<IEnumerable<T>> aggr = source.Aggregate(
                Maybe.Empty<T>(),
                (x, y) => x.ZipWith(y, Enumerable.Append));

            return aggr.Select(__sum);

            T __sum(IEnumerable<T> seq)
            {
                T sum = zero;
                foreach (var item in seq)
                {
                    sum = add(sum, item);
                }
                return sum;
            }
        }

        // Add Sum() for all simple value types.

        [Pure]
        public static Maybe<int> Sum(IEnumerable<Maybe<int>> source)
        {
            Maybe<IEnumerable<int>> aggr = source.Aggregate(
                Maybe.Empty<int>(),
                (x, y) => x.ZipWith(y, Enumerable.Append));

            return aggr.Select(Enumerable.Sum);
        }

        #endregion
    }

    // Extension methods for Maybe<T> where T is disposable.
    public partial class MaybeEx
    {
        // Bind() with automatic resource cleanup.
        // F# Workflow: use.
        [Pure]
        public static Maybe<TResult> Use<TDisposable, TResult>(
            this Maybe<TDisposable> @this,
            Func<TDisposable, Maybe<TResult>> binder)
            where TDisposable : IDisposable
        {
            if (binder is null) { throw new Anexn(nameof(binder)); }

            return @this.Bind(x => { using (x) { return binder(x); } });
        }

        // Select() with automatic resource cleanup.
        [Pure]
        public static Maybe<TResult> Use<TDisposable, TResult>(
            this Maybe<TDisposable> @this,
            Func<TDisposable, TResult> selector)
            where TDisposable : IDisposable
        {
            if (selector is null) { throw new Anexn(nameof(selector)); }

            return @this.Select(x => { using (x) { return selector(x); } });
        }
    }

    // Extensions methods for Maybe<T> where T is an XElement.
    public partial class MaybeEx
    {
        [Pure]
        public static Maybe<T> MapValue<T>(
            this Maybe<XElement> @this, Func<string, T> selector)
            => from x in @this select selector(x.Value);

        [Pure]
        public static Maybe<string> ValueOrNone(this Maybe<XElement> @this)
            => from x in @this select x.Value;
    }

    // Extensions methods for Maybe<T> where T is an XAttribute.
    public partial class MaybeEx
    {
        [Pure]
        public static Maybe<T> MapValue<T>(
            this Maybe<XAttribute> @this, Func<string, T> selector)
            => from x in @this select selector(x.Value);

        [Pure]
        public static Maybe<string> ValueOrNone(this Maybe<XAttribute> @this)
            => from x in @this select x.Value;
    }
}
