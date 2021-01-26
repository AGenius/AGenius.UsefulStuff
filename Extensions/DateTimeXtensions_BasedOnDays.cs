using System;
using System.Linq;
namespace AGenius.UsefulStuff
{
    public static partial class DateTimeXtensions
    {
        /// <summary>
        /// Checks whether the day of given DateTime is a Monday.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the day is Monday, false otherwise.</returns>
        public static bool IsAMonday(this DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Monday;
        }

        /// <summary>
        /// Checks whether the day of given DateTime is a Tuesday.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the day is Tuesday, false otherwise.</returns>
        public static bool IsATuesday(this DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Tuesday;
        }

        /// <summary>
        /// Checks whether the day of given DateTime is a Wednesday.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the day is Wednesday, false otherwise.</returns>
        public static bool IsAWednesday(this DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Wednesday;
        }

        /// <summary>
        /// Checks whether the day of given DateTime is a Thursday.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the day is Thursday, false otherwise.</returns>
        public static bool IsAThursday(this DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Thursday;
        }

        /// <summary>
        /// Checks whether the day of given DateTime is a Friday.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the day is Friday, false otherwise.</returns>
        public static bool IsAFriday(this DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Friday;
        }

        /// <summary>
        /// Checks whether the day of given DateTime is a Saturday.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the day is Saturday, false otherwise.</returns>
        public static bool IsASaturday(this DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday;
        }

        /// <summary>
        /// Checks whether the day of given DateTime is a Sunday.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the day is Sunday, false otherwise.</returns>
        public static bool IsASunday(this DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Sunday;
        }

        /// <summary>
        /// Checks whether the day of given DateTime is a working day according to the configuration.
        /// Please refer the app.config for more information. 
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the given day is a working day, false otherwise.</returns>
        public static bool IsAWorkingDay(this DateTime date)
        {
            return _workdaysList.Contains(date.DayOfWeek);
        }

        /// <summary>
        /// Checks whether the day of given DateTime is a holiday according to the configuration.
        /// Please refer the app.config for more information. 
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the given day is a holiday, false otherwise.</returns>
        public static bool IsAHoliday(this DateTime date)
        {
            return _holidaysList.Contains(date.DayOfWeek);
        }

        /// <summary>
        /// Checks whether the given day is Today.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the given day is Today, false otherwise.</returns>
        public static bool IsToday(this DateTime date)
        {
            return date.Date == DateTime.Now.Date;
        }

        /// <summary>
        /// Checks whether the given day is Tomorrow.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the given day is Tomorrow, false otherwise.</returns>
        public static bool IsTomorrow(this DateTime date)
        {
            return date.Date == DateTime.Now.Date.AddDays(1);
        }

        /// <summary>
        /// Checks whether the given day is Yesterday.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the given day is yesterday, false otherwise.</returns>
        public static bool IsYesterday(this DateTime date)
        {
            return date.Date == DateTime.Now.Date.AddDays(-1);
        }
    }
}