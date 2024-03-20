using System;

namespace AGenius.UsefulStuff
{
    public static partial class DateTimeXtensions
    {
        #region Common

        /// <summary>
        /// Gets the day string of the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The string representation of day.</returns>
        /// <example>Returns "Sunday" for the date 1.1.2012.</example>
        public static string GetDayString(this DateTime date)
        {
            return date.DayOfWeek.ToString();
        }

        /// <summary>
        /// Gets the month string of the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The string representation of month.</returns>
        /// <example>Returns "January" for the date 1.1.2012.</example>
        public static string GetMonthString(this DateTime date)
        {
            return date.ToString("MMMM");
        }

        #endregion

        #region DD/MM/YY methods

        /// <summary>
        /// Formats the given DateTime to "dd/MM/yy".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="timeformat">String that represents the time format to use</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "01/01/12" for the date 1.1.2012.</example>
        public static string ToDdMmYySlash(this DateTime date, string timeformat = "")
        {
            if (!string.IsNullOrEmpty(timeformat))
            {
                return date.ToString($"dd/MM/yy {timeformat}");
            }
            else
            {
                return date.ToString("dd/MM/yy");
            }
        }

        /// <summary>
        /// Formats the given DateTime to "dd.MM.yy".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="timeformat">String that represents the time format to use</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "01.01.12" for the date 1.1.2012.</example>
        public static string ToDdMmYyDot(this DateTime date, string timeformat = "")
        {
            if (!string.IsNullOrEmpty(timeformat))
            {
                return date.ToString($"dd.MM.yy {timeformat}");
            }
            else
            {
                return date.ToString("dd.MM.yy");
            }
        }

        /// <summary>
        /// Formats the given DateTime to "dd-MM-yy".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="timeformat">String that represents the time format to use</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "01-01-12" for the date 1.1.2012.</example>
        public static string ToDdMmYyHyphen(this DateTime date, string timeformat = "")
        {
            if (!string.IsNullOrEmpty(timeformat))
            {
                return date.ToString($"dd-MM-yy {timeformat}");
            }
            else
            {
                return date.ToString("dd-MM-yy");
            }
        }

        /// <summary>
        /// Formats the given DateTime to "ddMMyy" by applying the given separator.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="separator">The given separator.</param>
        /// <param name="timeformat">String that represents the time format to use</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "01,01,12" for the date 1.1.2012 and separator ,.</example>
        public static string ToDdMmYyWithSep(this DateTime date, string separator, string timeformat = "")
        {
            if (!string.IsNullOrEmpty(timeformat))
            {
                return date.ToString($"dd{separator}MM{separator}yy {timeformat}");
            }
            else
            {
                return date.ToString($"dd{separator}MM{separator}yy");
            }
        }

        #endregion

        #region DD/MM/YYYY methods

        /// <summary>
        /// Formats the given DateTime to "dd/MM/yyyy".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="timeformat">String that represents the time format to use</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "01/01/2012" for the date 1.1.2012.</example>
        public static string ToDdMmYyyySlash(this DateTime date, string timeformat = "")
        {
            if (!string.IsNullOrEmpty(timeformat))
            {
                return date.ToString($"dd/MM/yyyy {timeformat}");
            }
            else
            {
                return date.ToString("dd/MM/yyyy");
            }
        }

        /// <summary>
        /// Formats the given DateTime to "dd.MM.yyyy".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="timeformat">String that represents the time format to use</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "01.01.2012" for the date 1.1.2012.</example>
        public static string ToDdMmYyyyDot(this DateTime date, string timeformat = "")
        {
            if (!string.IsNullOrEmpty(timeformat))
            {
                return date.ToString($"dd.MM.yyyy {timeformat}");
            }
            else
            {
                return date.ToString("dd.MM.yyyy");
            }
        }

        /// <summary>
        /// Formats the given DateTime to "dd-MM-yyyy".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="timeformat">String that represents the time format to use</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "01-01-2012" for the date 1.1.2012.</example>
        public static string ToDdMmYyyyHyphen(this DateTime date, string timeformat = "")
        {
            if (!string.IsNullOrEmpty(timeformat))
            {
                return date.ToString($"dd-MM-yyyy {timeformat}");
            }
            else
            {
                return date.ToString("dd-MM-yyyy");
            }
        }

