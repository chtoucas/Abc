// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Xml.Linq;

    using Anexn = System.ArgumentNullException;

    public static partial class MaybeEx { }

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
        public static Maybe<T> MapValue<T>(
            this Maybe<XElement> @this, Func<string, T> selector)
            => from x in @this select selector(x.Value);

        public static Maybe<string> ValueOrNone(this Maybe<XElement> @this)
            => from x in @this select x.Value;
    }

    // Extensions methods for Maybe<T> where T is an XAttribute.
    public partial class MaybeEx
    {
        public static Maybe<T> MapValue<T>(
            this Maybe<XAttribute> @this, Func<string, T> selector)
            => from x in @this select selector(x.Value);

        public static Maybe<string> ValueOrNone(this Maybe<XAttribute> @this)
            => from x in @this select x.Value;
    }
}
