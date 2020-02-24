﻿// See LICENSE.txt in the project root for license information.

namespace Abc.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;

#if MONADS_PURE
    using Abc.Linq;
#endif

    // REVIEW: MayGetValues, Maybe<IEnumerable>?

    /// <summary>
    /// Provides extension methods for <see cref="NameValueCollection"/>
    /// and <see cref="IDictionary{T,U}"/>.
    /// </summary>
    public static partial class CollectionExtensions { }

    public partial class CollectionExtensions
    {
        public static Maybe<TValue> MayGetValue<TKey, TValue>(
            this IDictionary<TKey, TValue> @this, TKey key)
        {
            if (@this is null) { throw new ArgumentNullException(nameof(@this)); }

            return !(key is null) && @this.TryGetValue(key, out TValue value)
                ? Maybe.Of(value)
                : Maybe<TValue>.None;
        }
    }

    public partial class CollectionExtensions
    {
        public static Maybe<string> MayGetSingle(this NameValueCollection @this, string name)
        {
            return from values in @this.MayGetValues(name)
                   where values.Length == 1
                   select values[0];
        }

        public static Maybe<string[]> MayGetValues(
            this NameValueCollection @this, string name)
        {
            if (@this is null) { throw new ArgumentNullException(nameof(@this)); }

            return Maybe.Of(@this.GetValues(name));
        }

        public static IEnumerable<T> ParseValues<T>(
            this NameValueCollection @this, string name, Func<string, Maybe<T>> parser)
        {

#if MONADS_PURE
            var q = from values in @this.MayGetValues(name) select values.SelectAny(parser);
            return q.ValueOrEmpty();
#else
            // Check args eagerly.
            if (@this is null) { throw new ArgumentNullException(nameof(@this)); }

            return __iterator();

            IEnumerable<T> __iterator()
            {
                foreach (string item in @this.GetValues(name))
                {
                    var result = parser(item);

                    if (result.IsSome) { yield return result.Value; }
                }
            }
#endif
        }
    }
}