        /// <summary>
        /// Formats the given DateTime to "ddMMyyyy" by applying the given separator.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="separator">The given separator.</param>
        /// <param name="timeformat">String that represents the time format to use</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "01,01,2012" for the given DateTime 1.1.2012 and separator ,.</example>
        public static string ToDdMmYyyyWithSep(this DateTime date, string separator, string timeformat = "")
        {
            if (!string.IsNullOrEmpty(timeformat))
            {
                return date.ToString($"dd{separator}MM{separator}yyyy {timeformat}");
            }
            else
            {
                return date.ToString($"dd{separator}MM{separator}yyyy");
            }
        }

        #endregion

        #region MM/DD/YY methods

        /// <summary>
        /// Formats the given DateTime to "MM/dd/yy".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="timeformat">String that represents the time format to use</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "12/01/12" for the date 1.12.2012.</example>
        public static string ToMmDdYySlash(this DateTime date, string timeformat = "")
        {
            if (!string.IsNullOrEmpty(timeformat))
            {
                return date.ToString($"MM/dd/yy {timeformat}");
            }
            else
            {
                return date.ToString("MM/dd/yy");
            }
        }

        /// <summary>
        /// Formats the given DateTime to "MM.dd.yy".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="timeformat">String that represents the time format to use</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "12.01.12" for the date 1.12.2012.</example>
        public static string ToMmDdYyDot(this DateTime date, string timeformat = "")
        {
            if (!string.IsNullOrEmpty(timeformat))
            {
                return date.ToString($"MM.dd.yy {timeformat}");
            }
            else
            {
                return date.ToString("MM.dd.yy");
            }
        }

        /// <summary>
        /// Formats the given DateTime to "MM-dd-yy".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="timeformat">String that represents the time format to use</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "12-01-12" for the date 1.12.2012.</example>
        public static string ToMmDdYyHyphen(this DateTime date, string timeformat = "")
        {
            if (!string.IsNullOrEmpty(timeformat))
            {
                return date.ToString($"MM-dd-yy {timeformat}");
            }
            else
            {
                return date.ToString("MM-dd-yy");
            }
        }

        /// <summary>
        /// Formats the given DateTime to "MMddyy" by applying the given separator.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="separator">The given separator.</param>
        /// <param name="timeformat">String that represents the time format to use</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "12,01,12" for the given DateTime1.12.2012 and separator ,.</example>
        public static string ToMmDdYyWithSep(this DateTime date, string separator, string timeformat = "")
        {
            if (!string.IsNullOrEmpty(timeformat))
            {
                return date.ToString($"dd.MM.yyyy {timeformat}");
            }
            else
            {
                return date.ToString($"MM{separator}dd{separator}yy");
            }
        }

        #endregion

        #region MM/DD/YYYY methods

        /// <summary>
        /// Formats the given DateTime to "MM/dd/yyyy".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="timeformat">String that represents the time format to use</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "12/01/2012" for the date 1.12.2012.</example>
        public static string ToMmDdYyyySlash(this DateTime date, string timeformat = "")
        {
            if (!string.IsNullOrEmpty(timeformat))
            {
                return date.ToString($"MM/dd/yyyy {timeformat}");
            }
            else
            {
                return date.ToString("MM/dd/yyyy");
            }
        }

        /// <summary>
        /// Formats the given DateTime to "MM.dd.yyyy".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="timeformat">String that represents the time format to use</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "12.01.2012" for the date 1.12.2012.</example>
        public static string ToMmDdYyyyDot(this DateTime date, string timeformat = "")
        {
            if (!string.IsNullOrEmpty(timeformat))
            {
                return date.ToString($"MM.dd.yyyy {timeformat}");
            }
            else
            {
                return date.ToString("MM.dd.yyyy");
            }
        }

        /// <summary>
        /// Formats the given DateTime to "MM-dd-yyyy".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="timeformat">String that represents the time format to use</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "12-01-2012" for the date 1.12.2012.</example>
        public static string ToMmDdYyyyHyphen(this DateTime date, string timeformat = "")
        {
            if (!string.IsNullOrEmpty(timeformat))
            {
                return date.ToString($"MM-dd-yyyy {timeformat}");
            }
            else
            {
                return date.ToString("MM-dd-yyyy");
            }
        }

        /// <summary>
        /// Formats the given DateTime to "MMddyyyy" by applying the given separator.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="separator">The given separator.</param>
        /// <param name="timeformat">String that represents the time format to use</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "12,01,2012" for the given DateTime1.12.2012 and separator ,.</example>
        public static string ToMmDdYyyyWithSep(this DateTime date, string separator, string timeformat = "")
        {
            if (!string.IsNullOrEmpty(timeformat))
            {
                return date.ToString($"dd.MM.yyyy {timeformat}");
            }
            else
            {
                return date.ToString($"MM{separator}dd{separator}yyyy");
            }
        }

