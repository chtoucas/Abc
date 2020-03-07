// See LICENSE.txt in the project root for license information.

namespace Abc.Extensions
{
    using System.Xml;
    using System.Xml.Linq;

    using static MaybeFactory;

    /// <summary>
    /// Provides extension methods for <see cref="XAttribute"/> and
    /// <see cref="XElement"/>.
    /// <para>Warning, these extension methods do NOT throw when the object is
    /// null but rather return an empty maybe.</para>
    /// </summary>
    public static partial class XObjectX { }

    // Extension methods for XAttribute.
    public partial class XObjectX
    {
        public static Maybe<XAttribute> NextAttributeOrNone(this XAttribute? @this)
            => SomeOrNone(@this?.NextAttribute);

        public static Maybe<XAttribute> PreviousAttributeOrNone(this XAttribute? @this)
            => SomeOrNone(@this?.PreviousAttribute);
    }

    // Extension methods for XElement.
    public partial class XObjectX
    {
        public static Maybe<XAttribute> AttributeOrNone(this XElement? @this, XName name)
            => SomeOrNone(@this?.Attribute(name));

        public static Maybe<XElement> ElementOrNone(this XElement? @this, XName name)
            => SomeOrNone(@this?.Element(name));

        public static Maybe<XElement> NextElementOrNone(this XElement? @this)
        {
            XNode? nextElement = @this?.NextNode;
            while (nextElement != null && nextElement.NodeType != XmlNodeType.Element)
            {
                nextElement = nextElement.NextNode;
            }

            return SomeOrNone((nextElement as XElement));
        }
    }
}
