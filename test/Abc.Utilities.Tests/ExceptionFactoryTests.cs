// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

namespace Abc.Utilities
{
    using System;

    using Xunit;

    using Assert = AssertEx;
    using EF = ExceptionFactory;

    public static partial class ExceptionFactoryTests { }

    public partial class ExceptionFactoryTests
    {
        [Fact] public static void ControlFlow() => 
            Assert.CheckException(typeof(InvalidOperationException), EF.ControlFlow);

        [Fact] public static void EmptySequence() => 
            Assert.CheckException(typeof(InvalidOperationException), EF.EmptySequence);

        [Fact] public static void ReadOnlyCollection() => 
            Assert.CheckException(typeof(NotSupportedException), EF.ReadOnlyCollection);
    }

    // Argument exceptions.
    public partial class ExceptionFactoryTests
    {
        [Fact]
        public static void InvalidType()
        {
            // Act
            var ex = EF.InvalidType("paramName", typeof(string), 1);
            // Assert
            Assert.CheckArgumentException(ex);
        }
    }
}
