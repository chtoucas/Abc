// See LICENSE in the project root for license information.

namespace Abc.Utilities
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    [DebuggerNonUserCode]
    internal abstract partial class Predicates
    {
        [ExcludeFromCodeCoverage]
        protected Predicates() { }
    }

    [DebuggerNonUserCode]
    internal abstract partial class Predicates<TSource>
    {
        [ExcludeFromCodeCoverage]
        protected Predicates() { }
    }

    internal partial class Predicates
    {
        /// <summary>
        /// Represents the function that always returns false.
        /// <para>This field is read-only.</para>
        /// </summary>
#if !ABC_UTILITIES_ENABLE_CODE_COVERAGE
        [ExcludeFromCodeCoverage]
#endif
        public static readonly Func<bool> False = () => false;

        /// <summary>
        /// Represents the function that always returns true.
        /// <para>This field is read-only.</para>
        /// </summary>
#if !ABC_UTILITIES_ENABLE_CODE_COVERAGE
        [ExcludeFromCodeCoverage]
#endif
        public static readonly Func<bool> True = () => true;
    }

    internal partial class Predicates<TSource>
    {
        /// <summary>
        /// Represents the predicate that always evaluates to false.
        /// <para>This field is read-only.</para>
        /// </summary>
#if !ABC_UTILITIES_ENABLE_CODE_COVERAGE
        [ExcludeFromCodeCoverage]
#endif
        public static readonly Func<TSource, bool> False = _ => false;

        /// <summary>
        /// Represents the predicate that always evaluates to true.
        /// <para>This field is read-only.</para>
        /// </summary>
#if !ABC_UTILITIES_ENABLE_CODE_COVERAGE
        [ExcludeFromCodeCoverage]
#endif
        public static readonly Func<TSource, bool> True = _ => true;
    }
}
