// See LICENSE in the project root for license information.

namespace Abc.Utilities
{
    using System;

    using Xunit;

    using EF = ExceptionFactory;

    public static partial class ExceptionFactoryTests
    {
        private static void CheckException<T>(T ex) where T : Exception
        {
            Assert.IsType<T>(ex);
            Assert.NotNull(ex);
            Assert.NotNull(ex.Message);
        }

        private static void CheckArgumentException(ArgumentException ex)
        {
            Assert.NotNull(ex);
            Assert.NotNull(ex.Message);
            Assert.Equal("paramName", ex.ParamName);
        }
    }

    public partial class ExceptionFactoryTests
    {
        [Fact] public static void ControlFlow() => CheckException(EF.ControlFlow);

        [Fact] public static void EmptySequence() => CheckException(EF.EmptySequence);

        [Fact] public static void ReadOnlyCollection() => CheckException(EF.ReadOnlyCollection);
    }

    public partial class ExceptionFactoryTests
    {
        [Fact]
        public static void InvalidType()
        {
            // Act
            var ex = EF.InvalidType("paramName", typeof(string), 1);
            // Assert
            CheckArgumentException(ex);
        }
    }
}
