// See LICENSE.txt in the project root for license information.

namespace Play.Edu.Linq
{
    public interface IGroupingQuerySyntax<TKey, T> : IQuerySyntax<T>
    {
        TKey Key { get; }
    }
}
