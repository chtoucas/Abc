// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

namespace Abc
{
    using System;
    using System.Threading.Tasks;

    using Xunit;

    using Anexn = System.ArgumentNullException;
    using Aoorexn = System.ArgumentOutOfRangeException;

    /// <summary>
    /// Contains various static methods that are used to verify that conditions
    /// are met during the process of running tests.
    /// </summary>
    public abstract partial class AssertEx : Assert
    {
        protected AssertEx() { }

        /// <summary>
        /// Async test helpers.
        /// </summary>
        public static partial class Async { }
    }

    public partial class AssertEx
    {
        /// <summary>
        /// Fails with a user message.
        /// </summary>
        public static void Fails(string userMessage) => True(false, userMessage);
    }

    public partial class AssertEx
    {
        /// <summary>
        /// Verifies that an exception is exactly the given exception type
        /// (and not a derived one), then that its message is not null.
        /// </summary>
        public static void CheckException(Type expectedExceptionType, Exception exn)
        {
            if (exn is null) { throw new Anexn(nameof(exn)); }

            IsType(expectedExceptionType, exn);
            NotNull(exn.Message);
        }

        /// <summary>
        /// Verifies that an exception message is not null, then that the name
        /// of the parameter that causes the exception is equal to
        /// <paramref name="expectedParamName"/>.
        /// </summary>
        public static void CheckArgumentException(string expectedParamName, ArgumentException exn)
        {
            if (exn is null) { throw new Anexn(nameof(exn)); }

            NotNull(exn.Message);
            Equal(expectedParamName, exn.ParamName);
        }

        ///// <summary>
        ///// Verifies that the specified delegate throws an exception of type
        ///// <see cref="ArgumentException"/> (and not a derived exception type)
        ///// with a null parameter name.
        ///// </summary>
        //public static void ThrowsArgumentException(Action testCode) =>
        //    Throws<ArgumentException>(null, testCode);

        ///// <summary>
        ///// Verifies that the specified delegate throws an exception of type
        ///// <see cref="ArgumentException"/> (and not a derived exception type)
        ///// with a null parameter name.
        ///// </summary>
        //public static void ThrowsArgumentException(Func<object> testCode) =>
        //    Throws<ArgumentException>(null, testCode);

        //
        // Below, use "argName" instead of "paramName" to avoid an error CA1507
        // in the caller code.
        //

        /// <summary>
        /// Verifies that the specified delegate throws an exception of type
        /// <see cref="ArgumentException"/> (and not a derived exception type).
        /// </summary>
        public static void ThrowsArgumentException(string argName, Action testCode) =>
            Throws<ArgumentException>(argName, testCode);

        /// <summary>
        /// Verifies that the specified delegate throws an exception of type
        /// <see cref="ArgumentException"/> (and not a derived exception type).
        /// </summary>
        public static void ThrowsArgumentException(string argName, Func<object> testCode) =>
            Throws<ArgumentException>(argName, testCode);

        /// <summary>
        /// Verifies that the specified delegate throws an exception of type
        /// <see cref="Anexn"/> (and not a derived exception type).
        /// </summary>
        public static void ThrowsAnexn(string argName, Action testCode) =>
            Throws<Anexn>(argName, testCode);

        /// <summary>
        /// Verifies that the specified delegate throws an exception of type
        /// <see cref="Anexn"/> (and not a derived exception type).
        /// </summary>
        public static void ThrowsAnexn(string argName, Func<object> testCode) =>
            Throws<Anexn>(argName, testCode);

        /// <summary>
        /// Verifies that the specified delegate throws an exception of type
        /// <see cref="Aoorexn"/> (and not a derived exception type).
        /// </summary>
        public static void ThrowsAoorexn(string argName, Action testCode) =>
            Throws<Aoorexn>(argName, testCode);

        /// <summary>
        /// Verifies that the specified delegate throws an exception of type
        /// <see cref="Aoorexn"/> (and not a derived exception type).
        /// </summary>
        public static void ThrowsAoorexn(string argName, Func<object> testCode) =>
            Throws<Aoorexn>(argName, testCode);
    }

    // Async.
    public partial class AssertEx
    {
        public partial class Async
        {
            /// <summary>
            /// Verifies that the specified delegate throws an exception of type
            /// <see cref="Anexn"/> (and not a derived exception type).
            /// <para>Fails if the delegate uses eager (synchronous) validation.
            /// </para>
            /// </summary>
            public static async Task ThrowsAnexn(string argName, Func<Task> testCode)
            {
                if (testCode is null) { throw new Anexn(nameof(testCode)); }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                try
                {
                    testCode();
                }
                catch (Anexn)
                {
                    throw new InvalidOperationException(
                        "The specified task uses eager (synchronous) validation.");
                }
#pragma warning restore CS4014

                await ThrowsAsync<Anexn>(argName, testCode);
            }
        }
    }
}
