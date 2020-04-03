﻿// See LICENSE.dotnet in the project root for license information.
//
// https://github.com/dotnet/runtime/blob/master/src/libraries/System.Linq/tests/EnumerableTests.cs

using System.Collections.Generic;
using System.Linq;

internal class AnagramEqualityComparer : IEqualityComparer<string>
{
    public bool Equals(string x, string y)
    {
        if (ReferenceEquals(x, y)) { return true; }
        if (x is null || y is null) { return false; }
        int length = x.Length;
        if (length != y.Length) { return false; }
        using (var en = x.OrderBy(i => i).GetEnumerator())
        {
            foreach (char c in y.OrderBy(i => i))
            {
                en.MoveNext();
                if (c != en.Current) { return false; }
            }
        }
        return true;
    }

    public int GetHashCode(string obj)
    {
        if (obj == null) { return 0; }
        int hash = obj.Length;
        foreach (char c in obj)
        {
            hash ^= c;
        }
        return hash;
    }
}
