// See LICENSE.txt in the project root for license information.

namespace Abc.Extensions
{
    using System;
    using System.Xml.Linq;

    /// <summary>
    /// Provides extension methods for <see cref="Maybe{XElement}"/> and
    /// <see cref="Maybe{XAttribute}"/>.
    /// </summary>
    public static partial class MaybeX { }

    // Extensions methods for Maybe<XElement>.
    public partial class MaybeX
    {
        public static Maybe<T> MapValue<T>(
            this Maybe<XElement> @this, Func<string, T> selector)
            => from elmt in @this select selector(elmt.Value);

        public static Maybe<string> ValueOrNone(this Maybe<XElement> @this)
            => from elmt in @this select elmt.Value;
    }

    // Extensions methods for Maybe<XAttribute>.
    public partial class MaybeX
    {
        public static Maybe<T> MapValue<T>(
            this Maybe<XAttribute> @this, Func<string, T> selector)
            => from attr in @this select selector(attr.Value);

        public static Maybe<string> ValueOrNone(this Maybe<XAttribute> @this)
            => from attr in @this select attr.Value;
    }
}
