// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Globalization;

    using Xunit;

    using Assert = AssertEx;

    // We do not need to be comprehensive, the May helpers only wrap BCL methods.

    // FIXME: more test data, default parsers are culture-dependent.

    public static partial class MayTests { }

    // Parsers for simple value types.
    public partial class MayTests
    {
        private static readonly NumberFormatInfo s_EmptyNfi = new NumberFormatInfo();

        private static readonly NumberFormatInfo s_CurrencyNfi
            = new NumberFormatInfo()
            {
                CurrencySymbol = "$",
            };

        [Fact]
        public static void ParseXXX_None()
        {
            Assert.None(May.ParseInt16(null));
            Assert.None(May.ParseInt16("XXX"));

            Assert.None(May.ParseInt16(null, NumberStyles.Integer, CultureInfo.InvariantCulture));
            Assert.None(May.ParseInt16("XXX", NumberStyles.Integer, CultureInfo.InvariantCulture));

            Assert.None(May.ParseInt32(null));
            Assert.None(May.ParseInt32("XXX"));

            Assert.None(May.ParseInt32(null, NumberStyles.Integer, CultureInfo.InvariantCulture));
            Assert.None(May.ParseInt32("XXX", NumberStyles.Integer, CultureInfo.InvariantCulture));

            Assert.None(May.ParseInt64(null));
            Assert.None(May.ParseInt64("XXX"));

            Assert.None(May.ParseInt64(null, NumberStyles.Integer, CultureInfo.InvariantCulture));
            Assert.None(May.ParseInt64("XXX", NumberStyles.Integer, CultureInfo.InvariantCulture));

            Assert.None(May.ParseSingle(null));
            Assert.None(May.ParseSingle("XXX"));

            Assert.None(May.ParseSingle(null, NumberStyles.Float, CultureInfo.InvariantCulture));
            Assert.None(May.ParseSingle("XXX", NumberStyles.Float, CultureInfo.InvariantCulture));

            Assert.None(May.ParseDouble(null));
            Assert.None(May.ParseDouble("XXX"));

            Assert.None(May.ParseDouble(null, NumberStyles.Float, CultureInfo.InvariantCulture));
            Assert.None(May.ParseDouble("XXX", NumberStyles.Float, CultureInfo.InvariantCulture));

            Assert.None(May.ParseDecimal(null));
            Assert.None(May.ParseDecimal("XXX"));

            Assert.None(May.ParseDecimal(null, NumberStyles.Float, CultureInfo.InvariantCulture));
            Assert.None(May.ParseDecimal("XXX", NumberStyles.Float, CultureInfo.InvariantCulture));

            Assert.None(May.ParseSByte(null));
            Assert.None(May.ParseSByte("XXX"));

            Assert.None(May.ParseSByte(null, NumberStyles.Integer, CultureInfo.InvariantCulture));
            Assert.None(May.ParseSByte("XXX", NumberStyles.Integer, CultureInfo.InvariantCulture));

            Assert.None(May.ParseByte(null));
            Assert.None(May.ParseByte("XXX"));

            Assert.None(May.ParseByte(null, NumberStyles.Integer, CultureInfo.InvariantCulture));
            Assert.None(May.ParseByte("XXX", NumberStyles.Integer, CultureInfo.InvariantCulture));

            Assert.None(May.ParseUInt16(null));
            Assert.None(May.ParseUInt16("XXX"));

            Assert.None(May.ParseUInt16(null, NumberStyles.Integer, CultureInfo.InvariantCulture));
            Assert.None(May.ParseUInt16("XXX", NumberStyles.Integer, CultureInfo.InvariantCulture));

            Assert.None(May.ParseUInt32(null));
            Assert.None(May.ParseUInt32("XXX"));

            Assert.None(May.ParseUInt32(null, NumberStyles.Integer, CultureInfo.InvariantCulture));
            Assert.None(May.ParseUInt32("XXX", NumberStyles.Integer, CultureInfo.InvariantCulture));

            Assert.None(May.ParseUInt64(null));
            Assert.None(May.ParseUInt64("XXX"));

            Assert.None(May.ParseUInt64(null, NumberStyles.Integer, CultureInfo.InvariantCulture));
            Assert.None(May.ParseUInt64("XXX", NumberStyles.Integer, CultureInfo.InvariantCulture));
        }

        #region ParseBoolean()

        // Adapted from
        // https://github.com/dotnet/runtime/blob/master/src/libraries/System.Runtime/tests/System/BooleanTests.cs#L52
        public static TheoryData<string?> BadBooleanData
            => new TheoryData<string?>
            {
                { null },
                { "" },
                { " " },
                { "Garbage" },
                { "True\0Garbage" },
                { "True\0True" },
                { "True True" },
                { "True False" },
                { "False True" },
                { "Fa lse" },
                { "T" },
                { "0" },
                { "1" },
            };

        // Adapted from
        // https://github.com/dotnet/runtime/blob/master/src/libraries/System.Runtime/tests/System/BooleanTests.cs#L24
        public static TheoryData<string, bool> BooleanData
            => new TheoryData<string, bool>
            {
                { "True", true },
                { "true", true },
                { "TRUE", true },
                { "tRuE", true },
                { "  True  ", true },
                { "True\0", true },
                { " \0 \0  True   \0 ", true },

                { "False", false },
                { "false", false },
                { "FALSE", false },
                { "fAlSe", false },
                { "False  ", false },
                { "False\0", false },
                { "  False \0\0\0  ", false },
            };

        [Theory, MemberData(nameof(BadBooleanData))]
        public static void ParseBoolean_None(string? input)
        {
            Assert.None(May.ParseBoolean(input));
        }

        [Theory, MemberData(nameof(BooleanData))]
        public static void ParseBoolean(string input, bool exp)
        {
            Assert.Some(exp, May.ParseBoolean(input));
        }

        #endregion

        #region ParseInt16()

        public static readonly TheoryData<string, short> Int16Data
            = new TheoryData<string, short>
            {
                { "-1", -1 },
                { "0", 0 },
                { "1", 1 }
            };

        [Theory, MemberData(nameof(Int16Data))]
        public static void ParseInt16(string input, short exp)
        {
            Assert.Some(exp, May.ParseInt16(input));
        }

        [Theory, MemberData(nameof(Int16Data))]
        public static void ParseInt16_Invariant(string input, short exp)
        {
            Assert.Some(exp, May.ParseInt16(input, NumberStyles.Integer, CultureInfo.InvariantCulture));
        }

        #endregion

        #region ParseInt32()

        public static readonly TheoryData<string, int> Int32Data
            = new TheoryData<string, int>
            {
                { "-1", -1 },
                { "0", 0 },
                { "1", 1 }
            };

        [Theory, MemberData(nameof(Int32Data))]
        public static void ParseInt32(string input, int exp)
        {
            Assert.Some(exp, May.ParseInt32(input));
        }

        [Theory, MemberData(nameof(Int32Data))]
        public static void ParseInt32_Invariant(string input, int exp)
        {
            Assert.Some(exp, May.ParseInt32(input, NumberStyles.Integer, CultureInfo.InvariantCulture));
        }

        #endregion

        #region ParseInt64()

        public static readonly TheoryData<string, long> Int64Data
            = new TheoryData<string, long>
            {
                { "-1", -1 },
                { "0", 0 },
                { "1", 1 }
            };

        [Theory, MemberData(nameof(Int64Data))]
        public static void ParseInt64(string input, long exp)
        {
            Assert.Some(exp, May.ParseInt64(input));
        }

        [Theory, MemberData(nameof(Int64Data))]
        public static void ParseInt64_Invariant(string input, long exp)
        {
            Assert.Some(exp, May.ParseInt64(input, NumberStyles.Integer, CultureInfo.InvariantCulture));
        }

        #endregion

        #region ParseSingle()

        public static readonly TheoryData<string, float> SingleData
            = new TheoryData<string, float>
            {
                { "-1", -1f },
                { "0", 0f },
                { "1", 1f }
            };

        [Theory, MemberData(nameof(SingleData))]
        public static void ParseSingle(string input, float exp)
        {
            Assert.Some(exp, May.ParseSingle(input));
        }

        [Theory, MemberData(nameof(SingleData))]
        [InlineData("-1.1", -1.1f)]
        [InlineData("1.1", 1.1f)]
        public static void ParseSingle_Invariant(string input, float exp)
        {
            Assert.Some(exp, May.ParseSingle(input, NumberStyles.Float, CultureInfo.InvariantCulture));
        }

        #endregion

        #region ParseDouble()

        public static readonly TheoryData<string, double> DoubleData
            = new TheoryData<string, double>
            {
                { "-1", -1d },
                { "0", 0d },
                { "1", 1d }
            };

        [Theory, MemberData(nameof(DoubleData))]
        public static void ParseDouble(string input, double exp)
        {
            Assert.Some(exp, May.ParseDouble(input));
        }

        [Theory, MemberData(nameof(DoubleData))]
        [InlineData("-1.1", -1.1d)]
        [InlineData("1.1", 1.1d)]
        public static void ParseDouble_Invariant(string input, double exp)
        {
            Assert.Some(exp, May.ParseDouble(input, NumberStyles.Float, CultureInfo.InvariantCulture));
        }

        #endregion

        #region ParseDecimal()

        [Fact]
        public static void ParseDecimal()
        {
            Assert.Some(0m, May.ParseDecimal("0"));
            Assert.Some(-1m, May.ParseDecimal("-1"));
            Assert.Some(1m, May.ParseDecimal("1"));
        }

        [Fact]
        public static void ParseDecimal_Invariant()
        {
            Assert.Some(0m, May.ParseDecimal("0", NumberStyles.Float, CultureInfo.InvariantCulture));
            Assert.Some(-1m, May.ParseDecimal("-1", NumberStyles.Float, CultureInfo.InvariantCulture));
            Assert.Some(1m, May.ParseDecimal("1", NumberStyles.Float, CultureInfo.InvariantCulture));
            Assert.Some(-1.1m, May.ParseDecimal("-1.1", NumberStyles.Float, CultureInfo.InvariantCulture));
            Assert.Some(1.1m, May.ParseDecimal("1.1", NumberStyles.Float, CultureInfo.InvariantCulture));
        }

        #endregion

        #region ParseSByte()

        public static readonly TheoryData<string, sbyte> SByteData
            = new TheoryData<string, sbyte>
            {
                { "-1", -1 },
                { "0", 0 },
                { "1", 1 }
            };

        [Theory, MemberData(nameof(SByteData))]
        public static void ParseSByte(string input, sbyte exp)
        {
            Assert.Some(exp, May.ParseSByte(input));
        }

        [Theory, MemberData(nameof(SByteData))]
        public static void ParseSByte_Invariant(string input, sbyte exp)
        {
            Assert.Some(exp, May.ParseSByte(input, NumberStyles.Integer, CultureInfo.InvariantCulture));
        }

        #endregion

        #region ParseByte()

        // Adapted from
        // https://github.com/dotnet/runtime/blob/master/src/libraries/System.Runtime/tests/System/ByteTests.cs#L165
        public static TheoryData<string, NumberStyles, NumberFormatInfo?, byte> ByteData
        {
            get
            {
                var defaultStyle = NumberStyles.Integer;

                return new TheoryData<string, NumberStyles, NumberFormatInfo?, byte>
                {
                    // Default style, no format provider.
                    { "0", defaultStyle, null, Byte.MinValue },
                    { "1", defaultStyle, null, 1 },
                    { "123", defaultStyle, null, 123 },
                    { "+123", defaultStyle, null, 123 },
                    { "  123  ", defaultStyle, null, 123 },
                    { "255", defaultStyle, null, Byte.MaxValue },

                    // Custom style, empty format provider.
                    { "12", NumberStyles.HexNumber, null, 0x12 },
                    { "10", NumberStyles.AllowThousands, null, 10 },

                    // Default style, empty format provider.
                    { "123", defaultStyle, s_EmptyNfi, 123 },

                    // Custom style.
                    { "123", NumberStyles.Any, s_EmptyNfi, 123 },
                    { "12", NumberStyles.HexNumber, s_EmptyNfi, 0x12 },
                    { "ab", NumberStyles.HexNumber, s_EmptyNfi, 0xab },
                    { "AB", NumberStyles.HexNumber, null, 0xab },
                    { "$100", NumberStyles.Currency, s_CurrencyNfi, 100 },
                };
            }
        }

        [Theory, MemberData(nameof(ByteData))]
        public static void ParseByte(
            string input, NumberStyles style, NumberFormatInfo? nfi, byte exp)
        {
            if (style != NumberStyles.Integer || nfi != null) { return; }
            Assert.Some(exp, May.ParseByte(input));
        }

        [Theory, MemberData(nameof(ByteData))]
        public static void ParseByte_Invariant(
            string input, NumberStyles style, NumberFormatInfo? nfi, byte exp)
        {
            Assert.Some(exp, May.ParseByte(input, style, nfi));
        }

        #endregion

        #region ParseUInt16()

        public static readonly TheoryData<string, ushort> UInt16Data
            = new TheoryData<string, ushort>
            {
                { "0", 0 },
                { "1", 1 }
            };

        [Theory, MemberData(nameof(UInt16Data))]
        public static void ParseUInt16(string input, ushort exp)
        {
            Assert.Some(exp, May.ParseUInt16(input));
        }

        [Theory, MemberData(nameof(UInt16Data))]
        public static void ParseUInt16_Invariant(string input, ushort exp)
        {
            Assert.Some(exp, May.ParseUInt16(input, NumberStyles.Integer, CultureInfo.InvariantCulture));
        }

        #endregion

        #region ParseUInt32()

        public static readonly TheoryData<string, uint> UInt32Data
            = new TheoryData<string, uint>
            {
                { "0", 0 },
                { "1", 1 }
            };

        [Theory, MemberData(nameof(UInt32Data))]
        public static void ParseUInt32(string input, uint exp)
        {
            Assert.Some(exp, May.ParseUInt32(input));
        }

        [Theory, MemberData(nameof(UInt32Data))]
        public static void ParseUInt32_Invariant(string input, uint exp)
        {
            Assert.Some(exp, May.ParseUInt32(input, NumberStyles.Integer, CultureInfo.InvariantCulture));
        }

        #endregion

        #region ParseUInt64()

        public static readonly TheoryData<string, ulong> UInt64Data
            = new TheoryData<string, ulong>
            {
                { "0", 0 },
                { "1", 1 }
            };

        [Theory, MemberData(nameof(UInt64Data))]
        public static void ParseUInt64(string input, ulong exp)
        {
            Assert.Some(exp, May.ParseUInt64(input));
        }

        [Theory, MemberData(nameof(UInt64Data))]
        public static void ParseUInt64_Invariant(string input, ulong exp)
        {
            Assert.Some(exp, May.ParseUInt64(input, NumberStyles.Integer, CultureInfo.InvariantCulture));
        }

        #endregion
    }

    // ParseEnum().
    public partial class MayTests
    {
        [Theory]
        // By value.
        [InlineData("1")]
        [InlineData("1 ")]
        [InlineData(" 1")]
        [InlineData(" 1 ")]
        // By name.
        [InlineData("One")]
        [InlineData("One ")]
        [InlineData(" One")]
        [InlineData(" One ")]
        // By alias.
        [InlineData("Alias1")]
        [InlineData("Alias1 ")]
        [InlineData(" Alias1")]
        [InlineData(" Alias1 ")]
        public static void ParseEnum(string input)
        {
            Assert.Some(SimpleEnum.One, May.ParseEnum<SimpleEnum>(input));
            Assert.Some(SimpleEnum.One, May.ParseEnum<SimpleEnum>(input, ignoreCase: false));
            Assert.Some(SimpleEnum.One, May.ParseEnum<SimpleEnum>(input, ignoreCase: true));
        }

        [Theory]
        [InlineData("one")]
        [InlineData("oNe")]
        [InlineData("onE")]
        [InlineData("one ")]
        [InlineData(" one")]
        [InlineData(" one ")]
        [InlineData("oNe ")]
        [InlineData(" oNe")]
        [InlineData(" oNe ")]
        // Alias.
        [InlineData("alias1")]
        [InlineData("aliaS1")]
        [InlineData("alias1 ")]
        [InlineData(" alias1")]
        [InlineData(" alias1 ")]
        public static void ParseEnum_MixedCase(string input)
        {
            Assert.None(May.ParseEnum<SimpleEnum>(input));
            Assert.None(May.ParseEnum<SimpleEnum>(input, ignoreCase: false));
            Assert.Some(SimpleEnum.One, May.ParseEnum<SimpleEnum>(input, ignoreCase: true));
        }

        [Theory]
        [InlineData("4")]
        [InlineData("4 ")]
        [InlineData(" 4")]
        [InlineData(" 4 ")]
        [CLSCompliant(false)]
        // Weird but passing any integer value will succeed.
        public static void ParseEnum_AnyInteger(string input)
        {
            Assert.Some((SimpleEnum)4, May.ParseEnum<SimpleEnum>(input));
            Assert.Some((SimpleEnum)4, May.ParseEnum<SimpleEnum>(input, ignoreCase: false));
            Assert.Some((SimpleEnum)4, May.ParseEnum<SimpleEnum>(input, ignoreCase: true));
        }

        [Theory]
        [InlineData("a")]
        [InlineData("a ")]
        [InlineData(" a")]
        [InlineData(" a ")]
        [InlineData("Whatever")]
        [InlineData("Whatever ")]
        [InlineData(" Whatever")]
        [InlineData(" Whatever ")]
        public static void ParseEnum_InvalidName(string input)
        {
            Assert.None(May.ParseEnum<SimpleEnum>(input));
            Assert.None(May.ParseEnum<SimpleEnum>(input, ignoreCase: false));
            Assert.None(May.ParseEnum<SimpleEnum>(input, ignoreCase: true));
        }

        [Theory]
        [InlineData("One,Two")]
        [InlineData("One,Two ")]
        [InlineData(" One,Two")]
        [InlineData(" One,Two ")]
        [InlineData("One, Two")]
        [InlineData("One,  Two")]
        [InlineData("One,   Two")]
        [InlineData(" One, Two")]
        [InlineData(" One,  Two")]
        [InlineData(" One, Two ")]
        [InlineData(" One,  Two ")]
        public static void ParseEnum_CompositeValue(string input)
        {
            Assert.Some(FlagsEnum.OneTwo, May.ParseEnum<FlagsEnum>(input));
            Assert.Some(FlagsEnum.OneTwo, May.ParseEnum<FlagsEnum>(input, ignoreCase: false));
            Assert.Some(FlagsEnum.OneTwo, May.ParseEnum<FlagsEnum>(input, ignoreCase: true));
        }

        [Theory]
        [InlineData("one,two")]
        [InlineData("oNe,two")]
        [InlineData("one,tWo")]
        [InlineData("one,two ")]
        [InlineData(" one,two")]
        [InlineData(" one,two ")]
        [InlineData("one, two")]
        [InlineData("one,  two")]
        [InlineData("one, two ")]
        [InlineData(" one, two")]
        [InlineData(" one, two ")]
        public static void ParseEnum_CompositeValue_MixedCase(string input)
        {
            Assert.None(May.ParseEnum<FlagsEnum>(input));
            Assert.None(May.ParseEnum<FlagsEnum>(input, ignoreCase: false));
            Assert.Some(FlagsEnum.OneTwo, May.ParseEnum<FlagsEnum>(input, ignoreCase: true));
        }

        [Theory]
        [InlineData("OneTwo")]
        [InlineData("onetwo")]
        [InlineData("onetWo")]
        public static void ParseEnum_Flags_InvalidName(string input)
        {
            Assert.None(May.ParseEnum<SimpleEnum>(input));
            Assert.None(May.ParseEnum<SimpleEnum>(input, ignoreCase: false));
            Assert.None(May.ParseEnum<SimpleEnum>(input, ignoreCase: true));
        }
    }

    // ParseDateTime() & ParseDateTimeExactly().
    public partial class MayTests
    {
        [Fact]
        public static void ParseDateTime_None()
        {
            Assert.None(May.ParseDateTime(null));
            Assert.None(May.ParseDateTime("XXX"));

            Assert.None(May.ParseDateTime(null, CultureInfo.CurrentCulture, DateTimeStyles.None));
            Assert.None(May.ParseDateTime("XXX", CultureInfo.CurrentCulture, DateTimeStyles.None));
        }
    }

    // CreateUri().
    public partial class MayTests
    {
#pragma warning disable CA2234 // Pass system uri objects instead of strings

        [Fact]
        public static void CreateUri_None()
        {
            // Arrange
            var baseUri = new Uri("http://www.narvalo.org");
            var relativeUri = new Uri("about", UriKind.Relative);

            // Act & Assert
            Assert.None(May.CreateUri(null, ""));
            Assert.None(May.CreateUri(baseUri, (string?)null));

            Assert.None(May.CreateUri(null, relativeUri));
            Assert.None(May.CreateUri(baseUri, (Uri?)null));

            Assert.None(May.CreateUri(null, UriKind.Absolute));
            Assert.None(May.CreateUri("about", UriKind.Absolute));
            Assert.None(May.CreateUri("http://www.narvalo.org", UriKind.Relative));
        }

        [Fact]
        public static void CreateUri_Some()
        {
            // Arrange
            var baseUri = new Uri("http://www.narvalo.org");
            var relativeUri = new Uri("about", UriKind.Relative);
            var exp = new Uri("http://www.narvalo.org/about");
            // Act & Assert
            Assert.Some(exp, May.CreateUri(baseUri, "about"));
            Assert.Some(exp, May.CreateUri(baseUri, relativeUri));
            Assert.Some(relativeUri, May.CreateUri("about", UriKind.Relative));
            Assert.Some(baseUri, May.CreateUri("http://www.narvalo.org", UriKind.Absolute));
        }

#pragma warning restore CA2234
    }
}
