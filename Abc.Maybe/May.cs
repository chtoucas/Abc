// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Provides static methods in the style of the May-Parse pattern.
    /// </summary>
    /// <remarks>
    /// Beware, for value types a nullable is often a better choice.
    /// Furthermore, internally this class merely wraps Try-Parse methods.
    /// </remarks>
    /// <example>
    /// Usage recommendation:
    /// <code><![CDATA[
    /// public sealed class MayEx : May
    /// {
    ///     public static Maybe<XXX> ParseXXX(string? value) { }
    /// }
    ///
    /// ...
    ///
    /// using May = MayEx;
    ///
    /// // One can call built-in methods.
    /// Maybe<int> n = May.ParseInt32("1");
    ///
    /// // But also locally defined methods.
    /// Maybe<XXX> xxx = May.ParseXXX("XXX");
    /// ]]></code>
    /// </example>
    public abstract partial class May
    {
        protected May() { }
    }

    // Parsers for simple value types.
    public partial class May
    {
        /// <summary>
        /// Attemps to convert the string representation of a number to its
        /// <see cref="Decimal"/> equivalent.
        /// </summary>
        public static Maybe<decimal> ParseDecimal(string? value)
        {
            return Decimal.TryParse(value, out decimal result)
                ? Maybe.Some(result) : Maybe<decimal>.None;
        }

        /// <summary>
        /// Attemps to convert the string representation of a number to its
        /// <see cref="Decimal"/> equivalent using the specified style and
        /// culture-specific format.
        /// </summary>
        public static Maybe<decimal> ParseDecimal(
            string? value, NumberStyles style, IFormatProvider? provider)
        {
            return Decimal.TryParse(value, style, provider, out decimal result)
                ? Maybe.Some(result) : Maybe<decimal>.None;
        }

        // TODO: overloads with ReadOnlySpan<>. PublicAPI?
#if NETSTANDARD2_1
        public static Maybe<decimal> ParseDecimal(ReadOnlySpan<char> span)
        {
            return Decimal.TryParse(span, out decimal result)
                ? Maybe.Some(result) : Maybe<decimal>.None;
        }
#endif

        /// <summary>
        /// Attemps to convert the string representation of a number to its
        /// <see cref="Double"/> equivalent.
        /// </summary>
        public static Maybe<double> ParseDouble(string? value)
        {
            return Double.TryParse(value, out double result)
                ? Maybe.Some(result) : Maybe<double>.None;
        }

        /// <summary>
        /// Attemps to convert the string representation of a number to its
        /// <see cref="Double"/> equivalent using the specified style and
        /// culture-specific format.
        /// </summary>
        public static Maybe<double> ParseDouble(
            string? value, NumberStyles style, IFormatProvider? provider)
        {
            return Double.TryParse(value, style, provider, out double result)
                ? Maybe.Some(result) : Maybe<double>.None;
        }

        /// <summary>
        /// Attemps to convert the string representation of a number to its
        /// <see cref="Int16"/> equivalent.
        /// </summary>
        public static Maybe<short> ParseInt16(string? value)
        {
            return Int16.TryParse(value, out short result)
                ? Maybe.Some(result) : Maybe<short>.None;
        }

        /// <summary>
        /// Attemps to convert the string representation of a number to its
        /// <see cref="Int16"/> equivalent using the specified style and
        /// culture-specific format.
        /// </summary>
        public static Maybe<short> ParseInt16(
            string? value, NumberStyles style, IFormatProvider? provider)
        {
            return Int16.TryParse(value, style, provider, out short result)
                ? Maybe.Some(result) : Maybe<short>.None;
        }

        /// <summary>
        /// Attemps to convert the string representation of a number to its
        /// <see cref="Int32"/> equivalent.
        /// </summary>
        public static Maybe<int> ParseInt32(string? value)
        {
            return Int32.TryParse(value, out int result)
                ? Maybe.Some(result) : Maybe<int>.None;
        }

        /// <summary>
        /// Attemps to convert the string representation of a number to its
        /// <see cref="Int32"/> equivalent using the specified style and
        /// culture-specific format.
        /// </summary>
        public static Maybe<int> ParseInt32(
            string? value, NumberStyles style, IFormatProvider? provider)
        {
            return Int32.TryParse(value, style, provider, out int result)
                ? Maybe.Some(result) : Maybe<int>.None;
        }

        /// <summary>
        /// Attemps to convert the string representation of a number to its
        /// <see cref="Int64"/> equivalent.
        /// </summary>
        public static Maybe<long> ParseInt64(string? value)
        {
            return Int64.TryParse(value, out long result)
                ? Maybe.Some(result) : Maybe<long>.None;
        }

        /// <summary>
        /// Attemps to convert the string representation of a number to its
        /// <see cref="Int64"/> equivalent using the specified style and
        /// culture-specific format.
        /// </summary>
        public static Maybe<long> ParseInt64(
            string? value, NumberStyles style, IFormatProvider? provider)
        {
            return Int64.TryParse(value, style, provider, out long result)
                ? Maybe.Some(result) : Maybe<long>.None;
        }

        /// <summary>
        /// Attemps to convert the string representation of a number to its
        /// <see cref="Single"/> equivalent.
        /// </summary>
        public static Maybe<float> ParseSingle(string? value)
        {
            return Single.TryParse(value, out float result)
                ? Maybe.Some(result) : Maybe<float>.None;
        }

        /// <summary>
        /// Attemps to convert the string representation of a number to its
        /// <see cref="Single"/> equivalent using the specified style and
        /// culture-specific format.
        /// </summary>
        public static Maybe<float> ParseSingle(
            string? value, NumberStyles style, IFormatProvider? provider)
        {
            return Single.TryParse(value, style, provider, out float result)
                ? Maybe.Some(result) : Maybe<float>.None;
        }

        /// <summary>
        /// Attemps to convert the string representation of a number to its
        /// <see cref="SByte"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        public static Maybe<sbyte> ParseSByte(string? value)
        {
            return SByte.TryParse(value, out sbyte result)
                ? Maybe.Some(result) : Maybe<sbyte>.None;
        }

        /// <summary>
        /// Attemps to convert the string representation of a number to its
        /// <see cref="SByte"/> equivalent using the specified style and
        /// culture-specific format.
        /// </summary>
        [CLSCompliant(false)]
        public static Maybe<sbyte> ParseSByte(
            string? value, NumberStyles style, IFormatProvider? provider)
        {
            return SByte.TryParse(value, style, provider, out sbyte result)
                ? Maybe.Some(result) : Maybe<sbyte>.None;
        }

        /// <summary>
        /// Attemps to convert the string representation of a number to its
        /// <see cref="Byte"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        public static Maybe<byte> ParseByte(string? value)
        {
            return Byte.TryParse(value, out byte result)
                ? Maybe.Some(result) : Maybe<byte>.None;
        }

        /// <summary>
        /// Attemps to convert the string representation of a number to its
        /// <see cref="Byte"/> equivalent using the specified style and
        /// culture-specific format.
        /// </summary>
        [CLSCompliant(false)]
        public static Maybe<byte> ParseByte(
            string? value, NumberStyles style, IFormatProvider? provider)
        {
            return Byte.TryParse(value, style, provider, out byte result)
                ? Maybe.Some(result) : Maybe<byte>.None;
        }

        /// <summary>
        /// Attemps to convert the string representation of a number to its
        /// <see cref="UInt16"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        public static Maybe<ushort> ParseUInt16(string? value)
        {
            return UInt16.TryParse(value, out ushort result)
                ? Maybe.Some(result) : Maybe<ushort>.None;
        }

        /// <summary>
        /// Attemps to convert the string representation of a number to its
        /// <see cref="UInt16"/> equivalent using the specified style and
        /// culture-specific format.
        /// </summary>
        [CLSCompliant(false)]
        public static Maybe<ushort> ParseUInt16(
            string? value, NumberStyles style, IFormatProvider? provider)
        {
            return UInt16.TryParse(value, style, provider, out ushort result)
                ? Maybe.Some(result) : Maybe<ushort>.None;
        }

        /// <summary>
        /// Attemps to convert the string representation of a number to its
        /// <see cref="UInt32"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        public static Maybe<uint> ParseUInt32(string? value)
        {
            return UInt32.TryParse(value, out uint result)
                ? Maybe.Some(result) : Maybe<uint>.None;
        }

        /// <summary>
        /// Attemps to convert the string representation of a number to its
        /// <see cref="UInt32"/> equivalent using the specified style and
        /// culture-specific format.
        /// </summary>
        [CLSCompliant(false)]
        public static Maybe<uint> ParseUInt32(
            string? value, NumberStyles style, IFormatProvider? provider)
        {
            return UInt32.TryParse(value, style, provider, out uint result)
                ? Maybe.Some(result) : Maybe<uint>.None;
        }

        /// <summary>
        /// Attemps to convert the string representation of a number to its
        /// <see cref="UInt64"/> equivalent.
        /// </summary>
        [CLSCompliant(false)]
        public static Maybe<ulong> ParseUInt64(string? value)
        {
            return UInt64.TryParse(value, out ulong result)
                ? Maybe.Some(result) : Maybe<ulong>.None;
        }

        /// <summary>
        /// Attemps to convert the string representation of a number to its
        /// <see cref="UInt64"/> equivalent using the specified style and
        /// culture-specific format.
        /// </summary>
        [CLSCompliant(false)]
        public static Maybe<ulong> ParseUInt64(
            string? value, NumberStyles style, IFormatProvider? provider)
        {
            return UInt64.TryParse(value, style, provider, out ulong result)
                ? Maybe.Some(result) : Maybe<ulong>.None;
        }
    }

    // Parsers for value types that are not simple types.
    public partial class May
    {
        public static Maybe<TEnum> ParseEnum<TEnum>(string? value)
            where TEnum : struct, Enum
            => ParseEnum<TEnum>(value, ignoreCase: true);

        /// <remarks>
        /// This method exhibits the same behaviour as Enum.TryParse, in the
        /// sense that parsing any literal integer value will succeed even if
        /// it is not a valid enumeration value.
        /// </remarks>
        public static Maybe<TEnum> ParseEnum<TEnum>(string? value, bool ignoreCase)
            where TEnum : struct, Enum
            => Enum.TryParse(value, ignoreCase, out TEnum result)
                ? Maybe.Some(result) : Maybe<TEnum>.None;

        // REVIEW: default format, format provider, Parse/ParseExact.

        public static Maybe<DateTime> ParseDateTimeExact(string? value)
            => ParseDateTimeExact(
                value, "o", DateTimeStyles.None, DateTimeFormatInfo.CurrentInfo);

        public static Maybe<DateTime> ParseDateTimeExact(string? value, string? format)
            => ParseDateTimeExact(
                value, format, DateTimeStyles.None, DateTimeFormatInfo.CurrentInfo);

        public static Maybe<DateTime> ParseDateTimeExact(
            string? value,
            string? format,
            DateTimeStyles style,
            IFormatProvider? provider)
        {
            return DateTime.TryParseExact(value, format, provider, style, out DateTime result)
                ? Maybe.Some(result) : Maybe<DateTime>.None;
        }
    }

    // Parsers for reference types.
    public partial class May
    {
        /// <summary>
        /// Attemps to create a new <see cref="Uri"/> using the specified
        /// string instance and <see cref="UriKind"/>.
        /// </summary>
        public static Maybe<Uri> ParseUri(string? value, UriKind uriKind)
        {
            Uri.TryCreate(value, uriKind, out Uri? uri);
            return Maybe.SomeOrNone(uri);
        }
    }
}
