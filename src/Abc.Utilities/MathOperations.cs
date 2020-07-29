// See LICENSE in the project root for license information.

#nullable enable

namespace Abc.Utilities
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Provides static methods for common mathematical operations.
    /// <para>This class cannot be inherited.</para>
    /// </summary>
    [DebuggerNonUserCode]
    internal abstract partial class MathOperations
    {
        [ExcludeFromCodeCoverage]
        protected MathOperations() { }
    }

    // Euclidian division.
    //
    // When the numerator is a negative integer, the C# division operator
    // is a division with negative remainder. These methods implement the
    // standard Euclidian division by ensuring that the remainder is always an
    // integer in the range from 0 to the denominator (excluded). Similarly,
    // Math.DivRem does not follow the mathematical rules when the dividend is
    // negative.
    //
    // For n a power of 2, n = 2^k = (1 << k), one can simply write:
    //   q = m / n = m >> k
    //   r = m [n] = m & (n - 1)
    // and it works whether m is positive or negative. For instance,
    //   q = m / 4  = m >> 2
    //   r = m [4] = m & 3
    // and
    //   q = m / 8  = m >> 3
    //   r = m [8] = m & 7
    internal partial class MathOperations
    {
        // Division euclidienne dans \N
        // ----------------------------
        // Si m, n dans \N, n != 0, alors il existe q, r dans \N (uniques) tels
        // que :
        //   m = q n + r
        //   0 <= r < n
        // En .NET, pour obtenir q et r, il suffit d'écrire :
        //   [C#] q = m / n et r = m % n
        //
        // Division d'un entier relatif par un entier naturel non nul
        // ----------------------------------------------------------
        // Si m dans \Z et n dans \N, n!= 0, alors il existe q dans \Z et r dans
        // \N (uniques) tels que :
        //   m = q n + r
        //   0 <= r < n
        // Malheureusement, si m < 0, .NET utilise la version avec reste
        // négatif : [C#] q' = m / n et r' = m % n correspond à
        //   m = q' n + r'
        //   -n < r' <= 0
        // néanmoins récupérer r et n n'est pas bien difficile.
        // Si r' = 0, il suffit de prendre r = 0 et q = q', sinon
        //   q = q' - 1
        //   r = r' + n
        // pour le voir écrire m = (q' - 1) n + (r' + n), puis tirer avantage
        // de l'unicité de la décomposition.
        // Pour m < 0 et m % n != 0, le couple recherché est donc donné par :
        //   q = (m / n) - 1
        //   r = (m % n) + n
        // si m < 0 et m % n = 0, alors q = m / n et r = 0.
        //
        // On ne s'intéresse pas à la division euclidienne dans \Z ; on y perd
        // d'ailleurs l'unicité de la solution (au plus deux).
        //
        // Un autre manière (moins efficace) consiste à utiliser un Double ou un
        // Decimal. Par exemple, q = Math.Floor((double)m/n).
        //
        // Cas particulier n = 4. Pour m positif ou négatif, on peut prendre
        //   q = m >> 2
        //   r = m & 3
        // On a un résultat similaire pour n'importe quelle puissance de 2, mais
        // il n'est pas sûr que ce soit vraiment mieux quand n est grand (à
        // tester) : division par n = 2^k
        //   q = m >> k
        //   r = m & (n - 1)
        //
        // Bien entendu, quand on sait par avance que m >= 0, il n'est pas
        // nécessaire d'utiliser les opérateurs ci-dessous.

        /// <summary>
        /// Calculates the quotient of a 32-bit signed integer by a non-zero
        /// 32-bit unsigned integer.
        /// <para>This method does NOT validate its parameters.</para>
        /// </summary>
#if !ABC_UTILITIES_ENABLE_CODE_COVERAGE
        [ExcludeFromCodeCoverage]
#endif
        [Pure]
        // Code size = 19 bytes.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Divide(int m, int n)
        {
            Debug.Assert(n > 0);

            return m >= 0 || m % n == 0 ? m / n : (m / n - 1);
        }

        /// <summary>
        /// Calculates the quotient of a 64-bit signed integer by a non-zero
        /// 64-bit unsigned integer.
        /// <para>This method does NOT validate its parameters.</para>
        /// </summary>
#if !ABC_UTILITIES_ENABLE_CODE_COVERAGE
        [ExcludeFromCodeCoverage]
#endif
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Divide(long m, long n)
        {
            Debug.Assert(n > 0);

            return m >= 0L || m % n == 0L ? m / n : (m / n - 1L);
        }

        /// <summary>
        /// Calculates the quotient of a 32-bit signed integer by a non-zero
        /// 32-bit unsigned integer and also returns the remainder in an output
        /// parameter.
        /// <para>This method does NOT validate its parameters.</para>
        /// </summary>
        /// <remarks>
        /// The remainder <paramref name="r"/> is in the range from 0 to
        /// (<paramref name="n"/> - 1), both included.
        /// </remarks>
#if !ABC_UTILITIES_ENABLE_CODE_COVERAGE
        [ExcludeFromCodeCoverage]
#endif
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Divide(int m, int n, out int r)
        {
            Debug.Assert(n > 0);

            // On écrit la routine de telle sorte que la taille du code compilé
            // reste inférieure à 32 bytes, valeur limite pour que la méthode
            // soit déclarée éligible à l'"inlining" par le JIT.
            // Voir PerfTool.Misc.DividePerf.
            r = m % n;
            if (m >= 0 || r == 0)
            {
                return m / n;
            }
            else
            {
                r += n;
                return m / n - 1;
            }
        }

        /// <summary>
        /// Calculates the quotient of a 64-bit signed integer by a non-zero
        /// 64-bit unsigned integer and also returns the remainder in an output
        /// parameter.
        /// <para>This method does NOT validate its parameters.</para>
        /// </summary>
        /// <remarks>
        /// The remainder <paramref name="r"/> is in the range from 0 to
        /// (<paramref name="n"/> - 1), both included.
        /// </remarks>
#if !ABC_UTILITIES_ENABLE_CODE_COVERAGE
        [ExcludeFromCodeCoverage]
#endif
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Divide(long m, long n, out long r)
        {
            Debug.Assert(n > 0);

            r = m % n;
            if (m >= 0 || r == 0)
            {
                return m / n;
            }
            else
            {
                r += n;
                return m / n - 1;
            }
        }

        /// <summary>
        /// Calculates the remainder of the division of a 32-bit signed integer
        /// by a non-zero 32-bit unsigned integer.
        /// <para>This method does NOT validate its parameters.</para>
        /// </summary>
        /// <remarks>
        /// The remainder is in the range from 0 to (<paramref name="n"/> - 1),
        /// both included.
        /// </remarks>
#if !ABC_UTILITIES_ENABLE_CODE_COVERAGE
        [ExcludeFromCodeCoverage]
#endif
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Modulo(int m, int n)
        {
            Debug.Assert(n > 0);

            int r0 = m % n;
            return r0 >= 0 ? r0 : (r0 + n);
        }

        /// <summary>
        /// Calculates the remainder of the division of a 64-bit signed integer
        /// by a non-zero 64-bit unsigned integer.
        /// <para>This method does NOT validate its parameters.</para>
        /// </summary>
        /// <remarks>
        /// The remainder is in the range from 0 to (<paramref name="n"/> - 1),
        /// both included.
        /// </remarks>
#if !ABC_UTILITIES_ENABLE_CODE_COVERAGE
        [ExcludeFromCodeCoverage]
#endif
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Modulo(long m, long n)
        {
            Debug.Assert(n > 0);

            long r0 = m % n;
            return r0 >= 0 ? r0 : (r0 + n);
        }

        /// <summary>
        /// Calculates the adjusted remainder of the division of a 32-bit signed
        /// integer by a non-zero 32-bit unsigned integer.
        /// <para>This method does NOT validate its parameters.</para>
        /// </summary>
        /// <remarks>
        /// The adjusted remainder is in the range from 1 to
        /// <paramref name="n"/>, both included.
        /// </remarks>
        // Opération modulo rectifiée ("adjusted").
#if !ABC_UTILITIES_ENABLE_CODE_COVERAGE
        [ExcludeFromCodeCoverage]
#endif
        [Pure]
        // Code size = 22 bytes.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AdjustedModulo(int m, int n)
        {
            Debug.Assert(n > 0);

            int r0 = m % n;
            int mod = r0 >= 0 ? r0 : (r0 + n);
            return mod == 0 ? n : mod;
        }

        /// <summary>
        /// Calculates the adjusted remainder of the division of a 64-bit signed
        /// integer by a non-zero 64-bit unsigned integer.
        /// <para>This method does NOT validate its parameters.</para>
        /// </summary>
        /// <remarks>
        /// The adjusted remainder is in the range from 1 to
        /// <paramref name="n"/>, both included.
        /// </remarks>
#if !ABC_UTILITIES_ENABLE_CODE_COVERAGE
        [ExcludeFromCodeCoverage]
#endif
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long AdjustedModulo(long m, long n)
        {
            Debug.Assert(n > 0);

            long r0 = m % n;
            long mod = r0 >= 0 ? r0 : (r0 + n);
            return mod == 0 ? n : mod;
        }
    }
}
