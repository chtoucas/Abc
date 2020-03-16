﻿// See LICENSE.txt in the project root for license information.

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
    // - the configurable async methods
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
        // Maybe<TResult?>, alternative to ContinueWith() to avoid the creation
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
        // and since we already have ContinueWith(), it's better left off for now.
        // Moreover it is just a Select(_ => other); we only bother because we
        // would like to avoid the creation of an unnecessary lambda.
        // Of course, if we don't mind about Maybe<TResult?> the obvious
        // solution is to have only a single method to treat all cases.
        // NB: this looks a lot like the problem we have with Maybe.SomeOrNone()
        // and Maybe.Of().
        //
        // Other point: we should write "where TResult : notnull", since when
        // "other" is null we should rather use ContinueWith(), nervertheless,
        // whatever I try, I end up double-checking null's, in ReplaceWith()
        // and in the factory method --- that was another reason why we have
        // two versions of ReplaceWith().
        //
        // Simple solution: since IsNone is public, we do not really need to
        // bother w/ ReplaceWith(), same thing with ContinueWith() in fact.

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
        // It does work with null but then one should really use ContinueWith().
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
        // ContinueWith(Maybe<TResult>.None). We can't remove the nullable
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
    // References:
    // - Knuth vol. 4A
    // - https://en.wikipedia.org/wiki/Logical_connective
    // - https://en.wikipedia.org/wiki/Logic_gate
    // - https://en.wikipedia.org/wiki/Bitwise_operation
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
        //   0100 PassThruWhenNone()    NOT(->) aka NIMPLY
        //   0120 XorElse()             XOR
        //
        //   1000 PassThru()            AND
        //   1020 "RightProject"        "right projection"
        //   1100 Ignore()              left projection       <-- useless, ignore right
        //   1120 OrElse()              OR
        //
        //   2000 ContinueWith()        AND
        //   2020 Always()              right projection      <-- useless, ignore left
        //   2100 "LeftProject"         "left projection"
        //   2120 OrElseRTL()           "OR"
        //
        // Overview: return type / pseudo-code
        //   x.OrElse(y)                type x = y      x is some ? x : y
        //   x.OrElseRTL(y)             type x = y      y is some ? y : x
        //   x.XorElse(y)               type x = y      x is some ? (y is some ? none : x) : y
        //   x.ContinueWith(y)          type y          x is some ? y : none
        //   x.ContinueWithIfNone(y)    type y          x is none ? y : none
        //   x.PassThru(y)              type x          y is some ? x : none
        //   x.PassThruIfNone(y)        type x          y is none ? x : none
        //
        //   x.Ignore(y)                type x          x
        //   x.Always(y)                type y          y
        //   x.RightProject(y)          type x = y      x is some ? (y is some ? x : y) : y
        //   x.LeftProject(y)           type x = y      x is some ? (y is some ? y : x) : x
        //
        // Naming convention:
        // - ...IfNone() or ...IfSome(), the current instance is empty or not.
        // - ...WhenNone() or ...WhenSome(), the other maybe is empty or not.
        // that is:
        // - ContinueWith() is really ContinueWithIfSome()
        // - PassThru() is really PassThruWhenSome()

#pragma warning disable CA1801 // -Review unused parameters
        // Ignore() = flip Always():
        //   this.Ignore(other) = other.Always(this)
        /// <code><![CDATA[
        ///   Some(1) Ignore Some(2L) == Some(1)
        ///   Some(1) Ignore None     == Some(1)
        ///   None    Ignore Some(2L) == None
        ///   None    Ignore None     == None
        /// ]]></code>
        [Pure]
        public static Maybe<T> Ignore<T, TOther>(
            this Maybe<T> @this, Maybe<TOther> other)
            => @this;
#pragma warning restore CA1801

        // Always() = flip Ignore():
        //   this.Always(other) = other.Ignore(this)
        /// <code><![CDATA[
        ///   Some(1) Always Some(2L) == Some(2L)
        ///   Some(1) Always None     == None
        ///   None    Always Some(2L) == Some(2L)
        ///   None    Always None     == None
        /// ]]></code>
        [Pure]
        public static Maybe<TResult> Always<T, TResult>(
            this Maybe<T> @this, Maybe<TResult> other)
            => other;

        // Converse nonimplication; mnemotechnic "not P but Q".
        // Like ContinueWith() but when @this is the empty maybe.
        // ContinueWithIfNone() = flip PassThruWhenNone():
        //   this.ContinueWithIfNone(other) = other.PassThruWhenNone(this)
        /// <code><![CDATA[
        ///   Some(1) ContinueWithIfNone Some(2L) == None
        ///   Some(1) ContinueWithIfNone None     == None
        ///   None    ContinueWithIfNone Some(2L) == Some(2L)
        ///   None    ContinueWithIfNone None     == None
        /// ]]></code>
        [Pure]
        public static Maybe<TResult> ContinueWithIfNone<T, TResult>(
            this Maybe<T> @this, Maybe<TResult> other)
            => @this.IsNone ? other : Maybe<TResult>.None;

        // Nonimplication; mnemotechnic "P but not Q".
        // Like PassThru() but when "other" is the empty maybe.
        // PassThruWhenNone() = flip ContinueWithIfNone():
        //   this.PassThruWhenNone(other) = other.ContinueWithIfNone(this)
        /// <code><![CDATA[
        ///   Some(1) PassThruWhenNone Some(2L) == None
        ///   Some(1) PassThruWhenNone None     == Some(1)
        ///   None    PassThruWhenNone Some(2L) == None
        ///   None    PassThruWhenNone None     == None
        /// ]]></code>
        [Pure]
        public static Maybe<T> PassThruWhenNone<T, TOther>(
            this Maybe<T> @this, Maybe<TOther> other)
            => other.IsNone ? @this : Maybe<T>.None;

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
            => @this.IsNone ? other
                : other.IsNone ? @this : other;

        /// <code><![CDATA[
        ///   Some(1) RightProject Some(2) == Some(1)
        ///   Some(1) RightProject None    == None
        ///   None    RightProject Some(2) == Some(2)
        ///   None    RightProject None    == None
        /// ]]></code>
        [Pure]
        public static Maybe<T> RightProject<T>(
            this Maybe<T> @this, Maybe<T> other)
            => @this.IsNone ? other : (other.IsNone ? other : @this);

        /// <code><![CDATA[
        ///   Some(1) LeftProject Some(2) == Some(2)
        ///   Some(1) LeftProject None    == Some(1)
        ///   None    LeftProject Some(2) == None
        ///   None    LeftProject None    == None
        /// ]]></code>
        [Pure]
        public static Maybe<T> LeftProject<T>(
            this Maybe<T> @this, Maybe<T> other)
            => @this.IsNone ? @this : (other.IsNone ? @this : other);
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
