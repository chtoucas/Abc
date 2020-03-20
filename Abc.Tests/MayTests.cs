// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;

    using Xunit;

    using static global::My;

    using Assert = AssertEx;

    public static class MayTests
    {
        #region ParseEnum()

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
            Assert.Some(My.Enum012.One, May.ParseEnum<Enum012>(value));
            Assert.Some(My.Enum012.One, May.ParseEnum<Enum012>(value, ignoreCase: false));
            Assert.Some(My.Enum012.One, May.ParseEnum<Enum012>(value, ignoreCase: true));
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
            Assert.None(May.ParseEnum<Enum012>(value));
            Assert.None(May.ParseEnum<Enum012>(value, ignoreCase: false));
            Assert.Some(My.Enum012.One, May.ParseEnum<Enum012>(value, ignoreCase: true));
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
            Assert.Some((Enum012)4, May.ParseEnum<Enum012>(value));
            Assert.Some((Enum012)4, May.ParseEnum<Enum012>(value, ignoreCase: false));
            Assert.Some((Enum012)4, May.ParseEnum<Enum012>(value, ignoreCase: true));
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
            Assert.None(May.ParseEnum<Enum012>(value));
            Assert.None(May.ParseEnum<Enum012>(value, ignoreCase: false));
            Assert.None(May.ParseEnum<Enum012>(value, ignoreCase: true));
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
            Assert.Some(My.EnumBits.OneTwo, May.ParseEnum<EnumBits>(value));
            Assert.Some(My.EnumBits.OneTwo, May.ParseEnum<EnumBits>(value, ignoreCase: false));
            Assert.Some(My.EnumBits.OneTwo, May.ParseEnum<EnumBits>(value, ignoreCase: true));
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
            Assert.None(May.ParseEnum<EnumBits>(value));
            Assert.None(May.ParseEnum<EnumBits>(value, ignoreCase: false));
            Assert.Some(My.EnumBits.OneTwo, May.ParseEnum<EnumBits>(value, ignoreCase: true));
        }

        [Theory]
        [InlineData("OneTwo")]
        [InlineData("onetwo")]
        [InlineData("onetWo")]
        public static void ParseEnum_Flags_InvalidName(string value)
        {
            Assert.None(May.ParseEnum<Enum012>(value));
            Assert.None(May.ParseEnum<Enum012>(value, ignoreCase: false));
            Assert.None(May.ParseEnum<Enum012>(value, ignoreCase: true));
        }

        #endregion

    }
}
