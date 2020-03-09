﻿// See LICENSE.txt in the project root for license information.

namespace Abc.Utilities
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides static helpers to produce new sequences.
    /// <para>This class cannot be inherited.</para>
    /// </summary>
    internal static class Sequence
    {
        /// <summary>
        /// Generates an infinite sequence of one repeated value.
        /// </summary>
        public static IEnumerable<T> Repeat<T>(T value)
        {
            // REVIEW: optimize Repeat().
            while (true)
            {
                yield return value;
            }
        }
    }
}