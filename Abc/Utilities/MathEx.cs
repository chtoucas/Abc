// See LICENSE.txt in the project root for license information.

namespace Abc.Utilities
{
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Provides static methods for common mathematical operations.
    /// <para>This class cannot be inherited.</para>
    /// </summary>
    internal static partial class MathEx { }

    // Euclidian division.
    //
    // When the numerator is a negative integer, the C# division operator
    // is a division with negative remainder. These methods implement the
    // standard Euclidian division by ensuring that the remainder is always an
    // integer in the range from 0 to the denominator (excluded).
    //
    // For n a power of 2, n = 2^k = (1 << k), one can simply write:
    //   q = i / n = i >> k
    //   r = i [n] = i & (n - 1)
    internal partial class MathEx
    {
        /// <summary>
        /// Calculates the quotient of a 32-bit signed integer by a non-zero
        /// 32-bit unsigned integer.
        /// <para>This method does NOT validate its parameters.</para>
        /// </summary>
        [Pure]
        // Code size = 19 bytes.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Divide(int i, int n)
        {
            Debug.Assert(n > 0);

            return i >= 0 || i % n == 0 ? i / n : (i / n - 1);
        }

        /// <summary>
        /// Calculates the remainder of the division of a 32-bit signed integer
        /// by a non-zero 32-bit unsigned integer and also returns the quotient
        /// in an output parameter.
        /// <para>This method does NOT validate its parameters.</para>
        /// </summary>
        [Pure]
        // Code size = 28 bytes.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Modulo(int i, int n, out int q)
        {
            Debug.Assert(n > 0);

            int r = i % n;
            q = i / n;
            if (i < 0 && r != 0)
            {
                q--;
                r += n;
            }
            return r;
        }

        /// <summary>
        /// Calculates the adjusted remainder of the division of a 32-bit signed
        /// integer by a non-zero 32-bit unsigned integer
        /// </summary>
        /// <remarks>
        /// The standard remainder is in the range from 0 to <paramref name="n"/>
        /// excluded. The adjusted modulus is in the range from 1 to
        /// <paramref name="n"/> included.
        /// </remarks>
        [Pure]
        // Code size = 22 bytes.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AdjustedModulo(int i, int n)
        {
            Debug.Assert(n > 0);

            int r0 = i % n;
            int mod = r0 >= 0 ? r0 : (r0 + n);
            return mod == 0 ? n : mod;
        }
    }
}
