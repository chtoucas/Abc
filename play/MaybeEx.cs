// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Xml.Linq;

    public static partial class MaybeEx { }

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
