// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System.Collections;
    using System.Collections.Generic;

    using EF = Abc.Utilities.ExceptionFactory;

    // A total order for maybe's. Identical to what Maybe<T>.Compare() does,
    // but made available separately.
    // The convention is that the empty maybe is strictly less than any other
    // maybe.
    public sealed class MaybeComparer<T> : IComparer<Maybe<T>>, IComparer
    {
        public static readonly MaybeComparer<T> Default = new MaybeComparer<T>();

        private MaybeComparer() { }

        public int Compare(Maybe<T> left, Maybe<T> right)
            // BONSANG! When IsSome is true, Value is NOT null.
            => left.IsSome
                ? right.IsSome ? Comparer<T>.Default.Compare(left.Value!, right.Value!) : 1
                : right.IsSome ? -1 : 0;

        public int Compare(object? left, object? right)
            => left is null ? right is null ? 0 : -1
                : right is null ? 1
                : left is Maybe<T> first && right is Maybe<T> second
                    ? Compare(first, second)
                    : throw EF.MaybeComparer_InvalidType;
    }
}
