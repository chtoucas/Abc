// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Globalization;

    using Xunit;

    using Assert = AssertEx;

    // We do not need to be comprehensive, the May helpers only wrap BCL methods.

    // FIXME: default parsers are culture-dependent.

    public static partial class MayTests { }

    // Parsers for simple value types.
    public partial class MayTests
    {
        [Fact]
        public static void ParseXXX_None()
        {
            Assert.None(May.ParseBoolean(null));
            Assert.None(May.ParseBoolean("XXX"));

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

        [Theory]
        [InlineData("true", true)]
        [InlineData("false", false)]
        public static void ParseBoolean(string value, bool exp)
        {
            Assert.Some(exp, May.ParseBoolean(value));
        }

        public static readonly TheoryData<string, short> Int16Data
            = new TheoryData<string, short>
            {
                { "-1", -1 },
                { "0", 0 },
                { "1", 1 }
            };

        [Theory, MemberData(nameof(Int16Data))]
        public static void ParseInt16(string value, short exp)
        {
            Assert.Some(exp, May.ParseInt16(value));
        }

        [Theory, MemberData(nameof(Int16Data))]
        public static void ParseInt16_Invariant(string value, short exp)
        {
            Assert.Some(exp, May.ParseInt16(value, NumberStyles.Integer, CultureInfo.InvariantCulture));
        }

        public static readonly TheoryData<string, int> Int32Data
            = new TheoryData<string, int>
            {
                { "-1", -1 },
                { "0", 0 },
                { "1", 1 }
            };

        [Theory, MemberData(nameof(Int32Data))]
        public static void ParseInt32(string value, int exp)
        {
            Assert.Some(exp, May.ParseInt32(value));
        }

        [Theory, MemberData(nameof(Int32Data))]
        public static void ParseInt32_Invariant(string value, int exp)
        {
            Assert.Some(exp, May.ParseInt32(value, NumberStyles.Integer, CultureInfo.InvariantCulture));
        }

        public static readonly TheoryData<string, long> Int64Data
            = new TheoryData<string, long>
            {
                { "-1", -1 },
                { "0", 0 },
                { "1", 1 }
            };

        [Theory, MemberData(nameof(Int64Data))]
        public static void ParseInt64(string value, long exp)
        {
            Assert.Some(exp, May.ParseInt64(value));
        }

        [Theory, MemberData(nameof(Int64Data))]
        public static void ParseInt64_Invariant(string value, long exp)
        {
            Assert.Some(exp, May.ParseInt64(value, NumberStyles.Integer, CultureInfo.InvariantCulture));
        }

        public static readonly TheoryData<string, float> SingleData
            = new TheoryData<string, float>
            {
                { "-1", -1f },
                { "0", 0f },
                { "1", 1f }
            };

        [Theory, MemberData(nameof(SingleData))]
        public static void ParseSingle(string value, float exp)
        {
            Assert.Some(exp, May.ParseSingle(value));
        }

        [Theory, MemberData(nameof(SingleData))]
        [InlineData("-1.1", -1.1f)]
        [InlineData("1.1", 1.1f)]
        public static void ParseSingle_Invariant(string value, float exp)
        {
            Assert.Some(exp, May.ParseSingle(value, NumberStyles.Float, CultureInfo.InvariantCulture));
        }

        public static readonly TheoryData<string, double> DoubleData
            = new TheoryData<string, double>
            {
                { "-1", -1d },
                { "0", 0d },
                { "1", 1d }
            };

        [Theory, MemberData(nameof(DoubleData))]
        public static void ParseDouble(string value, double exp)
        {
            Assert.Some(exp, May.ParseDouble(value));
        }

        [Theory, MemberData(nameof(DoubleData))]
        [InlineData("-1.1", -1.1d)]
        [InlineData("1.1", 1.1d)]
        public static void ParseDouble_Invariant(string value, double exp)
        {
            Assert.Some(exp, May.ParseDouble(value, NumberStyles.Float, CultureInfo.InvariantCulture));
        }

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

        public static readonly TheoryData<string, ushort> UInt16Data
            = new TheoryData<string, ushort>
            {
                { "0", 0 },
                { "1", 1 }
            };

        [Theory, MemberData(nameof(UInt16Data))]
        public static void ParseUInt16(string value, ushort exp)
        {
            Assert.Some(exp, May.ParseUInt16(value));
        }

        [Theory, MemberData(nameof(UInt16Data))]
        public static void ParseUInt16_Invariant(string value, ushort exp)
        {
            Assert.Some(exp, May.ParseUInt16(value, NumberStyles.Integer, CultureInfo.InvariantCulture));
        }

        public static readonly TheoryData<string, uint> UInt32Data
            = new TheoryData<string, uint>
            {
                { "0", 0 },
                { "1", 1 }
            };

        [Theory, MemberData(nameof(UInt32Data))]
        public static void ParseUInt32(string value, uint exp)
        {
            Assert.Some(exp, May.ParseUInt32(value));
        }

        [Theory, MemberData(nameof(UInt32Data))]
        public static void ParseUInt32_Invariant(string value, uint exp)
        {
            Assert.Some(exp, May.ParseUInt32(value, NumberStyles.Integer, CultureInfo.InvariantCulture));
        }

        public static readonly TheoryData<string, ulong> UInt64Data
            = new TheoryData<string, ulong>
            {
                { "0", 0 },
                { "1", 1 }
            };

        [Theory, MemberData(nameof(UInt64Data))]
        public static void ParseUInt64(string value, ulong exp)
        {
            Assert.Some(exp, May.ParseUInt64(value));
        }

        [Theory, MemberData(nameof(UInt64Data))]
        public static void ParseUInt64_Invariant(string value, ulong exp)
        {
            Assert.Some(exp, May.ParseUInt64(value, NumberStyles.Integer, CultureInfo.InvariantCulture));
        }
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
        public static void ParseEnum(string value)
        {
            Assert.Some(SimpleEnum.One, May.ParseEnum<SimpleEnum>(value));
            Assert.Some(SimpleEnum.One, May.ParseEnum<SimpleEnum>(value, ignoreCase: false));
            Assert.Some(SimpleEnum.One, May.ParseEnum<SimpleEnum>(value, ignoreCase: true));
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
        public static void ParseEnum_MixedCase(string value)
        {
            Assert.None(May.ParseEnum<SimpleEnum>(value));
            Assert.None(May.ParseEnum<SimpleEnum>(value, ignoreCase: false));
            Assert.Some(SimpleEnum.One, May.ParseEnum<SimpleEnum>(value, ignoreCase: true));
        }

        [Theory]
        [InlineData("4")]
        [InlineData("4 ")]
        [InlineData(" 4")]
        [InlineData(" 4 ")]
        [CLSCompliant(false)]
        // Weird but passing any integer value will succeed.
        public static void ParseEnum_AnyInteger(string value)
        {
            Assert.Some((SimpleEnum)4, May.ParseEnum<SimpleEnum>(value));
            Assert.Some((SimpleEnum)4, May.ParseEnum<SimpleEnum>(value, ignoreCase: false));
            Assert.Some((SimpleEnum)4, May.ParseEnum<SimpleEnum>(value, ignoreCase: true));
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
        public static void ParseEnum_InvalidName(string value)
        {
            Assert.None(May.ParseEnum<SimpleEnum>(value));
            Assert.None(May.ParseEnum<SimpleEnum>(value, ignoreCase: false));
            Assert.None(May.ParseEnum<SimpleEnum>(value, ignoreCase: true));
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
        public static void ParseEnum_CompositeValue(string value)
        {
            Assert.Some(FlagsEnum.OneTwo, May.ParseEnum<FlagsEnum>(value));
            Assert.Some(FlagsEnum.OneTwo, May.ParseEnum<FlagsEnum>(value, ignoreCase: false));
            Assert.Some(FlagsEnum.OneTwo, May.ParseEnum<FlagsEnum>(value, ignoreCase: true));
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
        public static void ParseEnum_CompositeValue_MixedCase(string value)
        {
            Assert.None(May.ParseEnum<FlagsEnum>(value));
            Assert.None(May.ParseEnum<FlagsEnum>(value, ignoreCase: false));
            Assert.Some(FlagsEnum.OneTwo, May.ParseEnum<FlagsEnum>(value, ignoreCase: true));
        }

        [Theory]
        [InlineData("OneTwo")]
        [InlineData("onetwo")]
        [InlineData("onetWo")]
        public static void ParseEnum_Flags_InvalidName(string value)
        {
            Assert.None(May.ParseEnum<SimpleEnum>(value));
            Assert.None(May.ParseEnum<SimpleEnum>(value, ignoreCase: false));
            Assert.None(May.ParseEnum<SimpleEnum>(value, ignoreCase: true));
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
    }
}
