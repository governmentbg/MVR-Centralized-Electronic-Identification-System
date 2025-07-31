using System.Globalization;

#nullable disable

namespace eID.PJS.LocalLogsSearch.Service
{
    public enum TimeInterval
    {
        Tick,
        Millisecond,
        Second,
        Minute,
        Hour,
        Day,
        Week,
        Month,
        Year,
    }

    public enum DateRangeTemplate
    {
        Today,
        ThisWeek,
        ThisMonth,
        ThisYear,
        Tomorrow,
        NextWeek,
        NextMonth,
        NextYear,
        Yesterday,
        PreviousWeek,
        PreviousMonth,
        PreviousYear,
        ThisQuarter,
        Q1,
        Q2,
        Q3,
        Q4,
        Last3Days,
        Last7Days,
        Last14Days,
        Last30Days,
        Last60Days,
        Last90Days,
        January,
        February,
        March,
        April,
        May,
        Jun,
        July,
        August,
        September,
        October,
        November,
        December
    }

    public enum DateShiftDirection
    {
        Forward, Backward
    }

    public struct DateRange : IComparable, IComparable<DateRange>, IFormattable, ICloneable, IEquatable<DateRange>
    {
        private DateTime _fromDate;
        private DateTime _toDate;

        public DateRange(DateTime fromDate, DateTime toDate)
        {
            _fromDate = fromDate;
            _toDate = toDate;
        }

        public DateRange(TimeSpan interval) : this(DateTime.Now, interval)
        {
        }

        public DateRange(DateTime fromDate, TimeSpan interval)
        {
            _fromDate = fromDate;
            _toDate = fromDate + interval;
        }

        public DateRange(TimeInterval interval, int value) : this(DateTime.Now, interval, value)
        {

        }

        public DateRange(TimeInterval interval) : this(DateTime.Now, interval, 1)
        {

        }

        public DateRange(DateTime fromDate, TimeInterval interval) : this(fromDate, interval, 1)
        {

        }

        public DateRange(DateTime fromDate, TimeInterval interval, int value)
        {
            if (value > 0)
            {
                _fromDate = fromDate;
                _toDate = DateExtensions.DateFromInterval(fromDate, interval, value);
            }
            else
            {
                _toDate = fromDate;
                _fromDate = DateExtensions.DateFromInterval(fromDate, interval, value);
            }
        }

        public DateTime FromDate
        {
            get
            {
                return _fromDate;
            }
            set
            {
                if (_toDate > DateTime.MinValue && value >= _toDate)
                {
                    _fromDate = _toDate;
                    _toDate = value;
                    return;
                }

                _fromDate = value;
            }
        }

        public DateTime ToDate
        {
            get
            {
                return _toDate;
            }
            set
            {
                _toDate = value;
            }
        }

        public TimeSpan ToTimeSpan()
        {
            return _toDate - _fromDate;
        }

        public bool IsInRange(DateTime date)
        {
            return date >= _fromDate && date <= _toDate;
        }

        public bool IsInRange(DateRange range)
        {
            return range.FromDate >= _fromDate && range.ToDate <= _toDate;
        }

        public bool IsOverlaping(DateRange range)
        {
            if (_fromDate <= range.FromDate && _toDate <= range.ToDate)
                return true;

            if (_fromDate >= range.FromDate && _toDate >= range.ToDate)
                return true;


            return false;
        }

        public DateRange AddRange(TimeSpan interval)
        {
            TimeSpan result = ToTimeSpan() + interval;
            _toDate += result;

            return this;
        }

        public DateRange AddRange(DateRange range)
        {
            TimeSpan result = ToTimeSpan() + range.ToTimeSpan();
            _toDate += result;

            return this;
        }

        public DateRange RemoveRange(TimeSpan interval)
        {
            TimeSpan result = ToTimeSpan() - interval;
            _toDate -= result;
            return this;
        }

        public DateRange RemoveRange(DateRange range)
        {
            TimeSpan result = ToTimeSpan() - range.ToTimeSpan();
            _toDate -= result;
            return this;
        }

        public DateRange ExtendTo(TimeSpan interval)
        {
            _toDate += interval;
            return this;
        }

        public DateRange ExtendFrom(TimeSpan interval)
        {
            _fromDate -= interval;
            return this;
        }

