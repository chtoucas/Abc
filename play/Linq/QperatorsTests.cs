﻿// See LICENSE.dotnet.txt in the project root for license information.
//
// Partly inspired by
// https://github.com/dotnet/runtime/blob/master/src/libraries/System.Linq/tests/EnumerableTests.cs

namespace Abc.Linq
{
    using System.Collections.Generic;
    using System.Linq;

    public abstract class QperatorsTests
    {
        protected QperatorsTests() { }

        protected static readonly IEnumerable<int> Null = null!;
        protected static readonly IEnumerable<int> NotNull = Enumerable.Empty<int>();

        protected static bool IsEven(int num) => num % 2 == 0;

        private protected static IEnumerable<T> ForceNotCollection<T>(IEnumerable<T> source)
        {
            foreach (T item in source)
            {
                yield return item;
            }
        }

        private protected static IEnumerable<int> NumberRangeGuaranteedNotCollectionType(int num, int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return num + i;
            }
        }

        private protected static IEnumerable<int> RepeatedNumberGuaranteedNotCollectionType(int num, long count)
        {
            for (long i = 0; i < count; i++)
            {
                yield return num;
            }
        }
    }
}