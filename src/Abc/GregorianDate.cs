// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

namespace Abc
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Runtime.CompilerServices;

    using static Abc.Utilities.MathOperations;

    using AoorException = System.ArgumentOutOfRangeException;
    using EF = Utilities.ExceptionFactoryEx;

    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public readonly partial struct GregorianDate
        : IEquatable<GregorianDate>, IComparable<GregorianDate>, IComparable
    {
        private const int DaysPer400Years = 400 * 365 + 97;
        private const int DaysFromMarchToDecember = 306;

        public const int MinDaysSinceEpoch = 0;
        public const int MaxDaysSinceEpoch = 3_652_058;
        public const int MinSupportedYear = 1;
        public const int MaxSupportedYear = 9999;

        public static readonly GregorianDate MinValue = new GregorianDate(1, 1, 1);
        public static readonly GregorianDate MaxValue = new GregorianDate(MaxSupportedYear, 12, 31);

        private readonly int _bin;

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
            if (day < 1 || (day > 28 && day > CountDaysInMonth(year, month)))
            {
                throw new AoorException(nameof(day));
            }

            _bin = Pack(year, month, day);
        }

        private GregorianDate(int bin)
        {
            __CheckBinaryData(bin);
            _bin = bin;
        }

        [Pure]
        internal static GregorianDate CreateLenient(int y, int m, int d) =>
            new GregorianDate(Pack(y, m, d));

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
                int r0 = Year % 100;
                int mod = r0 >= 0 ? r0 : (r0 + 100);
                return mod == 0 ? 100 : mod;
            }
        }

        public int Year => _bin >> 9;

        public int Month => (_bin >> 5) & MonthMask;

        public int DayOfYear
        {
            get
            {
                Unpack(out int y, out int m, out int d);
                return CountDaysInYearBeforeMonth(y, m) + d;
            }
        }

        public int Day => _bin & DayMask;

        public DayOfWeek DayOfWeek
        {
            get
            {
                Unpack(out int y, out int m, out int d);
                int doomsday = GetDoomsday(y, m);
                return (DayOfWeek)((doomsday + d) % 7);
            }
        }

        public int IsoWeekday
        {
            get
            {
                Unpack(out int y, out int m, out int d);
                int doomsday = GetDoomsday(y, m);
                return AdjustedModulo(doomsday + d, 7);
            }
        }

        public int WeekOfYear
        {
            get
            {
                int weekday = GetIsoWeekdayAtStartOfYear(Year);
                return (DayOfYear + 5 + weekday) / 7;
            }
        }

        public bool IsIntercalary => ObMonthDay == __IntercalaryDay;

        private int ObYearMonth => _bin >> 5;
        private int ObMonthDay => _bin & ((1 << 9) - 1);

        public int DaysSinceEpoch
        {
            get
            {
                Unpack(out int y, out int m, out int d);

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

        [ExcludeFromCodeCoverage]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        private string DebuggerDisplay => Convert.ToString(_bin, 2);

        [Pure]
        public override string ToString()
        {
            Unpack(out int y, out int m, out int d);
            return FormattableString.Invariant($"{y:D4}-{m:D2}-{d:D2}");
        }

        public void Deconstruct(out int year, out int month, out int day) =>
            Unpack(out year, out month, out day);

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

        [Pure]
        private static int GetIsoWeekdayAtStartOfYear(int y)
        {
            y--;
            int c = y / 100;
            return AdjustedModulo(1 + y + (y >> 2) - c + (c >> 2), 7);
        }
    }

    // Helpers.
    public partial struct GregorianDate
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsLeapYear(int y) =>
            (y & 3) == 0 && (y % 100 != 0 || y % 400 == 0);

        [Pure]
        private static int CountDaysInYear(int y) => IsLeapYear(y) ? 366 : 365;

        [Pure]
        private static int CountDaysInYearBeforeMonth(int y, int m) =>
            m < 3 ? 31 * (m - 1)
                : IsLeapYear(y) ? (153 * m - 157) / 5
                : (153 * m - 162) / 5;

        [Pure]
        private static int CountDaysInMonth(int y, int m) =>
            m != 2 ? 30 + ((m + (m >> 3)) & 1)
                : IsLeapYear(y) ? 29
                : 28;

        [Pure]
        private static int GetDoomsday(int y, int m)
        {
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

        private const int __IntercalaryDay = (2 << 5) | 29;
        private const int __StartOfMarch = (3 << 5) | 1;
        private const int __StartOfYear = (1 << 5) | 1;
        private const int __EndOfYear = (12 << 5) | 31;

        [Pure]
        public static GregorianDate FromBinary(int bin)
        {
            ValidateBinaryData(bin);
            return new GregorianDate(bin);
        }

        [Pure]
        public int ToBinary() => _bin;

        private static void ValidateBinaryData(int bin)
        {
            int h = bin >> 24;
            if (h != 0)
            {
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

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Pack(int y, int m, int d) => (y << 9) | (m << 5) | d;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Unpack(out int y, out int m, out int d)
        {
            int bin = _bin;

            y = bin >> 9;
            m = (bin >> 5) & MonthMask;
            d = bin & DayMask;
        }

        [Conditional("DEBUG")]
        [ExcludeFromCodeCoverage]
        private static void __CheckBinaryData(int bin) => ValidateBinaryData(bin);
    }

    // Conversions, adjustments...
    public partial struct GregorianDate
    {
        [Pure]
        public static GregorianDate FromDateTime(DateTime date) =>
            new GregorianDate(date.Year, date.Month, date.Day);

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
        public GregorianDate GetStartOfYear() =>
            new GregorianDate((Year << 9) | __StartOfYear);

        [Pure]
        public GregorianDate GetEndOfYear() =>
            new GregorianDate((Year << 9) | __EndOfYear);

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

        [Pure]
        public GregorianDate Previous(DayOfWeek dayOfWeek)
        {
            if (dayOfWeek < DayOfWeek.Sunday || dayOfWeek > DayOfWeek.Saturday)
            {
                throw new AoorException(nameof(dayOfWeek));
            }

            int δ = dayOfWeek - DayOfWeek;
            return PlusDaysFast(δ >= 0 ? δ - 7 : δ);
        }

        [Pure]
        public GregorianDate PreviousOrSame(DayOfWeek dayOfWeek)
        {
            if (dayOfWeek < DayOfWeek.Sunday || dayOfWeek > DayOfWeek.Saturday)
            {
                throw new AoorException(nameof(dayOfWeek));
            }

            int δ = dayOfWeek - DayOfWeek;
            return δ == 0 ? this : PlusDaysFast(δ > 0 ? δ - 7 : δ);
        }

        [Pure]
        public GregorianDate NextOrSame(DayOfWeek dayOfWeek)
        {
            if (dayOfWeek < DayOfWeek.Sunday || dayOfWeek > DayOfWeek.Saturday)
            {
                throw new AoorException(nameof(dayOfWeek));
            }

            int δ = dayOfWeek - DayOfWeek;
            return δ == 0 ? this : PlusDaysFast(δ < 0 ? δ + 7 : δ);
        }

        [Pure]
        public GregorianDate Next(DayOfWeek dayOfWeek)
        {
            if (dayOfWeek < DayOfWeek.Sunday || dayOfWeek > DayOfWeek.Saturday)
            {
                throw new AoorException(nameof(dayOfWeek));
            }

            int δ = dayOfWeek - DayOfWeek;
            return PlusDaysFast(δ <= 0 ? δ + 7 : δ);
        }

        #endregion
    }

    // Enumerate days in a month or a year.
    public partial struct GregorianDate
    {
        [Pure]
        public static IEnumerable<GregorianDate> GetDaysInYear(int year)
        {
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

        [Pure]
        public static IEnumerable<GregorianDate> GetDaysInMonth(int year, int month)
        {
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
        public static bool operator ==(GregorianDate left, GregorianDate right) =>
            left._bin == right._bin;

        public static bool operator !=(GregorianDate left, GregorianDate right) =>
            left._bin != right._bin;

        [Pure]
        public bool Equals(GregorianDate other) => _bin == other._bin;

        [Pure]
        public override bool Equals(object? obj) =>
            obj is GregorianDate date && _bin == date._bin;

        [Pure]
        public override int GetHashCode() => _bin;
    }

    // Interface IComparable<>.
    public partial struct GregorianDate
    {
        public static bool operator <(GregorianDate left, GregorianDate right) =>
            left._bin < right._bin;

        public static bool operator <=(GregorianDate left, GregorianDate right) =>
            left._bin <= right._bin;

        public static bool operator >(GregorianDate left, GregorianDate right) =>
            left._bin > right._bin;

        public static bool operator >=(GregorianDate left, GregorianDate right) =>
            left._bin >= right._bin;

        [Pure]
        public static GregorianDate Min(GregorianDate left, GregorianDate right) =>
            left < right ? left : right;

        [Pure]
        public static GregorianDate Max(GregorianDate left, GregorianDate right) =>
            left > right ? left : right;

        [Pure]
        public int CompareTo(GregorianDate other) => _bin.CompareTo(other._bin);

        int IComparable.CompareTo(object? obj) =>
            obj is null ? 1
                : obj is GregorianDate date ? _bin.CompareTo(date._bin)
                : throw EF.InvalidType(nameof(obj), typeof(GregorianDate), obj);
    }

    // Natural math ops: no month or year manips.
    public partial struct GregorianDate
    {
#pragma warning disable CA2225 // Operator overloads have named alternates.

        public static int operator -(GregorianDate left, GregorianDate right) =>
            left.CountDaysSince(right);

        public static GregorianDate operator +(GregorianDate value, int days) =>
            value.PlusDays(days);

        public static GregorianDate operator -(GregorianDate value, int days) =>
            value.PlusDays(-days);

        public static GregorianDate operator ++(GregorianDate value) => value.NextDay();

        public static GregorianDate operator --(GregorianDate value) => value.PreviousDay();

#pragma warning restore CA2225

        [Pure]
        public int CountDaysSince(GregorianDate other) =>
            ObYearMonth == other.ObYearMonth ? Day - other.Day
                : DaysSinceEpoch - other.DaysSinceEpoch;

        [Pure]
        public GregorianDate PlusDays(int days)
        {
            if (days < -365 || days > 365)
            {
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
                return CreateLenient(y, m, dom);
            }

            int doy = DayOfYear + days;
            if (doy < 1)
            {
                if (y == MinSupportedYear) { throw EF.YearUnderflow; }
                y--;
                doy += CountDaysInYear(y);
            }
            else
            {
                int daysInYear = CountDaysInYear(y);
                if (doy > daysInYear)
                {
                    if (y == MaxSupportedYear) { throw EF.YearOverflow; }
                    y++;
                    doy -= daysInYear;
                }
            }

            return FromOrdinalDateImpl(y, doy);
        }
    }
}