        #endregion

        #region YY/MM/DD methods

        /// <summary>
        /// Formats the given DateTime to "yy/MM/dd".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="timeformat">String that represents the time format to use</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "12/11/30" for the date 30.11.2012.</example>
        public static string ToYyMmDdSlash(this DateTime date, string timeformat = "")
        {
            if (!string.IsNullOrEmpty(timeformat))
            {
                return date.ToString($"yy/MM/dd {timeformat}");
            }
            else
            {
                return date.ToString("yy/MM/dd");
            }
        }

        /// <summary>
        /// Formats the given DateTime to "yy.MM.dd".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="timeformat">String that represents the time format to use</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "12.11.30" for the date 30.11.2012.</example>
        public static string ToYyMmDdDot(this DateTime date, string timeformat = "")
        {
            if (!string.IsNullOrEmpty(timeformat))
            {
                return date.ToString($"yy.MM.dd {timeformat}");
            }
            else
            {
                return date.ToString("yy.MM.dd");
            }
        }

        /// <summary>
        /// Formats the given DateTime to "yy-MM-dd".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="timeformat">String that represents the time format to use</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "12-11-30" for the date 30.11.2012.</example>
        public static string ToYyMmDdHyphen(this DateTime date, string timeformat = "")
        {
            if (!string.IsNullOrEmpty(timeformat))
            {
                return date.ToString($"yy-MM-dd {timeformat}");
            }
            else
            {
                return date.ToString("yy-MM-dd");
            }
        }

        /// <summary>
        /// Formats the given DateTime to "yyMMdd" by applying the given separator.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="separator">The given separator.</param>
        /// <param name="timeformat">String that represents the time format to use</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "12,11,30" for the given DateTime30.11.2012 and separator ,.</example>
        public static string ToYyMmDdWithSep(this DateTime date, string separator, string timeformat = "")
        {
            if (!string.IsNullOrEmpty(timeformat))
            {
                return date.ToString($"yy{separator}MM{separator}dd {timeformat}");
            }
            else
            {
                return date.ToString($"yy{separator}MM{separator}dd");
            }
        }

        #endregion

        #region YYYY/MM/DD methods

        /// <summary>
        /// Formats the given DateTime to "yyyy/MM/dd".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="timeformat">String that represents the time format to use</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "2012/11/30" for the date 30.11.2012.</example>          
        public static string ToYyyyMmDdSlash(this DateTime date, string timeformat = "")
        {
            if (!string.IsNullOrEmpty(timeformat))
            {
                return date.ToString($"yyyy/MM/dd {timeformat}");
            }
            else
            {
                return date.ToString("yyyy/MM/dd");
            }
        }

        /// <summary>
        /// Formats the given DateTime to "yyyy.MM.dd".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="timeformat">String that represents the time format to use</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "2012.11.30" for the date 30.11.2012.</example>           
        public static string ToYyyyMmDdDot(this DateTime date, string timeformat = "")
        {
            if (!string.IsNullOrEmpty(timeformat))
            {
                return date.ToString($"yyyy.MM.dd {timeformat}");
            }
            else
            {
                return date.ToString("yyyy.MM.dd");
            }
        }

        /// <summary>
        /// Formats the given DateTime to "yyyy-MM-dd".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="timeformat">String that represents the time format to use</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "2012-11-30" for the date 30.11.2012.</example>           
        public static string ToYyyyMmDdHyphen(this DateTime date, string timeformat = "")
        {
            if (!string.IsNullOrEmpty(timeformat))
            {
                return date.ToString($"yyyy-MM-dd {timeformat}");
            }
            else
            {
                return date.ToString("yyyy-MM-dd");
            }
        }

        /// <summary>
        /// Formats the given DateTime to "yyyyMMdd".
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="separator">The given separator.</param>
        /// <param name="timeformat">String that represents the time format to use</param>
        /// <returns>The string representation according to the format.</returns>
        /// <example>Returns "2012,11,30" for the given DateTime30.11.2012 and separator ,.</example>           
        public static string ToYyyyMmDdWithSep(this DateTime date, string separator, string timeformat = "")
        {
            if (!string.IsNullOrEmpty(timeformat))
            {
                return date.ToString($"yyyy{separator}MM{separator}dd {timeformat}");
            }
            else
            {
                return date.ToString($"yyyy{separator}MM{separator}dd");
            }
        }

        #endregion
    }
}