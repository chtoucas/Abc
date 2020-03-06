// See LICENSE.txt in the project root for license information.

namespace Abc.Extensions
{
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// Provides extension methods for <see cref="XElement"/>.
    /// </summary>
    public static class XElementX
    {
        public static Maybe<XAttribute> AttributeOrNone(this XElement @this, XName name)
        {
            Require.NotNull(@this, nameof(@this));

            return Maybe.Of(@this.Attribute(name));
        }

        public static Maybe<XElement> ElementOrNone(this XElement @this, XName name)
        {
            Require.NotNull(@this, nameof(@this));

            return Maybe.Of(@this.Element(name));
        }

        public static Maybe<XElement> NextElementOrNone(this XElement @this)
        {
            Require.NotNull(@this, nameof(@this));

            XNode nextElement = @this.NextNode;
            while (nextElement != null && nextElement.NodeType != XmlNodeType.Element)
            {
                nextElement = nextElement.NextNode;
            }

            // NULL_FORGIVING
            return Maybe.Of((nextElement as XElement)!);
        }
    }
}
