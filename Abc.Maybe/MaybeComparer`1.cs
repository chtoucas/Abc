// See LICENSE in the project root for license information.

namespace Abc
{
    using System.Collections;
    using System.Collections.Generic;

    using EF = Abc.Utilities.ExceptionFactory;

    // REVIEW: should we make it work with a comparer w/ Maybe<T>.
    //   return comparer.Compare(this, maybe);
    // Should we esnure that MaybeComparer<T> works with T?
    // DO NOT FORGET TO UPDATE MaybeTests.StructuralComparable().
    // Same thing w/ IStructuralEquatable.
    // Implement IEqualityComparer<Maybe<T>>?
    // Implement IEqualityComparer or remove IComparer?

    // A total order for maybe's. Identical to what Maybe<T>.Compare() does,
    // but made available separately.
    // The convention is that the empty maybe is strictly less than any other
    // maybe.
    public sealed class MaybeComparer<T>
        : IEqualityComparer<Maybe<T>>,
            IComparer<Maybe<T>>, IComparer
    {
        public static readonly MaybeComparer<T> Default = new MaybeComparer<T>();

        private MaybeComparer() { }

        public int Compare(object? left, object? right) =>
            left is null ? right is null ? 0 : -1
                : right is null ? 1
                : left is Maybe<T> first && right is Maybe<T> second
                    ? Compare(first, second)
                    : throw EF.MaybeComparer_InvalidType;

        public int Compare(Maybe<T> left, Maybe<T> right) =>
            // BONSANG! When IsSome is true, Value is NOT null.
            left.IsSome
                ? right.IsSome ? Comparer<T>.Default.Compare(left.Value!, right.Value!) : 1
                : right.IsSome ? -1 : 0;

        public bool Equals(Maybe<T> left, Maybe<T> right) =>
            // BONSANG! When IsSome is true, Value is NOT null.
            left.IsSome
                ? right.IsSome && EqualityComparer<T>.Default.Equals(left.Value!, right.Value!)
                : !right.IsSome;

        public int GetHashCode(Maybe<T> obj) =>
            // BONSANG! When IsSome is true, Value is NOT null.
            obj.IsSome ? obj.Value!.GetHashCode() : 0;
    }
}
