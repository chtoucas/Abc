// See LICENSE.txt in the project root for license information.

namespace Abc
{
    using System;
    using System.Globalization;

    // REVIEW: Nullable or Maybe?
    public static partial class MayParse { }

    // Parsers for simple value types.
    public partial class MayParse
    {
        private static readonly Maybe<bool> s_True = Maybe.Some(true);
        private static readonly Maybe<bool> s_False = Maybe.Some(false);

        public static Maybe<bool> ToBoolean(string? value)
            => ToBoolean(value, BooleanStyles.Default);

        public static Maybe<bool> ToBoolean(string? value, BooleanStyles style)
        {
            if (value is null) { return Maybe<bool>.None; }

            string val = value.Trim();

            if (val.Length == 0)
            {
                return style.Contains(BooleanStyles.EmptyOrWhiteSpaceIsFalse)
                    ? s_False : Maybe<bool>.None;
            }
            else if (style.Contains(BooleanStyles.Literal))
            {
                // NB: Cette méthode n'est pas sensible à la casse de "value".
                return Boolean.TryParse(val, out bool retval)
                    ? retval ? s_True : s_False
                    : Maybe<bool>.None;
            }
            else if (style.Contains(BooleanStyles.ZeroOrOne) && (val == "0" || val == "1"))
            {
                return val == "1" ? s_True : s_False;
            }
            else if (style.Contains(BooleanStyles.HtmlInput) && value == "on")
            {
                return s_True;
            }
            else
            {
                return Maybe<bool>.None;
            }
        }

        public static Maybe<decimal> ToDecimal(string? value)
            => ToDecimal(value, NumberStyles.Number, NumberFormatInfo.CurrentInfo);

        public static Maybe<decimal> ToDecimal(
            string? value, NumberStyles style, IFormatProvider? provider)
        {
            return Decimal.TryParse(value, style, provider, out decimal result)
                ? Maybe.Some(result) : Maybe<decimal>.None;
        }

        public static Maybe<double> ToDouble(string? value)
        {
            const NumberStyles DefaultStyle_ =
                NumberStyles.AllowLeadingWhite
                | NumberStyles.AllowTrailingWhite
                | NumberStyles.AllowLeadingSign
                | NumberStyles.AllowDecimalPoint
                | NumberStyles.AllowThousands
                | NumberStyles.AllowExponent;

            return ToDouble(value, DefaultStyle_, NumberFormatInfo.CurrentInfo);
        }

        public static Maybe<double> ToDouble(
            string? value, NumberStyles style, IFormatProvider? provider)
        {
            return Double.TryParse(value, style, provider, out double result)
                ? Maybe.Some(result) : Maybe<double>.None;
        }

        public static Maybe<short> ToInt16(string? value)
            => ToInt16(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo);

        public static Maybe<short> ToInt16(
            string? value, NumberStyles style, IFormatProvider? provider)
        {
            return Int16.TryParse(value, style, provider, out short result)
                ? Maybe.Some(result) : Maybe<short>.None;
        }

        public static Maybe<int> ToInt32(string? value)
            => ToInt32(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo);

        public static Maybe<int> ToInt32(
            string? value, NumberStyles style, IFormatProvider? provider)
        {
            return Int32.TryParse(value, style, provider, out int result)
                ? Maybe.Some(result) : Maybe<int>.None;
        }

        public static Maybe<long> ToInt64(string? value)
            => ToInt64(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo);

        public static Maybe<long> ToInt64(
            string? value, NumberStyles style, IFormatProvider? provider)
        {
            return Int64.TryParse(value, style, provider, out long result)
                ? Maybe.Some(result) : Maybe<long>.None;
        }

        public static Maybe<float> ToSingle(string? value)
            => ToSingle(value, NumberStyles.Number, NumberFormatInfo.CurrentInfo);

        public static Maybe<float> ToSingle(
            string? value, NumberStyles style, IFormatProvider? provider)
        {
            return Single.TryParse(value, style, provider, out float result)
                ? Maybe.Some(result) : Maybe<float>.None;
        }

