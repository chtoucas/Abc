// See LICENSE.txt in the project root for license information.

//#define PLAIN_DAYOFWEEK

// We restrict dates to years in the range from 1 to 9999, nevertheless all
// formulae give a meaningful result even for years less than 0, but only if we
// replace the division (/) and modulo (%) operations by the genuine mathematical
// ones --- the right-shift's and left-shift's do not need such adjustments;
// see Utilities.MathEx. It also does not make sense to go beyond the 9999 limit.
// By the time we reach that point in time (and if we still use the Gregorian
// calendar) the leap rule will certainly have changed long time ago just to
// stay in sync with the solar year.
//
// We choose to represent a date by a triple (year, month, day), not by a count
// of days since a fixed date. This is the most efficient and most natural
// choice, but this means that sometimes we have to compute the property
// DaysSinceEpoch on-the-fly. This is mostly the case for things related to the
// day of the week or when adding days or subtracting two dates.
// DateTime from the BCL use the opposite strategy. It makes perfectly sense
// since internally a DateTime is always of Gregorian type, and support for any
// other calendar is done by interconversion.
//
// References:
// - DateTime from the BCL
// - [NodaTime](https://github.com/nodatime/nodatime)
// - [chrono](https://github.com/HowardHinnant/date) (C++ library)
// - [Doomsday algorithm](http://rudy.ca/doomsday.html)
//   and [Doomsday rule](https://en.wikipedia.org/wiki/Doomsday_rule)

