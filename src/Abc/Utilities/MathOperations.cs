// See LICENSE in the project root for license information.

namespace Abc.Utilities
{
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Runtime.CompilerServices;

    internal partial class MathOperations
    {
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
    }
}
