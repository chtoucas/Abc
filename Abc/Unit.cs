// See LICENSE.txt in the project root for license information.

#pragma warning disable CA1801 // -Review unused parameters
#pragma warning disable IDE0060 // Remove unused parameter

// REVIEW: ValueTuple

namespace Abc
{
    using System;

    public struct Unit : IEquatable<Unit>
    {
        public static Unit Default { get; } = default;

        /// <summary>
        /// Always returns true.
        /// </summary>
        public static bool operator ==(Unit left, Unit right) => true;

        /// <summary>
        /// Always returns false.
        /// </summary>
        public static bool operator !=(Unit left, Unit right) => false;

        /// <summary>
        /// Returns a string representation of the current instance.
        /// </summary>
        public override string ToString() => "()";

        /// <summary>
        /// Always returns true.
        /// </summary>
        public bool Equals(Unit other) => true;

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Unit;

        /// <inheritdoc />
        public override int GetHashCode() => 0;
    }
}
