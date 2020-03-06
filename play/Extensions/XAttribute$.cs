// See LICENSE.txt in the project root for license information.

namespace Abc.Extensions
{
    using System.Xml.Linq;

    /// <summary>
    /// Provides extension methods for <see cref="XAttribute"/>.
    /// </summary>
    public static class XAttributeX
    {
        public static Maybe<XAttribute> NextAttributeOrNone(this XAttribute @this)
        {
            Require.NotNull(@this, nameof(@this));

            return Maybe.Of(@this.NextAttribute);
        }

        public static Maybe<XAttribute> PreviousAttributeOrNone(this XAttribute @this)
        {
            Require.NotNull(@this, nameof(@this));

            return Maybe.Of(@this.PreviousAttribute);
        }
    }
}
