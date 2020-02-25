// See LICENSE.txt in the project root for license information.

namespace Play.Functional.Linq
{
    public interface IQuerySyntax
    {
        IQuerySyntax<T> Cast<T>();
    }
}
