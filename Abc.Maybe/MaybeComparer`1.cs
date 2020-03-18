// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System.Collections;
    using System.Collections.Generic;

    using EF = Abc.Utilities.ExceptionFactory;

    // A total order for maybe's.
    public sealed class MaybeComparer<T> : IComparer<Maybe<T>>, IComparer
    {
        public static readonly MaybeComparer<T> Default = new MaybeComparer<T>();

        private MaybeComparer() { }

        public int Compare(Maybe<T> x, Maybe<T> y)
            => x.IsSome
                ? y.IsSome ? Comparer<T>.Default.Compare(x.Value, y.Value) : 1
                : y.IsSome ? -1 : 0;

        public int Compare(object? x, object? y)
            => x is null ? y is null ? 0 : -1
                : y is null ? 1
                : x is Maybe<T> left && y is Maybe<T> right
                    ? Compare(left, right)
                    : throw EF.NotComparable;
    }
}
