using System;
using System.Collections.Generic;
using System.Linq;

namespace AGenius.UsefulStuff
{
    /// <summary>
    /// DateTimeExtensions
    /// </summary>
    public static class DateTimeExtensions
    {
        static readonly List<string> monthNames = new List<string> { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        /// <summary>
        /// Return the full Month Name
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string MonthName(this DateTime date)
        {
            return monthNames[date.Month - 1];
        }
        /// <summary>
        /// Returns the months names 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="fromCurrent">start from month of date </param>
        /// <returns>List of strings</returns>
        public static List<string> MonthNameList(this DateTime date, bool fromCurrent = true)
        {
            int start = 1;
            if (fromCurrent)
            {
                start = date.Month;
            }

            List<string> names = new List<string>
            {
                monthNames[start - 1]
            };

            do
            {
                start++;
                if (start > 12)
                {
                    start = 1;
                }
                names.Add(monthNames[start - 1]);
            } while (names.Count < 12);
            return names;
        }
        public static DateTime AddWithinWorkingHours(this DateTime start, TimeSpan offset)
        {
            const int hoursPerDay = 8;
            const int startHour = 9;
            // Don't start counting hours until start time is during working hours
            if (start.TimeOfDay.TotalHours > startHour + hoursPerDay)
                start = start.Date.AddDays(1).AddHours(startHour);
            if (start.TimeOfDay.TotalHours < startHour)
                start = start.Date.AddHours(startHour);
            if (start.DayOfWeek == DayOfWeek.Saturday)
                start.AddDays(2);
            else if (start.DayOfWeek == DayOfWeek.Sunday)
                start.AddDays(1);
            // Calculate how much working time already passed on the first day
            TimeSpan firstDayOffset = start.TimeOfDay.Subtract(TimeSpan.FromHours(startHour));
            // Calculate number of whole days to add
            int wholeDays = (int)(offset.Add(firstDayOffset).TotalHours / hoursPerDay);
            // How many hours off the specified offset does this many whole days consume?
            TimeSpan wholeDaysHours = TimeSpan.FromHours(wholeDays * hoursPerDay);
            // Calculate the final time of day based on the number of whole days spanned and the specified offset
            TimeSpan remainder = offset - wholeDaysHours;
            // How far into the week is the starting date?
            int weekOffset = ((int)(start.DayOfWeek + 7) - (int)DayOfWeek.Monday) % 7;
            // How many weekends are spanned?
            int weekends = (int)((wholeDays + weekOffset) / 5);
            // Calculate the final result using all the above calculated values
            return start.AddDays(wholeDays + weekends * 2).Add(remainder);
        }
        public static DateTime AddWorkDays(this DateTime date, int workingDays, List<DateTime> dates = null)
        {
            int direction = workingDays < 0 ? -1 : 1;
            DateTime newDate = date;
            while (workingDays != 0)
            {
                newDate = newDate.AddDays(direction);
                if (newDate.DayOfWeek != DayOfWeek.Saturday &&
                    newDate.DayOfWeek != DayOfWeek.Sunday &&
                    !newDate.IsAHoliday(dates))
                {
                    workingDays -= direction;
                }
            }
            return newDate;

            //return date.GetDates(workingDays < 0)
            //    .Where(newDate =>
            //        (newDate.DayOfWeek != DayOfWeek.Saturday &&
            //         newDate.DayOfWeek != DayOfWeek.Sunday))
            //    .Take(Math.Abs(workingDays))
            //    .Last();
        }

        /// <summary>
        /// Checks whether the day of given DateTime is a holiday according to the configuration.
        /// Please refer the app.config for more information. 
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <param name="dates">List of dates to check</param>
        /// <returns>True if the given day is a holiday, false otherwise.</returns>
        public static bool IsAHoliday(this DateTime date, List<DateTime> dates = null)
        {
            if (dates != null && dates.Count > 0)
            {
                return dates.Contains(date.Date);
            }
            return false;
        }
        /// <summary>Return date for the start of the week for a supplied date</summary>
        /// <param name="dt">Date to use</param>
        /// <returns></returns>
        public static DateTime StartOfWeek(this DateTime dt)
        {
            System.Globalization.CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            DayOfWeek fdow = ci.DateTimeFormat.FirstDayOfWeek;
            return dt.AddDays(-(dt.DayOfWeek - fdow));
        }
        private static IEnumerable<DateTime> GetDates(this DateTime date, bool isForward)
        {
            while (true)
            {
                date = date.AddDays(isForward ? -1 : 1);
                yield return date;
            }
        }
        public static DateTime Round(this DateTime value, TimeSpan unit)
        {
            return Round(value, unit, default);
        }
        public static DateTime Round(this DateTime value, TimeSpan unit, MidpointRounding style)
        {
            if (unit <= TimeSpan.Zero) throw new ArgumentOutOfRangeException("unit", "value must be positive");

            Decimal units = (decimal)value.Ticks / (decimal)unit.Ticks;
            Decimal roundedUnits = Math.Round(units, style);
            long roundedTicks = (long)roundedUnits * unit.Ticks;
            DateTime instance = new DateTime(roundedTicks);

            return instance;
        }

        // Between and GetAge Copied from Tazmainiandevil/Useful.Extensions

        /// <summary>
        /// Determines if the date time is between a specified start and end time.
        /// </summary>
        /// <param name="datetime">The time to check. </param>
        /// <param name="startTime">The start of the range. </param>
        /// <param name="endTime">The end of the range. </param>
        /// <param name="timesInclusive">If the start and end times are to be included; defaults to <see langword="false"/>. </param>
        /// <returns>
        /// <see langword="True"/> if the date time is between a specified start and end time, else <see langword="false"/>.
        /// </returns>
        public static bool Between(this DateTime datetime, DateTime startTime, DateTime endTime, bool timesInclusive = false)
        {
            return timesInclusive
                       ? datetime >= startTime && datetime <= endTime
                       : datetime > startTime && datetime < endTime;
        }
        /// <summary>
        /// Determines the current age based on the date of birth / creation supplied.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth / creation supplied for the person / object we are looking the age for.</param>
        /// <param name="assessmentDate">The date that we want to use for the age check.</param>
        /// <returns>The age of the person or object.</returns>
        public static int GetAge(this DateTime dateOfBirth, DateTime assessmentDate = default)
        {
            var now = assessmentDate == default ? DateTime.Now : assessmentDate;
            var age = now.Year - dateOfBirth.Year;

            if (now.Month < dateOfBirth.Month || now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day) { age--; }

            return age;
        }
        /// <summary>Return a has based on the date</summary>
        /// <param name="dateTime">The DataTime value to hash</param>
        /// <param name="ResultLength">Return this many characters from the hash result (default : 8)</param>
        /// <returns></returns>
        public static string DateHash(this DateTime dateTime, int ResultLength = 8)
        {
            long timeSince1970 = (long)dateTime.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
            long d = timeSince1970 * 1000;
            long timeSince2000 = (long)new DateTime(2000, 10, 06).Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
            long od = timeSince2000 / 1000;

            string rst = ((d - od) / 1000).EncodeBase36();

            return rst.Substring(0, ResultLength).ToUpper();
        }
        /// <summary>Return True if date is a weekend (Saturday or Sunday)</summary>
        /// <param name="dateTime">DateTime to be checked.</param>
        /// <returns>bool</returns>
        public static bool isWeekend(this DateTime dateTime)
        {
            if (dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday)
            {
                return true;
            }
            return false;
        }
    }
}