        public DateRange Extend(TimeSpan interval)
        {
            _fromDate -= interval;
            _toDate += interval;
            return this;
        }

        public DateRange Shrink(TimeSpan interval)
        {
            if (ToTimeSpan() < interval)
            {
                throw new ArgumentException("Cannot shrink. Interval is too big.");
            }

            _fromDate += interval;
            _toDate -= interval;
            return this;
        }

        public DateRange ShringTo(TimeSpan interval)
        {
            _toDate -= interval;
            return this;
        }

        public DateRange ShringFrom(TimeSpan interval)
        {
            _fromDate += interval;
            return this;
        }

        public DateRange Shift(TimeSpan interval, DateShiftDirection direction)
        {
            if (direction == DateShiftDirection.Forward)
            {
                _fromDate += interval;
                _toDate += interval;
            }
            else
            {
                _fromDate -= interval;
                _toDate -= interval;
            }

            return this;
        }

        public static DateRange operator +(DateRange range1, DateRange range2)
        {

            DateRange result = new DateRange();

            TimeSpan interval = range1.ToTimeSpan() + range2.ToTimeSpan();

            if (range1.FromDate <= range2.FromDate)
            {
                result.FromDate = range1.FromDate;
            }
            else
            {
                result.FromDate = range2.FromDate;
            }

            result.ToDate = result.FromDate + interval;
            return result;
        }
        public static bool operator ==(DateRange range1, DateRange range2)
        {
            return range1.Equals(range2);
        }
        public static bool operator !=(DateRange range1, DateRange range2)
        {
            return !range1.Equals(range2);
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is DateRange)
            {
                var range = (DateRange)obj;

                return CompareTo(range);
            }

            throw new ArgumentException("Expected object of type DateRange");
        }

        #endregion

        #region IComparable<DateRange> Members

        public int CompareTo(DateRange other)
        {
            TimeSpan current = ToTimeSpan();

            return current.CompareTo(other.ToTimeSpan());
        }

