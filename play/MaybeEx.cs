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

    // REVIEW: lazy extensions. Code to be optimized if moved to main proj.

    // Experimental helpers & extension methods for Maybe<T>.
    public static partial class MaybeEx { }

    // Async methods.
    public partial class MaybeEx
    {
        // Configurable core async methods?
        // https://devblogs.microsoft.com/dotnet/configureawait-faq/

        [Pure]
        public static async Task<Maybe<TResult>> BindAsync<T, TResult>(
            this Maybe<T> @this,
            Func<T, Task<Maybe<TResult>>> binder,
            bool continueOnCapturedContext)
        {
            Require.NotNull(binder, nameof(binder));

            if (@this.TryGetValue(out T value))
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
            this Maybe<T> @this,
            Func<T, Task<TResult>> selector,
            bool continueOnCapturedContext)
        {
            Require.NotNull(selector, nameof(selector));

            if (@this.TryGetValue(out T value))
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

        // Fluent versions? They are easy to add locally. For instance,
        //   @this.Do(caseSome, caseNone);
        //   return @this;
        //
        // Might be worth including when the action only depends on an external
        // condition, not on the maybe. Purpose: debugging, logging.
        // By the way, the "non-fluent" versions of the methods below are
        // useless.
        //
        // Beware, do not throw for null actions.
        // No attr [Pure], even if fluent API, we should be able to write
        // maybe.(..anything..).When(...).ReplaceWith(...) BUT also
        // maybe.(..anything..).When(...) without gettting a warning.
        public static Maybe<T> When<T>(
            this Maybe<T> @this, bool condition, Action? action)
        {
            if (condition)
            {
                action?.Invoke();
            }
            return @this;
        }

        // Reverse of When().
        public static Maybe<T> Unless<T>(
            this Maybe<T> @this, bool condition, Action? action)
        {
            return When(@this, !condition, action);
        }
    }

    // Misc methods.
    public partial class MaybeEx
    {
        // Specialized version of Where() when the state of the maybe and the
        // value it encloses are not taken into account.

        [Pure]
        public static Maybe<T> Filter<T>(this Maybe<T> @this, bool condition)
            => condition ? @this : Maybe<T>.None;
    }

    // Helpers related to IEnumerable<>.
    public static partial class MaybeEx
    {
        [Pure]
        public static Maybe<IEnumerable<T>> EmptyIfNone<T>(this Maybe<IEnumerable<T>> @this)
            => @this.OrElse(Maybe.EmptyEnumerable<T>());

        // What it should do:
        // - If the input sequence is empty,
        //   returns Maybe.Of(empty sequence).
        // - If all maybe's in the input sequence are empty,
        //   returns Maybe<IEnumerable<T>>.None.
        // - Otherwise,
        //   returns Maybe.Of(sequence of values).
        // See also CollectAny().
        [Pure]
        public static Maybe<IEnumerable<T>> Collect<T>(IEnumerable<Maybe<T>> source)
        {
            return source.Aggregate(
                Maybe.EmptyEnumerable<T>(),
                (x, y) => x.ZipWith(y, Enumerable.Append));
        }

        #region Sum()

        [Pure]
        public static Maybe<T> Sum<T>(IEnumerable<Maybe<T>> source, Func<T, T, T> add, T zero)
        {
            Require.NotNull(add, nameof(add));

            Maybe<IEnumerable<T>> aggr = source.Aggregate(
                Maybe.EmptyEnumerable<T>(),
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
                Maybe.EmptyEnumerable<int>(),
                (x, y) => x.ZipWith(y, Enumerable.Append));

            return aggr.Select(Enumerable.Sum);
        }

        #endregion
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
