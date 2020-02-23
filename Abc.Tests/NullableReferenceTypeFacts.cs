// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Xunit;

    using static global::My;

    using Assert = AssertEx;

    // Even if "nullable" is globally enabled, we still have to test null's.
    public static class NullableReferenceTypeFacts
    {
        [Fact]
        public static void NullForgiving()
        {
            // Constructor.
            Assert.Null(new Sample(null!).Value);
            // Property setter.
            Assert.Null((new Sample { Value = null! }).Value);
            // Method arg.
            Assert.Null(Sample.Create(NullNullString!).Value);
        }

        [Fact]
        public static void NullableDisable()
        {
#nullable disable
            // Constructor.
            Assert.Null(new Sample(null).Value);
            Assert.Null(new Sample<string>(NullNullString).Value);
            // Property setter.
            Assert.Null((new Sample { Value = null }).Value);
            // Method arg.
            Assert.Null(Sample.Create(NullNullString).Value);
#nullable restore
        }

        private sealed class Sample
        {
            public Sample() { }

            public Sample([DisallowNull]string value) => Value = value;

            [DisallowNull]
            public string Value { get; set; } = String.Empty;

            public static Sample<T> Create<T>(T value)
                where T : notnull
            {
                return new Sample<T>(value);
            }
        }

        private sealed class Sample<T> where T : notnull
        {
            public Sample(T value) => Value = value;

            public T Value { get; set; }
        }
    }
}
