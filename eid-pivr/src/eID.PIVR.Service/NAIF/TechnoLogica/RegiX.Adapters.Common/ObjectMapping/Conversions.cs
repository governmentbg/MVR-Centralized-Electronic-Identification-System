using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace TechnoLogica.RegiX.Adapters.Common.ObjectMapping
{
    /// <summary>
    /// Utility class used for conversions in the mapper objects
    /// </summary>
    public class Conversions
    {
        /// <summary>
        /// Formats
        /// </summary>
        private static readonly string[] Formats = new string[] { "yyyy", "'+'yyyy", "'-'yyyy", "yyyyzzz", "'+'yyyyzzz", "'-'yyyyzzz" };

        /// <summary>
        /// Converts number to xs:GDay
        /// </summary>
        public static Func<object, object> ToGDay = (o) => string.Format("---{0:00}", o);

        /// <summary>
        /// Converts number to xs:GMonth
        /// </summary>
        public static Func<object, object> ToGMonth = (o) => string.Format("--{0:00}", o);

        /// <summary>
        /// Converts from xs:GDay to number
        /// </summary>
        public static Func<string, int> GDayToInt =
            (s) =>
            {
                Match match = Regex.Match(s, @"---(?<number>\d{2})");
                if (match.Success)
                {
                    return Convert.ToInt32(match.Groups["number"].Value);
                }
                else
                {
                    throw new ApplicationException("Invalid GDay!");
                }
            };

        /// <summary>
        /// Converts from xs:Month to number
        /// </summary>
        public static Func<string, int> GMonthToInt =
            (s) =>
            {
                Match match = Regex.Match(s, @"--(?<number>\d{2})");
                if (match.Success)
                {
                    return Convert.ToInt32(match.Groups["number"].Value);
                }
                else
                {
                    throw new ApplicationException("Invalid GMonth!");
                }
            };


        /// <summary>
        /// Converts from xs:GYear to number
        /// </summary>
        public static Func<string, int> GYearToInt =
            (s) =>
            {
                int sign = 1;
                if (s[0] == '-')
                {
                    sign = -1;
                }
                return DateTime.ParseExact(s, Formats, CultureInfo.InvariantCulture, DateTimeStyles.None).Year * sign;
            };

        /// <summary>
        /// Converts from TimeSpan to xs:duration
        /// </summary>
        public static Func<object, object> ToTimeSpan = (o) =>
            {
                if (o != null && o != System.DBNull.Value)
                {
                    TimeSpan ts = TimeSpan.Parse(o.ToString());
                    StringBuilder sb = new StringBuilder();

                    if (ts.Ticks < 0)
                    {
                        sb.Append('-');
                    }
                    bool isFractionalSeconds = ((double)((int)ts.TotalSeconds)) != ts.TotalSeconds;

                    sb.AppendFormat("P{0}D", (int)ts.TotalDays);
                    sb.AppendFormat("T{0}H", ts.Hours);
                    sb.AppendFormat("{0}M", ts.Minutes);
                    sb.AppendFormat("{0}S", isFractionalSeconds ? string.Format("{0}.{1}", ts.Seconds, ts.Milliseconds) : ts.Seconds.ToString());

                    return sb.ToString();
                }
                else
                {
                    return o;
                }
            };

        /// <summary>
        /// Converts from ЕГН to birth date
        /// </summary>
        public static Func<object, object> EGNtoBirthDate = (o) =>
        {
            if (o != null && o != System.DBNull.Value)
            {
                DateTime result = new DateTime();
                try
                {
                    string monthFirstLetter = o.ToString().Substring(2, 1);
                    if (monthFirstLetter.Equals("4") || monthFirstLetter.Equals("5"))
                    {
                        int year = 2000 + Convert.ToInt32(o.ToString().Substring(0, 2));
                        int month = Convert.ToInt32(o.ToString().Substring(2, 2)) - 40;
                        int day = Convert.ToInt32(o.ToString().Substring(4, 2));
                        result = new DateTime(year, month, day);
                    }
                    else
                    {
                        if (monthFirstLetter.Equals("2") || monthFirstLetter.Equals("3"))
                        {
                            int year = 1800 + Convert.ToInt32(o.ToString().Substring(0, 2));
                            int month = Convert.ToInt32(o.ToString().Substring(2, 2)) - 20;
                            int day = Convert.ToInt32(o.ToString().Substring(4, 2));
                            result = new DateTime(year, month, day);
                        }
                        else
                        {
                            int year = 1900 + Convert.ToInt32(o.ToString().Substring(0, 2));
                            int month = Convert.ToInt32(o.ToString().Substring(2, 2));
                            int day = Convert.ToInt32(o.ToString().Substring(4, 2));
                            result = new DateTime(year, month, day);
                        }
                    }
                }
                catch
                {
                    return null;
                }
                return result;
            }
            else
            {
                return o;
            }
        };

        /// <summary>
        /// Converts from digit to sex
        /// </summary>
        public static Func<object, object> ToGenderName = (o) =>
        {
            if (o != null && o != System.DBNull.Value)
            {
                string result;
                switch (o.ToString())
                {
                    case "1":
                        result = "Мъж";
                        break;
                    case "2":
                        result = "Жена"; 
                        break;
                    default:
                        result = null;
                        break;
                }
                return result;
            }
            else
            {
                return o;
            }
        };
    }
}