namespace Abc
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Runtime.CompilerServices;

    using static Abc.Utilities.MathEx;

    using AoorException = System.ArgumentOutOfRangeException;
    using EF = Utilities.ExceptionFactory;

    /// <summary>
    /// Represents a date within the Gregorian calendar.
    /// <para><see cref="GregorianDate"/> is an immutable struct.</para>
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public readonly partial struct GregorianDate
        : IEquatable<GregorianDate>, IComparable<GregorianDate>, IComparable
    {
        /// <summary>
        /// Represents the number of days in a 400-year cycle.
        /// <para>This field is a constant equal to 146_097‬.</para>
        /// </summary>
        /// <remarks>
        /// On average, a year is 365.2425 days long.
        /// </remarks>
        private const int DaysPer400Years = 400 * 365 + 97;

        /// <summary>
        /// Represents the number of days from march to december, both
        /// included.
        /// <para>This field is constant.</para>
        /// </summary>
        private const int DaysFromMarchToDecember = 306;

        /// <summary>
        /// Represents the smallest possible value of the number of consecutive
        /// days since the epoch of the Gregorian calendar.
        /// <para>This field is a constant equal to 0.</para>
        /// </summary>
        public const int MinDaysSinceEpoch = 0;

        /// <summary>
        /// Represents the largest possible value of the number of consecutive
        /// days since the epoch of the Gregorian calendar.
        /// <para>This field is a constant equal to 3_652_058.</para>
        /// </summary>
        public const int MaxDaysSinceEpoch = 3_652_058;

        /// <summary>
        /// Represents the earliest supported year.
        /// <para>This field is a constant equal to 1.</para>
        /// </summary>
        public const int MinSupportedYear = 1;

        /// <summary>
        /// Represents the latest supported year.
        /// <para>This field is a constant equal to 9999.</para>
        /// </summary>
        public const int MaxSupportedYear = 9999;

        /// <summary>
        /// Gets the earliest date supported by the <see cref="GregorianDate"/>
        /// type: monday January 1st, 1 CE.
        /// <para>This field is read-only.</para>
        /// </summary>
        public static readonly GregorianDate MinValue = new GregorianDate(1, 1, 1);

        /// <summary>
        /// Gets the latest date supported by the <see cref="GregorianDate"/>
        /// type: December 31, 9999 CE.
        /// <para>This field is read-only.</para>
        /// </summary>
        public static readonly GregorianDate MaxValue
            = new GregorianDate(MaxSupportedYear, 12, 31);

        /// <summary>
        /// Represents the binary data stored in the current instance.
        /// </summary>
        private readonly int _bin;

        /// <summary>
        /// Initializes a new instance of the <see cref="GregorianDate"/> struct
        /// from the specified year, month and day.
        /// </summary>
        /// <exception cref="AoorException">The specified date parts do not form
        /// a valid Gregorian date within the Common Era on or before year 9999.
        /// </exception>
        public GregorianDate(int year, int month, int day)
        {
            if (year < MinSupportedYear || year > MaxSupportedYear)
            {
                throw new AoorException(nameof(year));
            }
            if (month < 1 || month > 12)
            {
                throw new AoorException(nameof(month));
            }
            // In the vast majority of cases, we can avoid the computation of
            // the exact number of days in the month.
            if (day < 1 || (day > 28 && day > CountDaysInMonth(year, month)))
            {
                throw new AoorException(nameof(day));
            }

            _bin = Pack(year, month, day);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GregorianDate"/> struct
        /// directly from the specified binary data.
        /// <para>This constructor does NOT validate its parameter.</para>
        /// <seealso cref="Pack(int, int, int)"/>
        /// </summary>
        private GregorianDate(int bin)
        {
            __VerifyBinaryData(bin);
            _bin = bin;
        }

        /// <summary>
        /// Creates a new instance of <see cref="GregorianDate"/> from the
        /// specified year, month and day.
        /// <para>This method does NOT validate its parameters.</para>
        /// </summary>
        [Pure]
        internal static GregorianDate CreateLenient(int y, int m, int d)
            => new GregorianDate(Pack(y, m, d));

        /// <summary>
        /// Gets the current (local) date on this computer.
        /// </summary>
        public static GregorianDate Today
        {
            get
            {
                var now = DateTime.Now;
                return CreateLenient(now.Year, now.Month, now.Day);
            }
        }

        public int Century
        {
            get
            {
                int q = Year / 100;
                return Year < 0 || Year % 100 == 0 ? q : q + 1;
            }
        }

        public int YearOfCentury
        {
            get
            {
                // AdjustedModulo(Year, 100);
                int r0 = Year % 100;
                int mod = r0 >= 0 ? r0 : (r0 + 100);
                return mod == 0 ? 100 : mod;
            }
        }

        /// <para>The result is in the range from 1 to 9999.</para>
        public int Year => _bin >> 9;

        /// <para>The result is in the range from 1 to 12.</para>
        public int Month => (_bin >> 5) & MonthMask;

        /// <para>The result is in the range from 1 to 366.</para>
        public int DayOfYear
        {
            get
            {
                Unpack(out int y, out int m, out int d);
                return CountDaysInYearBeforeMonth(y, m) + d;
            }
        }

        /// <para>The result is in the range from 1 to 31.</para>
        public int Day => _bin & DayMask;

        public DayOfWeek DayOfWeek
        {
            get
            {
#if PLAIN_DAYOFWEEK
                // The epoch of the Gregorian calendar is a Monday.
                return (DayOfWeek)(((int)DayOfWeek.Monday + DaysSinceEpoch) % 7);
#else
                Unpack(out int y, out int m, out int d);
                int doomsday = GetDoomsday(y, m);
                return (DayOfWeek)((doomsday + d) % 7);
#endif
            }
        }

        /// <summary>
        /// Gets the ISO weekday number.
        /// <para>Returns an integer in the range from 1 to 7, 1 being
        /// attributed to Monday.</para>
        /// </summary>
        public int IsoWeekday
        {
            get
            {
#if PLAIN_DAYOFWEEK
                return AdjustedModulo((int)DayOfWeek.Monday + DaysSinceEpoch, 7);
#else
                Unpack(out int y, out int m, out int d);
                int doomsday = GetDoomsday(y, m);
                return AdjustedModulo(doomsday + d, 7);
#endif
            }
        }

        /// <summary>
        /// Gets the week of the year.
        /// <para>This is NOT the ISO week number.</para>
        /// </summary>
        /// <remarks>
        /// <para>A week starts on Monday and weeks at both ends of a year may
        /// be incomplete (less than 7 days, even a one-day week is OK).</para>
        /// <para>In the vast majority of cases, we count 52 whole weeks in a
        /// year and one incomplete week.</para>
        /// <para>Long years are 54 weeks long (52 whole weeks and 2 one-day
        /// weeks), something that can only happen with a leap year starting on
        /// a Sunday --- long years always end on a Monday.</para>
        /// </remarks>
        public int WeekOfYear
        {
            get
            {
                int weekday = GetIsoWeekdayAtStartOfYear(Year);
                return (DayOfYear + 5 + weekday) / 7;
            }
        }

        public bool IsIntercalary => ObMonthDay == __IntercalaryDay;

        /// <summary>
        /// Gets the year-month part of the binary data.
        /// </summary>
        private int ObYearMonth => _bin >> 5;

        /// <summary>
        /// Gets the month-day part of the binary data.
        /// </summary>
        private int ObMonthDay => _bin & ((1 << 9) - 1);

        /// <summary>
        /// Gets the number of consecutive days since the epoch of the Gregorian
        /// calendar.
        /// <para>The result is in the range from <see cref="MinDaysSinceEpoch"/>
        /// to <see cref="MaxDaysSinceEpoch"/>.</para>
        /// </summary>
        public int DaysSinceEpoch
        {
            get
            {
                Unpack(out int y, out int m, out int d);

                // We pretend that the first month of the year is March
                // (numbering starting at zero, not one).
                if (m < 3)
                {
                    y--;
                    m += 9;
                }
                else
                {
                    m -= 3;
                }

                int C = y / 100;
                int Y = y % 100;

                return -DaysFromMarchToDecember + (DaysPer400Years * C >> 2)
                    + (1461 * Y >> 2) + (153 * m + 2) / 5 + d - 1;
            }
        }

        /// <summary>
        /// Gets a string representation of the binary data stored in the current
        /// instance.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        private string DebuggerDisplay => Convert.ToString(_bin, 2);

        /// <summary>
        /// Returns a culture-independent string representation of the current
        /// instance.
        /// <para>The result is compatible with the (extended) ISO 8601 format.
        /// </para>
        /// </summary>
        [Pure]
        public override string ToString()
        {
            Unpack(out int y, out int m, out int d);
            return FormattableString.Invariant($"{y:D4}-{m:D2}-{d:D2}");
        }

        /// <summary>
        /// Deconstructs the current instance into its components.
        /// </summary>
        public void Deconstruct(out int year, out int month, out int day)
            => Unpack(out year, out month, out day);

        /// <summary>
        /// Creates a new instance of <see cref="GregorianDate"/> from the
        /// specified ordinal date.
        /// </summary>
        /// <exception cref="AoorException">The specified ordinal date parts do
        /// not form a valid Gregorian date within the Common Era on or before
        /// year 9999.</exception>
        [Pure]
        public static GregorianDate FromOrdinalDate(int year, int dayOfYear)
        {
            if (year < MinSupportedYear || year > MaxSupportedYear)
            {
                throw new AoorException(nameof(year));
            }
            if (dayOfYear < 1
                || (dayOfYear > 365 && dayOfYear > CountDaysInYear(year)))
            {
                throw new AoorException(nameof(dayOfYear));
            }

            return FromOrdinalDateImpl(year, dayOfYear);
        }

        /// <summary>
        /// Creates a new instance of <see cref="GregorianDate"/> from the
        /// specified number of consecutive days since the epoch of the
        /// Gregorian calendar.
        /// <para>This method does NOT validate its parameter.</para>
        /// </summary>
        [Pure]
        public static GregorianDate FromDaysSinceEpoch(int daysSinceEpoch)
        {
            daysSinceEpoch += DaysFromMarchToDecember;

            int C = ((daysSinceEpoch << 2) + 3) / DaysPer400Years;
            int D = daysSinceEpoch - (DaysPer400Years * C >> 2);
            int Y = ((D << 2) + 3) / 1461;

            int d0y = D - (1461 * Y >> 2);

            int m = (5 * d0y + 2) / 153;
            int d = 1 + d0y - (153 * m + 2) / 5;

            if (m > 9)
            {
                Y++;
                m -= 9;
            }
            else
            {
                m += 3;
            }

            return CreateLenient(100 * C + Y, m, d);
        }

        /// <summary>
        /// Creates a new instance of <see cref="GregorianDate"/> from the
        /// specified ordinal date.
        /// <para>This method does NOT validate its parameter.</para>
        /// </summary>
        [Pure]
        private static GregorianDate FromOrdinalDateImpl(int y, int doy)
        {
            if (doy < 60)
            {
                doy--;
                return CreateLenient(y, 1 + doy / 31, 1 + doy % 31);
            }
            else if (doy == 60)
            {
                int bin = (y << 9)
                    | (IsLeapYear(y) ? __IntercalaryDay : __StartOfMarch);
                return new GregorianDate(bin);
            }
            else
            {
                doy -= IsLeapYear(y) ? 61 : 60;

                int m = (5 * doy + 2) / 153;
                int d = 1 + doy - (153 * m + 2) / 5;

                return CreateLenient(y, m + 3, d);
            }
        }

        /// <summary>
        /// Obtains the ISO weekday of the first day of the specified year.
        /// </summary>
        [Pure]
        private static int GetIsoWeekdayAtStartOfYear(int y)
        {
#if PLAIN_DAYOFWEEK
            var startOfYear = new GregorianDate((y << 9) | __StartOfYear);
            return startOfYear.IsoWeekday;
#else
            // Calculation of IsoWeekday with m = d = 1.
            y--;
            int c = y / 100;
            return AdjustedModulo(1 + y + (y >> 2) - c + (c >> 2), 7);
#endif
        }
    }

    // Helpers.
    public partial struct GregorianDate
    {
        /// <summary>
        /// Checks whether the specified year is leap or not.
        /// <para>This method does NOT check whether the year is in a specific
        /// range or not.</para>
        /// </summary>
        [Pure]
        // Code size = 26 bytes.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsLeapYear(int y)
            // year is a multiple of 4 and, if it is a centennial year, only keep
            // those multiple of 400; a centennial year is common if it is not a
            // multiple of 400.
            => (y & 3) == 0 && (y % 100 != 0 || y % 400 == 0);

        /// <summary>
        /// Obtains the number of days in the specified year.
        /// <para>This method does NOT check whether the year is in a specific
        /// range or not.</para>
        /// </summary>
        [Pure]
        private static int CountDaysInYear(int y)
            => IsLeapYear(y) ? 366 : 365;

        /// <summary>
        /// Obtains the number of days before the start of the specified
        /// month.
        /// <para>This method does NOT check whether the year is in a specific
        /// range or not.</para>
        /// <para>This method does NOT validate the month parameter.</para>
        /// </summary>
        [Pure]
        private static int CountDaysInYearBeforeMonth(int y, int m)
            // The "plain" formula is given by:
            //   days = (153 * m + 2) / 5
            // corrected for m > 2:
            //   days + (IsLeapYear(y) ? 60 : 59);
            => m < 3 ? 31 * (m - 1)
                : IsLeapYear(y) ? (153 * m - 157) / 5
                : (153 * m - 162) / 5;

        /// <summary>
        /// Obtains the number of days in the specified month.
        /// <para>This method does NOT check whether the year is in a specific
        /// range or not.</para>
        /// <para>This method does NOT validate the month parameter.</para>
        /// </summary>
        [Pure]
        private static int CountDaysInMonth(int y, int m)
            // Trick:
            // - month < 8,  30 if even, 31 if odd
            // - month >= 8, 31 if even, 30 if odd
            // divide by 8 (m >> 3) = 0 (m < 8) or 1 (m >= 8), add m, and
            // finally check the parity (p & 1) = 0 if even, 1 if odd.
            => m != 2 ? 30 + ((m + (m >> 3)) & 1)
                : IsLeapYear(y) ? 29
                : 28;

        [Pure]
        private static int GetDoomsday(int y, int m)
        {
            // Doomsday rule, adapted by Keith & Craver.
            //
            // Algorithm by Conway:
            //   given a reference doomsday in the year:
            //     Twosday + y + (y >> 2) - c + (c >> 2)
            //   where c = y/100 and Twosday = 2!
            //   given a reference doomsday in the month:
            //     3, 28, 0, 4, 9, 6, 11, 8, 5, 10, 7, 12 (common year)
            //     4, 29, 0, 4, 9, 6, 11, 8, 5, 10, 7, 12 (leap year)
            //   the day of the week is (sunday = 0):
            //     (doomsday-in-year + d - doomsday-in-month) % 7

            int α;
            if (m < 3)
            {
                y--;
                α = 23 * m / 9 - 2;
            }
            else
            {
                α = 23 * m / 9 + 2;
            }

            int c = Divide(y, 100);

            return α + y + (y >> 2) - c + (c >> 2);
        }
    }

    // Binary data helpers.
    public partial struct GregorianDate
    {
        private const int MonthMask = (1 << 4) - 1;
        private const int DayMask = (1 << 5) - 1;

        // Sentinels: binary data for selected values of (month, day).
        private const int __EndOfFebruary = (2 << 5) | 28;
        private const int __IntercalaryDay = (2 << 5) | 29;
        private const int __StartOfMarch = (3 << 5) | 1;
        private const int __StartOfYear = (1 << 5) | 1;
        private const int __EndOfYear = (12 << 5) | 31;

        /// <summary>
        /// Deserializes a 32-bit binary value and recreates the original
        /// serialized <see cref="GregorianDate"/> object.
        /// </summary>
        [Pure]
        public static GregorianDate FromBinary(int bin)
        {
            ValidateBinaryData(bin);
            return new GregorianDate(bin);
        }

        /// <summary>
        /// Serializes the current instance to a 32-bit binary value that
        /// subsequently can be used to recreate the <see cref="GregorianDate"/>
        /// object using <see cref="FromBinary(int)"/>.
        /// </summary>
        [Pure]
        public int ToBinary() => _bin;

        /// <summary>
        /// Validates the specified binary data.
        /// </summary>
        /// <exception cref="ArgumentException">The specified binary data is
        /// not well-formed or invalid.</exception>
        private static void ValidateBinaryData(int bin)
        {
            int h = bin >> 24;
            if (h != 0)
            {
                // The highest byte is always zero.
                throw EF.InvalidBinaryInput(nameof(bin));
            }

            int y = bin >> 9;
            if (y < MinSupportedYear || y > MaxSupportedYear)
            {
                throw EF.InvalidBinaryInput(nameof(bin));
            }

            int m = (bin >> 5) & MonthMask;
            if (m < 1 || m > 12)
            {
                throw EF.InvalidBinaryInput(nameof(bin));
            }

            int d = bin & DayMask;
            if (d < 1 || (d > 28 && d > CountDaysInMonth(y, m)))
            {
                throw EF.InvalidBinaryInput(nameof(bin));
            }
        }

        /// <summary>
        /// Packs the specified date parts.
        /// </summary>
        [Pure]
        // Code size = 11 bytes.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Pack(int y, int m, int d)
            // Internal representation (from left to right):
            //   year  23-bit signed integer
            //   month  4-bit unsigned integer
            //   day    5-bit unsigned integer
            // At worst the year will only fill 15 bits out of the 23 possible,
            // which leaves one blank byte.
            => (y << 9) | (m << 5) | d;

        /// <summary>
        /// Unpacks the binary data.
        /// </summary>
        // Code size = 28 bytes.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Unpack(out int y, out int m, out int d)
        {
            // Warning, if we do not copy _bin locally, the method won't be
            // inlined for sure (code size = 36 bytes > 32 bytes).
            int bin = _bin;

            y = bin >> 9;
            m = (bin >> 5) & MonthMask;
            d = bin & DayMask;
        }

        [Conditional("DEBUG")]
        [ExcludeFromCodeCoverage]
        private static void __VerifyBinaryData(int bin)
            => ValidateBinaryData(bin);
    }

    // Conversions, adjustments...
    public partial struct GregorianDate
    {
        /// <summary>
        /// Creates a new instance of <see cref="GregorianDate"/> from the
        /// specified <see cref="DateTime"/> object.
        /// </summary>
        [Pure]
        public static GregorianDate FromDateTime(DateTime date)
           => new GregorianDate(date.Year, date.Month, date.Day);

        /// <summary>
        /// Converts the current instance to a <see cref="DateTime"/> object.
        /// </summary>
        [Pure]
        public DateTime ToDateTime()
        {
            Unpack(out int y, out int m, out int d);
            return new DateTime(y, m, d);
        }

        [Pure]
        public int CountRemainingDaysInYear()
        {
            Unpack(out int y, out int m, out int d);
            return CountDaysInYear(y) - CountDaysInYearBeforeMonth(y, m) - d;
        }

        [Pure]
        public int CountRemainingDaysInMonth()
        {
            Unpack(out int y, out int m, out int d);
            return CountDaysInMonth(y, m) - d;
        }

        #region Year and month boundaries.

        [Pure]
        public GregorianDate GetStartOfYear()
            => new GregorianDate((Year << 9) | __StartOfYear);

        [Pure]
        public GregorianDate GetEndOfYear()
            => new GregorianDate((Year << 9) | __EndOfYear);

        [Pure]
        public GregorianDate GetStartOfMonth()
        {
            Unpack(out int y, out int m, out _);
            return CreateLenient(y, m, 1);
        }

        [Pure]
        public GregorianDate GetEndOfMonth()
        {
            Unpack(out int y, out int m, out _);
            int d = CountDaysInMonth(y, m);
            return CreateLenient(y, m, d);
        }

        #endregion

        #region Adjust a single field.

        /// <summary>
        /// Adjusts the year field of the specified date to the specified value,
        /// yielding a new date.
        /// </summary>
        /// <exception cref="AoorException">The method would create an invalid
        /// Gregorian date within the Common Era on or before year 9999.
        /// </exception>
        [Pure]
        public GregorianDate AdjustYear(int newYear)
        {
            if (newYear < MinSupportedYear || newYear > MaxSupportedYear)
            {
                throw new AoorException(nameof(newYear));
            }

            int bMD = ObMonthDay;
            if (bMD == __IntercalaryDay && !IsLeapYear(newYear))
            {
                throw new AoorException(nameof(newYear));
            }

            return new GregorianDate((newYear << 9) | bMD);
        }

        /// <summary>
        /// Adjusts the month field of the specified date to the specified value,
        /// yielding a new date.
        /// </summary>
        /// <exception cref="AoorException">The method would create an invalid
        /// Gregorian date within the Common Era on or before year 9999.
        /// </exception>
        [Pure]
        public GregorianDate AdjustMonth(int newMonth)
        {
            if (newMonth < 1 || newMonth > 12)
            {
                throw new AoorException(nameof(newMonth));
            }

            int y = Year;
            int d = Day;

            if (d > CountDaysInMonth(y, newMonth))
            {
                throw new AoorException(nameof(newMonth));
            }

            return CreateLenient(y, newMonth, d);
        }

        /// <summary>
        /// Adjusts the day field of the specified date to the specified value,
        /// yielding a new date.
        /// </summary>
        /// <exception cref="AoorException">The method would create an invalid
        /// Gregorian date within the Common Era on or before year 9999.
        /// </exception>
        [Pure]
        public GregorianDate AdjustDay(int newDay)
        {
            if (newDay < 1)
            {
                throw new AoorException(nameof(newDay));
            }

            int y = Year;
            int m = Month;

            if (newDay > CountDaysInMonth(y, m))
            {
                throw new AoorException(nameof(newDay));
            }

            return CreateLenient(y, m, newDay);
        }

        #endregion

        #region Adjust the day of the week.

        // With PLAIN_DAYOFWEEK, we do not use the math op Plus()
        //   first we must compute DaysSinceEpoch, shift it, then convert the
        //   result back to a (y, m, d).
        // Without PLAIN_DAYOFWEEK (default),
        //   we only need DayOfWeek which, thanks to the Doosmday rule, is
        //   slightly faster to compute than DaysSinceEpoch, then use Plus()
        //   which is rather optimised for this type of situation.
        //
        // To compute Previous(dayOfWeek), we use NextOrSame(dayOfWeek, -7),
        // not PreviousOrSame(dayOfWeek, -1). This seems odd, but it does so
        // to avoid problems near MinValue. The same goes with the other methods
        //   PreviousOrSame(dayOWeek) = NextOrSame(dayOfWeek, -6)
        //     PreviousOrSame(dayOfWeek, 0) fails near MinValue
        //   Nearest(dayOfWeek)
        //     PreviousOrSame(dayOfWeek, 3) fails near MinValue and MaxValue
        //   NextOrSame(dayOWeek) = PreviousOrSame(dayOfWeek, 6)
        //     NextOrSame(dayOfWeek, 0) fails near MaxValue
        //   PreviousOrSame(dayOWeek) = PreviousOrSame(dayOfWeek, 7)
        //     NextOrSame(dayOfWeek, 1) fails near MaxValue

        [Pure]
        public GregorianDate Previous(DayOfWeek dayOfWeek)
#if PLAIN_DAYOFWEEK
            => NextOrSame(dayOfWeek, -7);
#else
        {
            if (dayOfWeek < DayOfWeek.Sunday || dayOfWeek > DayOfWeek.Saturday)
            {
                throw new AoorException(nameof(dayOfWeek));
            }

            int δ = dayOfWeek - DayOfWeek;
            return PlusDaysFast(δ >= 0 ? δ - 7 : δ);
        }
#endif

        [Pure]
        public GregorianDate PreviousOrSame(DayOfWeek dayOfWeek)
#if PLAIN_DAYOFWEEK
            => NextOrSame(dayOfWeek, -6);
#else
        {
            if (dayOfWeek < DayOfWeek.Sunday || dayOfWeek > DayOfWeek.Saturday)
            {
                throw new AoorException(nameof(dayOfWeek));
            }

            int δ = dayOfWeek - DayOfWeek;
            return δ == 0 ? this : PlusDaysFast(δ > 0 ? δ - 7 : δ);
        }
#endif

        [Pure]
        public GregorianDate NextOrSame(DayOfWeek dayOfWeek)
#if PLAIN_DAYOFWEEK
            => PreviousOrSame(dayOfWeek, 6);
#else
        {
            if (dayOfWeek < DayOfWeek.Sunday || dayOfWeek > DayOfWeek.Saturday)
            {
                throw new AoorException(nameof(dayOfWeek));
            }

            int δ = dayOfWeek - DayOfWeek;
            return δ == 0 ? this : PlusDaysFast(δ < 0 ? δ + 7 : δ);
        }
#endif

        [Pure]
        public GregorianDate Next(DayOfWeek dayOfWeek)
#if PLAIN_DAYOFWEEK
            => PreviousOrSame(dayOfWeek, 7);
#else
        {
            if (dayOfWeek < DayOfWeek.Sunday || dayOfWeek > DayOfWeek.Saturday)
            {
                throw new AoorException(nameof(dayOfWeek));
            }

            int δ = dayOfWeek - DayOfWeek;
            return PlusDaysFast(δ <= 0 ? δ + 7 : δ);
        }
#endif

#if PLAIN_DAYOFWEEK
        [Pure]
        private GregorianDate PreviousOrSame(DayOfWeek dayOfWeek, int dayShift)
        {
            // This is necessary to avoid unattended underflows.
            Debug.Assert(dayShift >= 6);

            if (dayOfWeek < DayOfWeek.Sunday || dayOfWeek > DayOfWeek.Saturday)
            {
                throw new AoorException(nameof(dayOfWeek));
            }

            int daysSinceEpoch = DaysSinceEpoch + dayShift;
            // The next line works because the "numerator" is >= 0.
            daysSinceEpoch -= (daysSinceEpoch + (DayOfWeek.Monday - dayOfWeek)) % 7;
            if (daysSinceEpoch > MaxDaysSinceEpoch)
            {
                // NB: this method find a "previous or same" but we actually use
                // it to find a "next or same"...
                throw EF.DayNumberOverflow;
            }

            return FromDaysSinceEpoch(daysSinceEpoch);
        }

        [Pure]
        private GregorianDate NextOrSame(DayOfWeek dayOfWeek, int dayShift)
        {
            // This is necessary to avoid unattended overflows.
            Debug.Assert(dayShift <= -6);

            if (dayOfWeek < DayOfWeek.Sunday || dayOfWeek > DayOfWeek.Saturday)
            {
                throw new AoorException(nameof(dayOfWeek));
            }

            int daysSinceEpoch = DaysSinceEpoch + dayShift;
            daysSinceEpoch += Modulo(-daysSinceEpoch + (dayOfWeek - DayOfWeek.Monday), 7);
            if (daysSinceEpoch < MinDaysSinceEpoch)
            {
                // NB: this method find a "next or same" but we actually use it
                // to find a "previous or same"...
                throw EF.DayNumberUnderflow;
            }

            return FromDaysSinceEpoch(daysSinceEpoch);
        }
#endif

        #endregion
    }

    // Enumerate days in a month or a year.
    public partial struct GregorianDate
    {
        /// <summary>
        /// Obtains the collection of all dates in the specified year.
        /// </summary>
        /// <exception cref="AoorException">The specified year is not in the
        /// range from 1 to 9999.</exception>
        [Pure]
        public static IEnumerable<GregorianDate> GetDaysInYear(int year)
        {
            // Check arg eagerly.
            if (year < MinSupportedYear || year > MaxSupportedYear)
            {
                throw new AoorException(nameof(year));
            }

            return __iterator();

            IEnumerable<GregorianDate> __iterator()
            {
                int bY = year << 9;

                for (int m = 1; m <= 12; m++)
                {
                    int bYM = bY | (m << 5);

                    int daysInMonth = CountDaysInMonth(year, m);
                    for (int d = 1; d <= daysInMonth; d++)
                    {
                        yield return new GregorianDate(bYM | d);
                    }
                }
            }
        }

        /// <summary>
        /// Obtains the collection of all dates in the specified month.
        /// </summary>
        /// <exception cref="AoorException">The specified year is not in the
        /// range from 1 to 9999.</exception>
        /// <exception cref="AoorException">The specified month is not in the
        /// range from 1 to 12.</exception>
        [Pure]
        public static IEnumerable<GregorianDate> GetDaysInMonth(int year, int month)
        {
            // Check args eagerly.
            if (year < MinSupportedYear || year > MaxSupportedYear)
            {
                throw new AoorException(nameof(year));
            }
            if (month < 1 || month > 12)
            {
                throw new AoorException(nameof(month));
            }

            return __iterator();

            IEnumerable<GregorianDate> __iterator()
            {
                int bYM = (year << 9) | (month << 5);

                int daysInMonth = CountDaysInMonth(year, month);
                for (int d = 1; d <= daysInMonth; d++)
                {
                    yield return new GregorianDate(bYM | d);
                }
            }
        }
    }

    // Interface IEquatable<>.
    public partial struct GregorianDate
    {
        /// <summary>
        /// Determines whether two specified instances of <see cref="GregorianDate"/>
        /// are equal.
        /// </summary>
        public static bool operator ==(GregorianDate left, GregorianDate right)
            => left._bin == right._bin;

        /// <summary>
        /// Determines whether two specified instances of <see cref="GregorianDate"/>
        /// are not equal.
        /// </summary>
        public static bool operator !=(GregorianDate left, GregorianDate right)
            => left._bin != right._bin;

        /// <summary>
        /// Determines whether this instance is equal to the value of the
        /// specified <see cref="GregorianDate"/>.
        /// </summary>
        [Pure]
        public bool Equals(GregorianDate other)
            => _bin == other._bin;

        /// <summary>
        /// Determines whether this instance is equal to a specified object.
        /// </summary>
        [Pure]
        public override bool Equals(object? obj)
            => obj is GregorianDate date && _bin == date._bin;

        /// <summary>
        /// Obtains the hash code for this instance.
        /// </summary>
        [Pure]
        public override int GetHashCode()
            => _bin;
    }

    // Interface IComparable<>.
    public partial struct GregorianDate
    {
        /// <summary>
        /// Compares the two specified date instances to see if the left one is
        /// strictly earlier than the right one.
        /// </summary>
        public static bool operator <(GregorianDate left, GregorianDate right)
            => left._bin < right._bin;

        /// <summary>
        /// Compares the two specified date instances to see if the left one is
        /// earlier than or equal to the right one.
        /// </summary>
        public static bool operator <=(GregorianDate left, GregorianDate right)
            => left._bin <= right._bin;

        /// <summary>
        /// Compares the two specified date instances to see if the left one is
        /// strictly later than the right one.
        /// </summary>
        public static bool operator >(GregorianDate left, GregorianDate right)
            => left._bin > right._bin;

        /// <summary>
        /// Compares the two specified date instances to see if the left one is
        /// later than or equal to the right one.
        /// </summary>
        public static bool operator >=(GregorianDate left, GregorianDate right)
            => left._bin >= right._bin;

        /// <summary>
        /// Obtains the earlier date of two specified dates.
        /// </summary>
        [Pure]
        public static GregorianDate Min(GregorianDate left, GregorianDate right)
            => left < right ? left : right;

        /// <summary>
        /// Obtains the later date of two specified dates.
        /// </summary>
        [Pure]
        public static GregorianDate Max(GregorianDate left, GregorianDate right)
            => left > right ? left : right;

        /// <summary>
        /// Indicates whether this date instance is earlier, later or the same
        /// as the specified one.
        /// </summary>
        [Pure]
        public int CompareTo(GregorianDate other)
            => _bin.CompareTo(other._bin);

        /// <inheritdoc />
        int IComparable.CompareTo(object? obj)
            => obj is null ? 1
                : obj is GregorianDate date ? _bin.CompareTo(date._bin)
                : throw EF.InvalidType(nameof(obj), typeof(GregorianDate), obj);
    }

    // Math ops.
    public partial struct GregorianDate
    {
        #region Natural operations.

        // The "natural" ops are those based on the day, the base unit of a
        // calendar.

#pragma warning disable CA2225 // Operator overloads have named alternates.

        /// <summary>
        /// Subtracts the two specified dates and returns the number of days
        /// between them at midnight (0h).
        /// </summary>
        public static int operator -(GregorianDate left, GregorianDate right)
            => left.CountDaysSince(right);

        /// <summary>
        /// Adds a number of days to the specified date, yielding a new date.
        /// </summary>
        /// <exception cref="OverflowException">The operation would overflow
        /// the calendar boundaries.</exception>
        public static GregorianDate operator +(GregorianDate value, int days)
            => value.PlusDays(days);

        /// <summary>
        /// Subtracts a number of days to the specified date, yielding a new date.
        /// </summary>
        /// <exception cref="OverflowException">The operation would overflow
        /// the calendar boundaries.</exception>
        public static GregorianDate operator -(GregorianDate value, int days)
            => value.PlusDays(-days);

        /// <summary>
        /// Adds one day to the specified date, yielding a new date.
        /// </summary>
        /// <exception cref="OverflowException">The operation would overflow the
        /// latest supported date.</exception>
        public static GregorianDate operator ++(GregorianDate value)
            => value.NextDay();

        /// <summary>
        /// Subtracts one day to the specified date, yielding a new date.
        /// </summary>
        /// <exception cref="OverflowException">The operation would underflow
        /// the earliest supported date.</exception>
        public static GregorianDate operator --(GregorianDate value)
            => value.PreviousDay();

#pragma warning restore CA2225

        [Pure]
        public int CountDaysSince(GregorianDate other)
            => ObYearMonth == other.ObYearMonth ? Day - other.Day
                : DaysSinceEpoch - other.DaysSinceEpoch;

        [Pure]
        public GregorianDate PlusDays(int days)
        {
            // The limits are chosen such that (this + days) is guaranteed to
            // stay in the range from (Year - 1) to (Year + 1).
            if (days < -365 || days > 365)
            {
                // Slow-track, we have to go through the DaysSinceEpoch.
                int daysSinceEpoch = checked(DaysSinceEpoch + days);

                if (daysSinceEpoch < MinDaysSinceEpoch || daysSinceEpoch > MaxDaysSinceEpoch)
                {
                    throw EF.DayNumberOverflowOrUnderflow;
                }

                return FromDaysSinceEpoch(daysSinceEpoch);
            }
            else
            {
                return PlusDaysFast(days);
            }
        }

        [Pure]
        public GregorianDate NextDay()
        {
            if (this == MaxValue) { throw EF.DayNumberOverflow; }

            Unpack(out int y, out int m, out int d);

            return d < 28 || d < CountDaysInMonth(y, m)
                ? CreateLenient(y, m, d + 1)
                : m < 12 ? CreateLenient(y, m + 1, 1)
                : new GregorianDate(((y + 1) << 9) | __StartOfYear);
        }

        [Pure]
        public GregorianDate PreviousDay()
        {
            if (this == MinValue) { throw EF.DayNumberUnderflow; }

            Unpack(out int y, out int m, out int d);

            return d > 1 ? CreateLenient(y, m, d - 1)
                : m > 1 ? CreateLenient(y, m - 1, CountDaysInMonth(y, m - 1))
                : new GregorianDate(((y - 1) << 9) | __EndOfYear);
        }

        [Pure]
        private GregorianDate PlusDaysFast(int days)
        {
            Debug.Assert(days >= -365);
            Debug.Assert(days <= 365);

            Unpack(out int y, out int m, out int d);

            int dom = d + days;
            if (dom >= 1 && (dom <= 28 || dom <= CountDaysInMonth(y, m)))
            {
                // Same month.
                return CreateLenient(y, m, dom);
            }

            int doy = DayOfYear + days;
            if (doy < 1)
            {
                // Move to the previous year.
                if (y == MinSupportedYear) { throw EF.YearUnderflow; }
                y--;
                doy += CountDaysInYear(y);
            }
            else
            {
                int daysInYear = CountDaysInYear(y);
                if (doy > daysInYear)
                {
                    // Move to the next year.
                    if (y == MaxSupportedYear) { throw EF.YearOverflow; }
                    y++;
                    doy -= daysInYear;
                }
            }

            return FromOrdinalDateImpl(y, doy);
        }

        #endregion

        /// <summary>
        /// Subtracts the two specified dates and returns the number of years,
        /// months and days between them.
        /// </summary>
        [Pure]
        public static (int Years, int Months, int Days) Subtract(
            GregorianDate left, GregorianDate right)
        {
            // At first, I counted the years, then the months and finally the days
            // but this is WRONG because doing so we lose information on the way,
            // we kind of accumulate "rounding errors". For instance, with
            //   start = 29/02/0008
            //   end   = 28/02/0012
            // we would conclude that there are between them
            //   3 years   (29/02/0008 -> 28/02/0011)
            //   12 months (28/02/0011 -> 28/02/0012)
            //   and 0 days
            // The problem is that "start" is intercalary and the two dates are
            // separated by at least one common year (otherwise it would work).
            // What we really should do is:
            //   totalMonths = 47 (29/02/0008 -> 29/01/0012)
            //   days        = 30 (29/01/0012 -> 28/02/0012)
            // which gives us 3 years, 11 months and 30 days.
            // TODO: explain.

            left.Unpack(out int y, out int m, out int d);
            right.Unpack(out int y0, out int m0, out int d0);

            // We use an unpatched count; see CountMonthsSince().
            int χ = left >= right ? ((d - d0) >> 31) : -((d0 - d) >> 31);
            int months = 12 * (y - y0) + (m - m0) + χ;

            int days = left - right.PlusMonths(months);

            // Reminder: C# integer division rounds towards zero, which is
            // exactly what we want here.
            return (months / 12, months % 12, days);
        }

        #region AddYears(), PlusYears() & CountYearsSince()

        // There is only one case where the month-day part changes: going
        // from an intercalary day to a common year.
        // Let y' := y + years,
        //   if d/m = 29/2
        //     if y' is leap    return 29/2/y'
        //     otherwise        return ??/?/y'
        //   otherwise          return  d/m/y'
        // The usual answer to the unsolved case is 28/2/y', but returning
        // 1/3/y' seems equally valid. Here our answer is the end of February.
        //
        // Then, there is the problem of counting the number of years between
        // two dates. Here, we choose to define the op such that:
        //   (date + years) - date -> years
        //   if (date - other) -> years then (other - date) -> -years

        /// <summary>
        /// Adds a number of years to the year field of the specified date
        /// instance, yielding a new date and also returns the cut-off "error"
        /// in an output parameter.
        /// </summary>
        /// <exception cref="OverflowException">The operation would overflow
        /// either the capacity of <see cref="Int32"/> or the calendar boundaries.
        /// </exception>
        [Pure]
        public static GregorianDate AddYears(
            GregorianDate date, int years, out int cutoff)
        {
            int y = checked(date.Year + years);

            if (y < MinSupportedYear || y > MaxSupportedYear) { throw EF.YearOverflowOrUnderflow; }

            int bMD = date.ObMonthDay;
            if (bMD == __IntercalaryDay && !IsLeapYear(y))
            {
                bMD = __EndOfFebruary;
                cutoff = 1;
            }
            else
            {
                cutoff = 0;
            }

            return new GregorianDate((y << 9) | bMD);
        }

        /// <summary>
        /// Counts the number of years from the specified date to this date
        /// instance.
        /// </summary>
        /// <remarks>
        /// This methods ensures that the following properties hold:
        /// <code>
        ///   date.PlusYears(years).CountYearsSince(date) == years;
        ///   date.CountYearsSince(other) == -other.CountYearsSince(date);
        /// </code>
        /// </remarks>
        [Pure]
        public int CountYearsSince(GregorianDate other)
        {
            int y = Year;
            int y0 = other.Year;
            int years = y - y0;

            if (years == 0) { return 0; }

            int bMD = ObMonthDay;
            int bMD0 = other.ObMonthDay;

            if (years > 0)
            {
                __patch(ref bMD0, y, bMD);
                // Trick: given "n" an Int32, if n >= 0 then n >> 31 = 0;
                // otherwise n >> 31 = -1. Instead of writing
                //   return years + (bMD < bMD0 ? -1 : 0);
                // and just for the fun (it might be slower actually), we use:
                return years + ((bMD - bMD0) >> 31);
            }
            else
            {
                __patch(ref bMD, y0, bMD0);
                // Same as above,
                //   return years + (bMD0 < bMD ? 1 : 0);
                return years - ((bMD0 - bMD) >> 31);
            }

            static void __patch(ref int md0, int y, int md)
            {
                if (md0 == __IntercalaryDay
                    && md == __EndOfFebruary
                    && !IsLeapYear(y))
                {
                    // Trick: to compare md0 and md, we pretend that the
                    // intercalary day is a 28/02.
                    md0 = __EndOfFebruary;
                }
            }
        }

        /// <summary>
        /// Adds a number of years to the year field of this date instance,
        /// yielding a new date.
        /// <para>When this date instance is an intercalary day and, whether the
        /// target year is leap or not, this method returns the end of February.
        /// </para>
        /// <remarks>
        /// This method is NOT "algebraic". If a date is not an intercalary day,
        /// we do have:
        /// <code>
        ///   date.PlusYears(years).PlusYears(-years) == date;
        /// </code>
        /// but, if it is an intercalary day, the equality only holds when
        /// <paramref name="years"/> is such that the resulting date is
        /// intercalary too.
        /// </remarks>
        /// </summary>
        /// <exception cref="OverflowException">The operation would overflow
        /// either the capacity of <see cref="Int32"/> or the calendar boundaries.
        /// </exception>
        [Pure]
        public GregorianDate PlusYears(int years)
        {
            int y = checked(Year + years);

            if (y < MinSupportedYear || y > MaxSupportedYear) { throw EF.YearOverflowOrUnderflow; }

            int bMD = ObMonthDay;
            if (bMD == __IntercalaryDay && !IsLeapYear(y))
            {
                bMD = __EndOfFebruary;
            }

            return new GregorianDate((y << 9) | bMD);
        }

        #endregion

        #region AddMonths(), PlusMonths() & CountMonthsSince()

        /// <summary>
        /// Adds a number of months to the month field of the specified date
        /// instance, yielding a new date.
        /// <para>This method also returns the cut-off "error" in an output
        /// parameter.</para>
        /// </summary>
        /// <exception cref="OverflowException">The operation would overflow
        /// either the capacity of <see cref="Int32"/> or the calendar boundaries.
        /// </exception>
        [Pure]
        public static GregorianDate AddMonths(
            GregorianDate date, int months, out int cutoff)
        {
            date.Unpack(out int y, out int m, out int d);

            // Only the op that may overflow is wrapped in a checked context.
            m = 1 + Modulo(checked(m - 1 + months), 12, out int y0);
            y += y0;

            if (y < MinSupportedYear || y > MaxSupportedYear) { throw EF.YearOverflowOrUnderflow; }

            int daysInMonth = CountDaysInMonth(y, m);
            cutoff = Math.Max(0, d - daysInMonth);

            return CreateLenient(y, m, cutoff > 0 ? daysInMonth : d);
        }

        /// <summary>
        /// Counts the number of months between the two specified dates.
        /// </summary>
        /// <remarks>
        /// This methods ensures that the following properties hold:
        /// <code>
        ///   date.PlusMonths(months).CountMonthsSince(date) == months;
        ///   date.CountMonthsSince(other) == -other.CountMonthsSince(date);
        /// </code>
        /// </remarks>
        [Pure]
        public int CountMonthsSince(GregorianDate other)
        {
            Unpack(out int y, out int m, out int d);
            other.Unpack(out int y0, out int m0, out int d0);

            int months = 12 * (y - y0) + (m - m0);

            // It works because of the particular form of the previous eq. and
            // because of the type of data involved.
            if (months == 0) { return 0; }

            if (this >= other)
            {
                __patch(ref d0, y, m, d);
                // See CountYearsSince() for an explanation.
                //   return months + (d < d0 ? -1 : 0);
                return months + ((d - d0) >> 31);
            }
            else
            {
                __patch(ref d, y0, m0, d0);
                // See CountYearsSince() for an explanation.
                //   return months + (d0 < d ? 1 : 0);
                return months - ((d0 - d) >> 31);
            }

            static void __patch(ref int d0, int y, int m, int d)
            {
                if (d0 > d && d == CountDaysInMonth(y, m))
                {
                    d0 = d;
                }
            }
        }

        /// <summary>
        /// Adds a number of months to the month field of this date instance,
        /// yielding a new date.
        /// </summary>
        /// <remarks>
        /// This method is NOT "algebraic". In general, we cannot assert that:
        /// <code>
        ///   date.PlusMonths(months).PlusMonths(-months) == date;
        /// </code>
        /// </remarks>
        /// <exception cref="OverflowException">The operation would overflow
        /// either the capacity of <see cref="Int32"/> or the calendar boundaries.
        /// </exception>
        [Pure]
        public GregorianDate PlusMonths(int months)
        {
            Unpack(out int y, out int m, out int d);

            // Only the op that may overflow is wrapped in a checked context.
            m = 1 + Modulo(checked(m - 1 + months), 12, out int y0);
            y += y0;

            if (y < MinSupportedYear || y > MaxSupportedYear) { throw EF.YearOverflowOrUnderflow; }

            return CreateLenient(y, m, Math.Min(d, CountDaysInMonth(y, m)));
        }

        #endregion
    }
}
