// See LICENSE in the project root for license information.

namespace Abc
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    using EF = Abc.Utilities.ExceptionFactory;

    // REVIEW: should we make it work with a comparer w/ Maybe<T>.
    //   return comparer.Compare(this, maybe);
    // Should we ensure that MaybeComparer<T> works with T? IEqualityComparer<T>?
    // DO NOT FORGET TO UPDATE MaybeTests.StructuralComparable().

    // Pluggable comparison.
    // A total order for maybe's. Identical to what Maybe<T>.Compare() does,
    // but made available separately.
    // The convention is that the empty maybe is strictly less than any other
    // maybe.
    public sealed class MaybeComparer<T>
        : IEqualityComparer<Maybe<T>>, IEqualityComparer,
            IComparer<Maybe<T>>, IComparer
    {
        public static readonly MaybeComparer<T> Default = new MaybeComparer<T>();

        private MaybeComparer() { }

        //
        // IEqualityComparer<>
        //

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Maybe<T> x, Maybe<T> y) =>
            // BONSANG! When IsSome is true, Value is NOT null.
            x.IsSome
                ? y.IsSome && EqualityComparer<T>.Default.Equals(x.Value!, y.Value!)
                : !y.IsSome;

        bool IEqualityComparer.Equals(object? x, object? y)
        {
            if (x == y) { return true; }
            if (x is null || y is null) { return false; }
            if (x is Maybe<T> left && y is Maybe<T> right) { return Equals(left, right); }
            throw EF.MaybeComparer_InvalidType;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHashCode(Maybe<T> obj) =>
            // BONSANG! When IsSome is true, Value is NOT null.
            obj.IsSome ? obj.Value!.GetHashCode() : 0;

        int IEqualityComparer.GetHashCode(object? obj)
        {
            if (obj is null) { return 0; }
            if (obj is Maybe<T> maybe) { return GetHashCode(maybe); }
            throw EF.MaybeComparer_InvalidType;
        }

        //
        // IComparer<>
        //

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(Maybe<T> x, Maybe<T> y) =>
            // BONSANG! When IsSome is true, Value is NOT null.
            x.IsSome
                ? y.IsSome ? Comparer<T>.Default.Compare(x.Value!, y.Value!) : 1
                : y.IsSome ? -1 : 0;

        int IComparer.Compare(object? x, object? y)
        {
            if (x is null) { return y is null ? 0 : -1; }
            if (y is null) { return 1; }
            if (x is Maybe<T> left && y is Maybe<T> right) { return Compare(left, right); }
            throw EF.MaybeComparer_InvalidType;
        }

        //
        // Equals() & GetHashCode() for the comparer itself.
        //

        public override bool Equals(object? obj) =>
            obj != null && GetType() == obj.GetType();

        public override int GetHashCode() =>
            GetType().GetHashCode();
    }
}