        #endregion

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, format, _fromDate, _toDate);
        }

        #endregion

        public object Clone()
        {
            return new DateRange(_fromDate, _toDate);
        }

        #region Static methods


        public static DateRange FromFullInterval(DateTime fromDate, TimeInterval interval, int value)
        {
            var date = FromInterval(new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0, fromDate.Kind), interval, value);
            date.ToDate = date.ToDate.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);
            return date;
        }

        public static DateRange FromInterval(DateTime fromDate, TimeInterval interval, int value)
        {
            DateRange range = new DateRange();

            if (value > 0)
            {
                range.FromDate = fromDate;
                range.ToDate = DateExtensions.DateFromInterval(fromDate, interval, value);
            }
            else
            {
                range.ToDate = fromDate;
                range.FromDate = DateExtensions.DateFromInterval(fromDate, interval, value);
            }

            return range;
        }

        public static DateRange FromTemplate(DateRangeTemplate template)
        {
            return FromTemplate(template, CultureInfo.CurrentCulture);
        }

        public static DateRange FromTemplateInvariantCulture(DateRangeTemplate template)
        {
            return FromTemplate(template, CultureInfo.InvariantCulture);
        }

        public static DateRange FromTemplate(DateRangeTemplate template, CultureInfo culture)
        {
            int month = 0;
            int year = 0;
            int day = 0;
            Calendar c = culture.Calendar;

            switch (template)
            {
                case DateRangeTemplate.NextMonth:
                    month = DateTime.Today.NextMonth();
                    year = DateTime.Today.Year;
                    if (month < DateTime.Today.Month)
                    {
                        year++;
                    }

                    day = c.GetDaysInMonth(year, month);
                    return new DateRange(new DateTime(year, month, 1), new DateTime(year, month, day).EndOfDay());

                case DateRangeTemplate.NextWeek:
                    return new DateRange(DateTime.Today.FirstDayOfWeek(culture).StartOfDay(), DateTime.Today.LastDayOfWeek().EndOfDay());
                case DateRangeTemplate.NextYear:
                    return new DateRange(new DateTime(DateTime.Today.Year + 1, 1, 1), new DateTime(DateTime.Today.Year + 1, 12, 31).EndOfDay());
                case DateRangeTemplate.PreviousMonth:
                    month = DateTime.Today.PreviousMonth();
                    year = DateTime.Today.Year;
                    if (month > DateTime.Today.Month)
                    {
                        year--;
                    }

                    day = c.GetDaysInMonth(year, month);
                    return new DateRange(new DateTime(year, month, 1), new DateTime(year, month, day).EndOfDay());
                case DateRangeTemplate.PreviousWeek:
                    return new DateRange(DateTime.Today.AddDays(-7).FirstDayOfWeek().StartOfDay(), DateTime.Today.AddDays(-7).LastDayOfWeek().EndOfDay());
                case DateRangeTemplate.PreviousYear:
                    return new DateRange(new DateTime(DateTime.Today.Year - 1, 1, 1), new DateTime(DateTime.Today.Year - 1, 12, 31).EndOfDay());
                case DateRangeTemplate.Q1:
                    return new DateRange(new DateTime(DateTime.Today.Year, 1, 1), new DateTime(DateTime.Today.Year, 3, 31).EndOfDay());
                case DateRangeTemplate.Q2:
                    return new DateRange(new DateTime(DateTime.Today.Year, 4, 1), new DateTime(DateTime.Today.Year, 6, 31).EndOfDay());
                case DateRangeTemplate.Q3:
                    return new DateRange(new DateTime(DateTime.Today.Year, 7, 1), new DateTime(DateTime.Today.Year, 9, 31).EndOfDay());
                case DateRangeTemplate.Q4:
                    return new DateRange(new DateTime(DateTime.Today.Year, 10, 1), new DateTime(DateTime.Today.Year, 12, 31).EndOfDay());
                case DateRangeTemplate.ThisMonth:
                    day = c.GetDaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
                    return new DateRange(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), new DateTime(DateTime.Today.Year, DateTime.Today.Month, day).EndOfDay());
                case DateRangeTemplate.ThisQuarter:
                    int thisMnt = DateTime.Today.Month;
                    if (thisMnt >= 1 && thisMnt <= 3)
                        return FromTemplate(DateRangeTemplate.Q1);
                    if (thisMnt >= 4 && thisMnt <= 6)
                        return FromTemplate(DateRangeTemplate.Q2);
                    if (thisMnt >= 7 && thisMnt <= 9)
                        return FromTemplate(DateRangeTemplate.Q3);
                    if (thisMnt >= 10 && thisMnt <= 12)
                        return FromTemplate(DateRangeTemplate.Q4);
                    break;
                case DateRangeTemplate.ThisWeek:
                    return new DateRange(DateTime.Today.FirstDayOfWeek(culture).StartOfDay(), DateTime.Today.LastDayOfWeek().EndOfDay());
                case DateRangeTemplate.ThisYear:
                    return new DateRange(new DateTime(DateTime.Today.Year, 1, 1), new DateTime(DateTime.Today.Year, 12, 31).EndOfDay());
                case DateRangeTemplate.Today:
                    return new DateRange(DateTime.Today, DateTime.Today.EndOfDay());
                case DateRangeTemplate.Tomorrow:
                    return new DateRange(DateTime.Today.Tomorrow(), DateTime.Today.Tomorrow().EndOfDay());
                case DateRangeTemplate.Yesterday:
                    return new DateRange(DateTime.Today.Yesterday(), DateTime.Today.Yesterday().EndOfDay());
                case DateRangeTemplate.Last3Days:
                    return new DateRange(DateTime.Today.AddDays(-3).StartOfDay(), DateTime.Today.EndOfDay());
                case DateRangeTemplate.Last7Days:
                    return new DateRange(DateTime.Today.AddDays(-7).StartOfDay(), DateTime.Today.EndOfDay());
                case DateRangeTemplate.Last14Days:
                    return new DateRange(DateTime.Today.AddDays(-14).StartOfDay(), DateTime.Today.EndOfDay());
                case DateRangeTemplate.Last30Days:
                    return new DateRange(DateTime.Today.AddDays(-30).StartOfDay(), DateTime.Today.EndOfDay());
                case DateRangeTemplate.Last60Days:
                    return new DateRange(DateTime.Today.AddDays(-60).StartOfDay(), DateTime.Today.EndOfDay());
                case DateRangeTemplate.Last90Days:
                    return new DateRange(DateTime.Today.AddDays(-90).StartOfDay(), DateTime.Today.EndOfDay());
                case DateRangeTemplate.January:
                    return new DateRange(new DateTime(DateTime.Today.Year, 1, 1).StartOfDay(), new DateTime(DateTime.Today.Year, 1, 31).EndOfDay());
                case DateRangeTemplate.February:
                    return new DateRange(new DateTime(DateTime.Today.Year, 2, 1).StartOfDay(), new DateTime(DateTime.Today.Year, 2, DateTime.IsLeapYear(DateTime.Today.Year) ? 29 : 28).EndOfDay());
                case DateRangeTemplate.March:
                    return new DateRange(new DateTime(DateTime.Today.Year, 3, 1).StartOfDay(), new DateTime(DateTime.Today.Year, 3, 31).EndOfDay());
                case DateRangeTemplate.April:
                    return new DateRange(new DateTime(DateTime.Today.Year, 4, 1).StartOfDay(), new DateTime(DateTime.Today.Year, 4, 30).EndOfDay());
                case DateRangeTemplate.May:
                    return new DateRange(new DateTime(DateTime.Today.Year, 5, 1).StartOfDay(), new DateTime(DateTime.Today.Year, 5, 31).EndOfDay());
                case DateRangeTemplate.Jun:
                    return new DateRange(new DateTime(DateTime.Today.Year, 6, 1).StartOfDay(), new DateTime(DateTime.Today.Year, 6, 30).EndOfDay());
                case DateRangeTemplate.July:
                    return new DateRange(new DateTime(DateTime.Today.Year, 7, 1).StartOfDay(), new DateTime(DateTime.Today.Year, 7, 31).EndOfDay());
                case DateRangeTemplate.August:
                    return new DateRange(new DateTime(DateTime.Today.Year, 8, 1).StartOfDay(), new DateTime(DateTime.Today.Year, 8, 31).EndOfDay());
                case DateRangeTemplate.September:
                    return new DateRange(new DateTime(DateTime.Today.Year, 9, 1).StartOfDay(), new DateTime(DateTime.Today.Year, 9, 30).EndOfDay());
                case DateRangeTemplate.October:
                    return new DateRange(new DateTime(DateTime.Today.Year, 10, 1).StartOfDay(), new DateTime(DateTime.Today.Year, 10, 31).EndOfDay());
                case DateRangeTemplate.November:
                    return new DateRange(new DateTime(DateTime.Today.Year, 11, 1).StartOfDay(), new DateTime(DateTime.Today.Year, 11, 30).EndOfDay());
                case DateRangeTemplate.December:
                    return new DateRange(new DateTime(DateTime.Today.Year, 12, 1).StartOfDay(), new DateTime(DateTime.Today.Year, 12, 31).EndOfDay());

            }

            return new DateRange();
        }

        public static DateRange MaxRange()
        {
            return new DateRange(DateTime.MinValue, DateTime.MaxValue);
        }

        public static DateRange Empty
        {
            get { return new DateRange(DateTime.MinValue, DateTime.MinValue); }
        }

        public static bool IsEmpty(DateRange range)
        {
            return range.FromDate == range.ToDate;
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} -:- {1}", _fromDate.ToISO8601(), _toDate.ToISO8601());
        }

        public bool Equals(DateRange other)
        {
            return other.FromDate == FromDate && other.ToDate == ToDate;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return Equals((DateRange)obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static bool operator <(DateRange left, DateRange right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DateRange left, DateRange right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DateRange left, DateRange right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DateRange left, DateRange right)
        {
            return left.CompareTo(right) >= 0;
        }
    }

    public static class DateExtensions
    {
        public static TimeSpan Create(TimeInterval interval)
        {
            switch (interval)
            {
                case TimeInterval.Day:
                    return new TimeSpan(1, 0, 0, 0);
                case TimeInterval.Hour:
                    return new TimeSpan(0, 1, 0, 0);
                case TimeInterval.Millisecond:
                    return new TimeSpan(0, 0, 0, 0, 1);
                case TimeInterval.Minute:
                    return new TimeSpan(0, 0, 1, 0, 0);
                case TimeInterval.Month:
                    return new TimeSpan(31, 0, 1, 0, 0);
                case TimeInterval.Second:
                    return new TimeSpan(0, 0, 0, 1, 0);
                case TimeInterval.Week:
                    return new TimeSpan(7, 0, 0, 0, 0);
                case TimeInterval.Year:
                    return new TimeSpan(365, 0, 0, 0, 0);
                default:
                    return new TimeSpan();
            }
        }

        public static DateTime FromInterval(this DateTime fromDate, TimeInterval interval, int value)
        {
            return DateFromInterval(fromDate, interval, value);
        }

        public static DateTime DateFromInterval(DateTime fromDate, TimeInterval interval, int value)
        {
            if (value == 0)
                return fromDate;

            switch (interval)
            {
                case TimeInterval.Tick:
                    return fromDate.AddTicks(value);
                case TimeInterval.Millisecond:
                    return fromDate.AddMilliseconds(value);
                case TimeInterval.Second:
                    return fromDate.AddSeconds(value);
                case TimeInterval.Minute:
                    return fromDate.AddMinutes(value);
                case TimeInterval.Hour:
                    return fromDate.AddHours(value);
                case TimeInterval.Day:
                    return fromDate.AddDays(value);
                case TimeInterval.Week:
                    return fromDate.AddDays(value * 7);
                case TimeInterval.Month:
                    return fromDate.AddMonths(value);
                case TimeInterval.Year:
                    return fromDate.AddYears(value);
            }

            return fromDate;
        }

        public static DateTime StartOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0, date.Kind);
        }

        public static DateTime EndOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999, date.Kind);
        }

        public static DateTime FirstDayOfWeek(this DateTime date)
        {
            return date.FirstDayOfWeek(CultureInfo.CurrentCulture);
        }

        public static DateTime FirstDayOfWeek(this DateTime date, CultureInfo culture)
        {
            int first = (int)culture.DateTimeFormat.FirstDayOfWeek;
            int day = (int)date.DayOfWeek;

            if (culture.DateTimeFormat.FirstDayOfWeek == DayOfWeek.Monday)
            {
                if (day == 0)
                    day = 7;
            }

            DateTime newDate = date.AddDays(-1 * (day - first));

            return newDate;
        }

        public static DateTime LastDayOfWeek(this DateTime date)
        {
            return date.FirstDayOfWeek().AddDays(6);
        }

        public static DateTime FirstDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime LastDayOfMonth(this DateTime date)
        {
            return date.LastDayOfMonth(CultureInfo.CurrentCulture);
        }

        public static DateTime LastDayOfMonth(this DateTime date, CultureInfo culture)
        {
            return new DateTime(date.Year, date.Month, culture.Calendar.GetDaysInMonth(date.Year, date.Month));
        }

        public static int NextMonth(this DateTime date)
        {
            return date.AddMonths(1).Month;
        }

        public static int PreviousMonth(this DateTime date)
        {
            return date.AddMonths(-1).Month;
        }

        public static DateTime Tomorrow(this DateTime date)
        {
            return date.AddDays(1);
        }

        public static DateTime Yesterday(this DateTime date)
        {
            return date.AddDays(-1);
        }

        /// <summary>
        /// Serialize DateTime value in format yyyy-MM-dd HH:mm:ss.lll 
        /// </summary>
        /// <param name="value">DateTime value</param>
        /// <returns>string in format yyyy-MM-dd HH:mm:ss.lll</returns>
        public static string DateTimeToString(this DateTime value)
        {
            string tz = value.Kind == DateTimeKind.Utc ? "Z" : "";


            return string.Format("{0}-{1}-{2}T{3}:{4}:{5}.{6}{7}",
                                 value.Year.ToString("0000"),
                                 value.Month.ToString("00"),
                                 value.Day.ToString("00"),
                                 value.Hour.ToString("00"),
                                 value.Minute.ToString("00"),
                                 value.Second.ToString("00"),
                                 value.Millisecond.ToString("000"), tz);


        }

        public static string ToISO8601(this DateTime value)
        {
            return value.DateTimeToString();
        }

        public static string DateTimeNullableToString(this DateTime? value)
        {
            if (value != null)
                return value.Value.DateTimeToString();

            return null;
        }


    }

}
