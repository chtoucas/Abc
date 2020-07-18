// See LICENSE in the project root for license information.

namespace Abc.Utilities
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Provides static methods for common mathematical operations.
    /// <para>This class cannot be inherited.</para>
    /// </summary>
#if NO_CODE_COVERAGE
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
    internal static partial class MathOperations { }

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

    // Doubles.
    internal partial class MathOperations
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double RoundAwayFromZero(double num)
#if SOFAM
            // See sofam.h (dnint): round to nearest whole number (double)
            => num < 0 ? Math.Ceiling(num - .5) : Math.Floor(num + .5);
#else
            => Math.Round(num, MidpointRounding.AwayFromZero);
#endif

        //// "z is the result of x % y and is computed as x - n * y, where n is the
        //// largest possible integer that is less than or equal to x / y. This
        //// method of computing the remainder is analogous to that used for
        //// integer operands, but differs from the IEEE 754 definition (in which
        //// n is the integer closest to x / y)."
        //// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#remainder-operator
        //public static double GetFractionalPart(double num)
        //    //=> value - Math.Truncate(num);
        //    => num % 1d;

        // Extract from
        // https://randomascii.wordpress.com/2012/02/25/comparing-floating-point-numbers-2012-edition/
        //
        //   1. If you are comparing against zero, then relative epsilons and ULPs
        //   based comparisons are usually meaningless. You'll need to use an
        //   absolute epsilon, whose value might be some small multiple of
        //   FLT_EPSILON and the inputs to your calculation. Maybe.
        //
        //   2. If you are comparing against a non-zero number then relative epsilons
        //   or ULPs based comparisons are probably what you want. You'll probably
        //   want some small multiple of FLT_EPSILON for your relative epsilon, or
        //   some small number of ULPs. An absolute epsilon could be used if you knew
        //   exactly what number you were comparing against.
        //
        //   3. If you are comparing two arbitrary numbers that could be zero or
        //   non-zero then you need the kitchen sink. Good luck and God speed.
        //
        // Références :
        // - https://randomascii.wordpress.com/2012/02/25/comparing-floating-point-numbers-2012-edition/
        // - https://docs.microsoft.com/en-us/dotnet/api/system.double.equals
        // - https://floating-point-gui.de/errors/comparison/

        // REVIEW: valeur par défaut de tolerance : 1e-9 ?
        // Valeur de precision, tout en sachent qu'on s'intéresse principalement
        // à des unités de temps.
        // Comportement avec NaN, +/-Infinity.

        /// <summary>
        /// Rounds two doubles towards the nearest even number using the
        /// specified number of decimal places, then compare the results for
        /// equality.
        /// </summary>
        // Comparaison après arrondi.
        // 0 <= precision <= 15
        // precision = number of decimal places to be taken into consideration
        // when performing a comparison between two doubles.
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreApproximatelyEqual(double x, double y, int precision = 7)
            => Math.Round(x, precision, MidpointRounding.ToEven)
                == Math.Round(y, precision, MidpointRounding.ToEven);

        // Comparaison absolue puis comparaison relative.
        // 1. threshold > 0, seuil absolu à ne pas dépasser.
        // 2. tolerance > 0, tolérance relative, devrait être un petit multiple
        //    de Epsilon, et idéalement choisie en fonction de |x| et |y|.
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreNearby(
            double x, double y, double threshold, double tolerance = Double.Epsilon)
        {
            if (x == y) { return true; }

            double diff = Math.Abs(x - y);
            return
                // Comparaison absolue pour des nombres vraiment très proches.
                diff <= threshold
                // Comparaison relative.
                // Cette méthode ne marche pas quand (x - y) est très proche de
                // zéro, ce qui explique qu'on fasse d'abord une comparaison
                // absolue.
                || diff <= tolerance * Math.Max(Math.Abs(x), Math.Abs(y));
        }

        // Comparaison absolue puis comparaison des ULPs = "Units in the Last
        // Place" (Goldberg).
        // 1. threshold > 0, seuil absolu à ne pas dépasser.
        // 2. ulpThreshold > 0
        public static bool AreNearby(
            double x, double y, double threshold, int ulpThreshold)
        {
            // Comparaison absolue pour des nombres vraiment très proches.
            if (x == y || Math.Abs(x - y) <= threshold) { return true; }

            // Comparaison des ULPs.
            //   For numbers that are slightly below a power of two the FLT_EPSILON
            //   technique is twice as lenient. In other words, if we compare 4.0 to
            //   4.0 plus two ULPs then a one ULPs comparison and a FLT_EPSILON
            //   relative comparison will both say they are not equal. However if you
            //   compare 4.0 to 4.0 minus two ULPs then a one ULPs comparison will say
            //   they are not equal (of course) but a FLT_EPSILON relative comparison
            //   will say that they are equal.
            //   . . . 4-ulp 4 4+ulp . . .
            // À droite la distance est égale à 4 * epsilon.
            // Cette méthode ne marche pas quand (x - y) est très proche de zéro
            // car un très très petit double peut avoir une représentation entière
            // très grande.
            // https://randomascii.wordpress.com/2012/02/25/comparing-floating-point-numbers-2012-edition/
            //
            // Attention, problèmes éventuels de portabilité et de performance.
            // https://randomascii.wordpress.com/2012/01/11/tricks-with-the-floating-point-format/

            long xbits = BitConverter.DoubleToInt64Bits(x);
            long ybits = BitConverter.DoubleToInt64Bits(y);

            // Signes différents, retourne "false" sauf pour +0 et -0, cependant
            // ce dernier cas a déjà été traité lors de la comparaison absolue,
            // si cela n'avait pas été le cas, il aurait fallu écrire :
            // > if (xbits >> 63 != ybits >> 63) { return x == y; }
            if (xbits >> 63 != ybits >> 63) { return false; }

            // Comparaison de la distance (nombre de ULPs) entre les deux
            // doubles. La valeur absolue calculée est égale à 1 plus le nombre
            // de doubles entre x et y, en quelque sorte, il s'agit de la
            // distance entre leurs représentations entières.
            // Distance ULP = 1 quand les deux nombres sont adjacents.
            return Math.Abs(xbits - ybits) <= ulpThreshold;
        }
    }

    // Decimals.
    internal partial class MathOperations
    {
        internal const int HalfOneMin = Int32.MinValue / 10;
        internal const int HalfOneMax = Int32.MaxValue / 10;

        // REVIEW: AddHalfOne/SubtractHalfOne en passer par un long quand on
        // sort des limites HalfOneMin/Max.
        // On pourrait aussi traiter le cas général (1 <= b <= 9),
        //public static decimal Add(int num, byte b)
        //    => num >= 0
        //        ? new decimal(checked(10 * num + b), 0x00000000, 0x00000000, false, 0x0001)
        //        : new decimal(checked(-10 * num - b), 0x00000000, 0x00000000, true, 0x0001);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal AddHalfOne(int num)
            => num < HalfOneMin || num > HalfOneMax
                ? num + .5m
                : num >= 0
                    ? new decimal(10 * num + 5, 0x00000000, 0x00000000, false, 0x0001)
                    : new decimal(-10 * num - 5, 0x00000000, 0x00000000, true, 0x0001);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal SubtractHalfOne(int num)
            => num < HalfOneMin || num > HalfOneMax
                ? num - .5m
                : num > 0
                    ? new decimal(10 * num - 5, 0x00000000, 0x00000000, false, 0x0001)
                    : new decimal(-10 * num + 5, 0x00000000, 0x00000000, true, 0x0001);
    }

    // Logarithme décimal.
    internal partial class MathOperations
    {
        /// <summary>
        /// Calculates the integer log base 10 of a strictly positive 32-bit
        /// signed integer.
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Log10(int num)
        {
            Debug.Assert(num > 0);

            int n = 0;
            while (num > 9)
            {
                num /= 10;
                n++;
            }
            return n;
        }

        /// <summary>
        /// Calculates the number of digits of a positive 32-bit signed integer.
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int AdjustedLog10(int num)
        {
            Debug.Assert(num >= 0);

            int n = 1;
            while (num > 9)
            {
                num /= 10;
                n++;
            }
            return n;
        }

        /// <summary>
        /// Calculates the number of digits of a strictly positive 32-bit signed
        /// integer when we know for sure that <paramref name="num"/> is greater
        /// than or equal to a power of 10.
        /// <para>For this method to work, <paramref name="pow0"/> MUST be the
        /// (<paramref name="n0"/> - 1)th-power of 10.</para>
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int AdjustedLog10(int num, int pow0, int n0)
        {
            Debug.Assert(num >= pow0);
            Debug.Assert(pow0 == (int)Math.Pow(10, n0 - 1));

            int n = n0;
            num /= pow0;
            while (num > 9)
            {
                num /= 10;
                n++;
            }
            return n;
        }

        /// <summary>
        /// Calculates the number of digits of a positive 32-bit signed integer
        /// and also returns the greatest power of 10 lower than or equal to
        /// the specified integer in an output parameter.
        /// </summary>
        // pow = 10^(result - 1).
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AdjustedLog10(int num, out int pow)
        {
            Debug.Assert(num >= 0);

            // Algorithme brut (attention aux débordements arithmétiques).
            // > long pow64 = 10;
            // > while (pow64 <= num)
            // > {
            // >   l++;
            // >   pow64 *= 10;
            // > }
            // > pow = unchecked((int)(pow64 / 10));
            // Version modifiée : on ajoute une étape mais on ne risque pas de
            // débordements arihtmétiques.
            int n = 1;
            pow = 1;
            int upper = num / 10;
            while (pow <= upper)
            {
                n++;
                pow *= 10;
            }
            return n;
        }

        /// <summary>
        /// Calculates the number of digits of a strictly positive 32-bit signed
        /// integer when we know for sure that <paramref name="num"/> is greater
        /// than or equal to a power of 10, and also returns the greatest power
        /// of 10 lower than or equal to the specified integer in an output
        /// parameter.
        /// <para>For this method to work, <paramref name="pow0"/> MUST be the
        /// (<paramref name="n0"/> - 1)th-power of 10.</para>
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AdjustedLog10(int num, int pow0, int n0, out int pow)
        {
            Debug.Assert(num >= pow0);
            Debug.Assert(pow0 == (int)Math.Pow(10, n0 - 1));

            int n = n0;
            pow = pow0;
            int upper = num / 10;
            while (pow <= upper)
            {
                n++;
                pow *= 10;
            }
            return n;
        }
    }
}
