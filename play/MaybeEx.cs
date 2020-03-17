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

    // REVIEW: lazy extensions. Is there anything useful we can do w/
    // Lazy<Maybe<T>> or Maybe<Lazy<T>>?

    // NB: the code should be optimized if promoted to the main project.

    // Experimental helpers & extension methods for Maybe<T>.
    //
    // Only a handful of them are really considered for inclusion in the main
    // project as most of them are pretty straightforward.
    // Currently, the best candidates for promotion are:
    // - configurable async methods
    // - ReplaceWith(), a specialization of Select()
    // - Filter(), a specialization of Where()
    //
    // NB: if we change our mind and decide to hide (or rather remove) the
    // property IsNone, we SHOULD add the methods using it to Maybe<T>.
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

        // Reverse of When().
        public static void Unless<T>(
            this Maybe<T> @this, bool condition, Action<T>? onSome, Action? onNone)
        {
            if (!condition)
            {
                if (@this.TryGetValue(out T value))
                {
                    onSome?.Invoke(value);
                }
                else
                {
                    onNone?.Invoke();
                }
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
        // Objectives: constraint TResult : notnull, prevent the creation of
        // Maybe<TResult?>, alternative to AndThen() to avoid the creation
        // of a Maybe when it is not strictly necessary.
        //
        // We should be able to write:
        //   maybe.ReplaceWith(1)
        //   maybe.ReplaceWith((TResult)obj)
        // We should NOT be able to write:
        //   maybe.ReplaceWith((int?)1)         // nullable value type
        //   maybe.ReplaceWith((TResult?)null)  // nullable reference type
        //
        // We want to offer two versions of ReplaceWith(), one for classes and
        // another for structs, to make sure that the method returns a
        // Maybe<TResult> not a Maybe<TResult?>, but it causes some API problems,
        // and since we already have AndThen(), it's better left off for now.
        // Moreover it is just a Select(_ => other); we only bother because we
        // would like to avoid the creation of an unnecessary lambda.
        // Of course, if we don't mind about Maybe<TResult?> the obvious
        // solution is to have only a single method to treat all cases.
        // NB: this looks a lot like the problem we have with Maybe.SomeOrNone()
        // and Maybe.Of().
        //
        // Other point: we should write "where TResult : notnull", since when
        // "other" is null we should rather use AndThen(), nervertheless,
        // whatever I try, I end up double-checking null's, in ReplaceWith()
        // and in the factory method --- that was another reason why we have
        // two versions of ReplaceWith().
        //
        // Simple solution: since IsNone is public, we do not really need to
        // bother w/ ReplaceWith(), same thing with AndThen() in fact.

#if true
        /// <remarks>
        /// <see cref="ReplaceWith"/> is a <see cref="Maybe{T}.Select"/> with a
        /// constant selector <c>_ => value</c>.
        /// <code><![CDATA[
        ///   Some(1) & 2L == Some(2L)
        ///   None    & 2L == None
        /// ]]></code>
        /// </remarks>
        // Compare to the nullable equiv w/ x an int? and y a long:
        //   (x.HasValue ? (long?)y : (long?)null).
        // It does work with null but then one should really use AndThen().
        [Pure]
        public static Maybe<TResult> ReplaceWith<T, TResult>(
            this Maybe<T> @this, TResult value)
            where TResult : notnull
        {
            // Drawback: double-null checks for structs.
            return !@this.IsNone ? Maybe.Of(value) : Maybe<TResult>.None;
        }
#else
        [Pure]
        public static Maybe<TResult> ReplaceWith<T, TResult>(
            this Maybe<T> @this, TResult? value)
            where TResult : class
        {
            return !@this.IsNone ? Maybe.SomeOrNone(value)
                : Maybe<TResult>.None;
        }

        // It works with null but then one should really use
        // AndThen(Maybe<TResult>.None). We can't remove the nullable
        // in the param otherwise we would have two methods with the same
        // name.
        [Pure]
        public static Maybe<TResult> ReplaceWith<T, TResult>(
            this Maybe<T> @this, TResult? value)
            where TResult : struct
        {
            return !@this.IsNone && value.HasValue ? Maybe.Some(value.Value)
                : Maybe<TResult>.None;
        }
#endif

        // Specialized version of Where() when the state of the maybe and the
        // value it encloses are not taken into account.
        [Pure]
        public static Maybe<T> Filter<T>(this Maybe<T> @this, bool condition)
            => condition ? @this : Maybe<T>.None;
    }

    // Gates, bools and bits. The analogy is very contrived.
    public partial class MaybeEx
    {
        // Possible combinations:
        //   Some(1)  Some(2L)  -> None, Some(1), Some(2L)
        //   Some(1)  None      -> None, Some(1)
        //   None     Some(2L)  -> None, Some(2L)
        //   None     None      -> None
        // Below 0 = None, 1 = Some(1) and 2 = Some(2).
        // 0 is "false", 1 and 2 are "true".
        //
        // Compare to the truth table at https://en.wikipedia.org/wiki/Bitwise_operation
        //   0000 -
        //   0020 ContinueWithIfNone()  NOT(<-)
        //   0100 ZeroOutWhen()         NOT(->) aka NIMPLY
        //   0120 XorElse()             XOR
        //
        //   1000 PassThruWhen()        AND
        //   1020 LeftAnd()             right projection
        //   1100 Ignore()              left projection
        //   1120 OrElse()              OR
        //
        //   2000 AndThen()             AND
        //   2020 ContinueWith()        right projection
        //   2100 RightAnd()            left projection
        //   2120 OrElseRTL()           OR
        //
        // Overview: op / return type / pseudo-code
        //   x.OrElse(y)                same types      x is some ? x : y
        //   x.AndThen(y)               type y          x is some ? y : none(y)
        //   x.XorElse(y)               same types      x is some ? (y is some ? none : x) : y
        //   x.ContinueWithIfNone(y)    type y          x is some ? none(y) : y
        //   x.OrElseRTL(y)             same types      y is some ? y : x
        //   x.PassThruWhen(y)          type x          y is some ? x : none(x)
        //   x.ZeroOutWhen(y)           type x          y is some ? none(x) : x
        //   x.Ignore(y)                type x          x
        //   x.ContinueWith(y)          type y          y
        //
        //   LeftAnd(x, y)              same types      x is some && y is some ? x : y
        //   RightAnd(x, y)             same types      x is some && y is some ? y : x
        //
        // Method / flipped method
        //               x.OrElse(y) == y.OrElseRTL(x)
        //              x.XorElse(y) == y.XorElse(x)
        //              x.AndThen(y) == y.PassThruWhen(x)
        //          x.ZeroOutWhen(y) == y.ContinueWithIfNone(x)
        // methods not in main:
        //             LeftAnd(x, y) == RightAnd(y, x)
        //               x.Ignore(y) == y.ContinueWith(x)
        //
        // References:
        // - Knuth vol. 4A, chap. 7.1
        // - https://en.wikipedia.org/wiki/Logical_connective
        // - https://en.wikipedia.org/wiki/Logic_gate
        // - https://en.wikipedia.org/wiki/Bitwise_operation

#pragma warning disable CA1801 // -Review unused parameters
        // Ignore() = flip ContinueWith():
        //   this.Ignore(other) = other.ContinueWith(this)
        /// <code><![CDATA[
        ///   Some(1) Ignore Some(2L) == Some(1)
        ///   Some(1) Ignore None     == Some(1)
        ///   None    Ignore Some(2L) == None
        ///   None    Ignore None     == None
        /// ]]></code>
        [Pure]
        public static Maybe<T> Ignore<T, TOther>(
            this Maybe<T> @this, Maybe<TOther> other)
        {
            return @this;
        }
#pragma warning restore CA1801

        // ContinueWith() = flip Ignore():
        //   this.ContinueWith(other) = other.Ignore(this)
        /// <code><![CDATA[
        ///   Some(1) ContinueWith Some(2L) == Some(2L)
        ///   Some(1) ContinueWith None     == None
        ///   None    ContinueWith Some(2L) == Some(2L)
        ///   None    ContinueWith None     == None
        /// ]]></code>
        [Pure]
        public static Maybe<TResult> ContinueWith<T, TResult>(
            this Maybe<T> @this, Maybe<TResult> other)
        {
            return other;
        }

        // Conjunction; mnemotechnic "P if Q".
        // "@this" pass through when "other" is some.
        // PassThruWhen() = flip AndThen():
        //   this.PassThruWhen(other) = other.AndThen(this)
        /// <summary>
        /// Returns the current instance if <paramref name="other"/> is not
        /// empty; otherwise returns the empty maybe of type
        /// <typeparamref name="TOther"/>.
        /// </summary>
        /// <remarks>
        /// <code><![CDATA[
        ///   Some(1) PassThruWhen Some(2L) == Some(1)
        ///   Some(1) PassThruWhen None     == None
        ///   None    PassThruWhen Some(2L) == None
        ///   None    PassThruWhen None     == None
        /// ]]></code>
        /// </remarks>
        // Compare to the nullable equiv w/ x an int? and y a long?:
        //   (y.HasValue ? x : (int?)null).
        [Pure]
        public static Maybe<T> PassThruWhen<T, TOther>(
            this Maybe<T> @this, Maybe<TOther> other)
        {
            // Using Bind():
            //   other.Bind(_ => @this);
            return other.IsNone ? Maybe<T>.None : @this;
        }

        // Converse nonimplication; mnemotechnic "not P but Q".
        // Like AndThen() but when @this is the empty maybe.
        // ContinueWithIfNone() = flip ZeroOutWhen():
        //   this.ContinueWithIfNone(other) = other.ZeroOutWhen(this)
        // Whereas AndThen() maps
        //   some(X) to some(Y), and none(X) to none(Y)
        // ContinueWithIfNone() maps
        //   some(X) to none(Y), and none(X) to some(Y)
        /// <code><![CDATA[
        ///   Some(1) ContinueWithIfNone Some(2L) == None
        ///   Some(1) ContinueWithIfNone None     == None
        ///   None    ContinueWithIfNone Some(2L) == Some(2L)
        ///   None    ContinueWithIfNone None     == None
        /// ]]></code>
        [Pure]
        public static Maybe<TResult> ContinueWithIfNone<T, TResult>(
           this Maybe<T> @this, Maybe<TResult> other)
        {
            return @this.IsNone ? other : Maybe<TResult>.None;
        }

        // Conjunction. RTL = right-to-left.
        // OrElseRTL() = flip OrElse():
        //   this.OrElseRTL(other) == other.OrElse(this).
        /// <code><![CDATA[
        ///   Some(1) OrElseRTL Some(2) == Some(2)
        ///   Some(1) OrElseRTL None    == Some(1)
        ///   None    OrElseRTL Some(2) == Some(2)
        ///   None    OrElseRTL None    == None
        /// ]]></code>
        [Pure]
        public static Maybe<T> OrElseRTL<T>(
            this Maybe<T> @this, Maybe<T> other)
        {
            return @this.IsNone ? other
                : other.IsNone ? @this
                : other;
        }

        // If left or right is empty, returns right; otherwise returns left.
        // LeftAnd() = flip RightAnd():
        //   LeftAnd(left, right) == RightAnd(right, left).
        /// <code><![CDATA[
        ///   Some(1) LeftAnd Some(2) == Some(1)
        ///   Some(1) LeftAnd None    == None
        ///   None    LeftAnd Some(2) == Some(2)
        ///   None    LeftAnd None    == None
        /// ]]></code>
        [Pure]
        public static Maybe<T> LeftAnd<T>(Maybe<T> left, Maybe<T> right)
        {
            return !left.IsNone && !right.IsNone ? left : right;
        }

        // If left or right is empty, returns left; otherwise returns right.
        // RightAnd() = flip LeftAnd():
        //   RightAnd(left, right) == LeftAnd(right, left).
        /// <code><![CDATA[
        ///   Some(1) RightAnd Some(2) == Some(2)
        ///   Some(1) RightAnd None    == Some(1)
        ///   None    RightAnd Some(2) == None
        ///   None    RightAnd None    == None
        /// ]]></code>
        [Pure]
        public static Maybe<T> RightAnd<T>(Maybe<T> left, Maybe<T> right)
        {
            return !left.IsNone && !right.IsNone ? right : left;
        }
    }

    // Extension methods for Maybe<T> where T is enumerable.
    public partial class MaybeEx
    {
        [Pure]
        public static IEnumerable<T> ValueOrEmpty<T>(
            this Maybe<IEnumerable<T>> @this)
        {
            return @this.ValueOrElse(Enumerable.Empty<T>());
        }

        [Pure]
        public static Maybe<IEnumerable<T>> EmptyIfNone<T>(
            this Maybe<IEnumerable<T>> @this)
        {
            return @this.OrElse(Maybe.EmptyEnumerable<T>());
        }
    }

    // LINQ extensions for IEnumerable<Maybe<T>>.
    public partial class MaybeEx
    {
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

        // Aggregation: monadic sum.
        // For Maybe<T>, it amounts to returning the first non-empty item, or
        // an empty maybe if they are all empty.
        [Pure]
        public static Maybe<T> FirstOrNone<T>(IEnumerable<Maybe<T>> source)
        {
            return source.FirstOrDefault(x => !x.IsNone);
        }

        #region Aggregation Sum()

        [Pure]
        public static Maybe<T> Sum<T>(
            IEnumerable<Maybe<T>> source, Func<T, T, T> add, T zero)
        {
            Require.NotNull(add, nameof(add));

            Maybe<IEnumerable<T>> aggr = Collect(source);

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
            Maybe<IEnumerable<int>> aggr = Collect(source);

            return aggr.Select(Enumerable.Sum);
        }

        #endregion
    }

    // Extensions methods for Maybe<T> where T is an XObject.
    public partial class MaybeEx
    {
        [Pure]
        public static Maybe<T> MapValue<T>(
            this Maybe<XElement> @this, Func<string, T> selector)
            => from x in @this select selector(x.Value);

        [Pure]
        public static Maybe<string> ValueOrNone(this Maybe<XElement> @this)
            => from x in @this select x.Value;

        [Pure]
        public static Maybe<T> MapValue<T>(
            this Maybe<XAttribute> @this, Func<string, T> selector)
            => from x in @this select selector(x.Value);

        [Pure]
        public static Maybe<string> ValueOrNone(this Maybe<XAttribute> @this)
            => from x in @this select x.Value;
    }
}