        [CLSCompliant(false)]
        public static Maybe<sbyte> ToSByte(string? value)
            => ToSByte(value, NumberStyles.Number, NumberFormatInfo.CurrentInfo);

        [CLSCompliant(false)]
        public static Maybe<sbyte> ToSByte(
            string? value, NumberStyles style, IFormatProvider? provider)
        {
            return SByte.TryParse(value, style, provider, out sbyte result)
                ? Maybe.Some(result) : Maybe<sbyte>.None;
        }

        [CLSCompliant(false)]
        public static Maybe<byte> ToByte(string? value)
            => ToByte(value, NumberStyles.Number, NumberFormatInfo.CurrentInfo);

        [CLSCompliant(false)]
        public static Maybe<byte> ToByte(
            string? value, NumberStyles style, IFormatProvider? provider)
        {
            return Byte.TryParse(value, style, provider, out byte result)
                ? Maybe.Some(result) : Maybe<byte>.None;
        }

        [CLSCompliant(false)]
        public static Maybe<ushort> ToUInt16(string? value)
            => ToUInt16(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo);

        [CLSCompliant(false)]
        public static Maybe<ushort> ToUInt16(
            string? value, NumberStyles style, IFormatProvider? provider)
        {
            return UInt16.TryParse(value, style, provider, out ushort result)
                ? Maybe.Some(result) : Maybe<ushort>.None;
        }

        [CLSCompliant(false)]
        public static Maybe<uint> ToUInt32(string? value)
            => ToUInt32(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo);

        [CLSCompliant(false)]
        public static Maybe<uint> ToUInt32(
            string? value, NumberStyles style, IFormatProvider? provider)
        {
            return UInt32.TryParse(value, style, provider, out uint result)
                ? Maybe.Some(result) : Maybe<uint>.None;
        }

        [CLSCompliant(false)]
        public static Maybe<ulong> ToUInt64(string? value)
            => ToUInt64(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo);

        [CLSCompliant(false)]
        public static Maybe<ulong> ToUInt64(
            string? value, NumberStyles style, IFormatProvider? provider)
        {
            return UInt64.TryParse(value, style, provider, out ulong result)
                ? Maybe.Some(result) : Maybe<ulong>.None;
        }
    }

    // Parsers for value types that are not simple types.
    public partial class MayParse
    {
        public static Maybe<TEnum> ToEnum<TEnum>(string? value)
            where TEnum : struct
            => ToEnum<TEnum>(value, ignoreCase: true);

        // TODO: Explain that this method exhibits the same behaviour as Enum.TryParse,
        // in the sense that parsing any literal integer value will succeed even if
        // it is not a valid enumeration value.
        // See http://stackoverflow.com/questions/2191037/why-can-i-parse-invalid-values-to-an-enum-in-net
        public static Maybe<TEnum> ToEnum<TEnum>(string? value, bool ignoreCase)
            where TEnum : struct
            => Enum.TryParse(value, ignoreCase, out TEnum result)
                ? Maybe.Some(result) : Maybe<TEnum>.None;

        public static Maybe<DateTime> ToDateTime(string? value)
            => ToDateTime(value, "o");

        public static Maybe<DateTime> ToDateTime(string? value, string? format)
            => ToDateTime(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None);

        public static Maybe<DateTime> ToDateTime(
            string? value,
            string? format,
            IFormatProvider? provider,
            DateTimeStyles style)
        {
            return DateTime.TryParseExact(value, format, provider, style, out DateTime result)
                ? Maybe.Some(result) : Maybe<DateTime>.None;
        }
    }

    // Parsers for reference types.
    public partial class MayParse
    {
        public static Maybe<Uri> ToUri(string? value, UriKind uriKind)
            // REVIEW: Uri.TryCreate accepts empty strings.
            => Uri.TryCreate(value, uriKind, out Uri? uri)
                ? Maybe.Of(uri) : Maybe<Uri>.None;
    }
}
