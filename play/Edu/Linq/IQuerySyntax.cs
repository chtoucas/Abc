﻿// See LICENSE.txt in the project root for license information.

namespace Abc.Edu.Linq
{
    public interface IQuerySyntax
    {
        IQuerySyntax<T> Cast<T>();
    }
}
