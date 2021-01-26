using System;
namespace AGenius.UsefulStuff
{
    public static partial class DateTimeXtensions
    {
        /// <summary>
        /// Gets the first occurrence of the given day of the month in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="day">The day to be found.</param>
        /// <returns>DateTime instance of the first occurrence of the given day.</returns>
        public static DateTime GetFirstDayOccurrenceOfTheMonth(this DateTime date, DayOfWeek day)
        {
            var firstDayOfMonth = date.GetFirstDayOfTheMonth();
            return firstDayOfMonth.DayOfWeek == day ? firstDayOfMonth : firstDayOfMonth.GetNextDay(day);
        }

        /// <summary>
        /// Gets the first Monday of the month in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the first Monday of the given month.</returns>
        public static DateTime GetFirstMondayOfTheMonth(this DateTime date)
        {
            return date.GetFirstDayOccurrenceOfTheMonth(DayOfWeek.Monday);
        }

        /// <summary>
        /// Gets the first Tuesday of the month in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the first Tuesday of the given month.</returns>
        public static DateTime GetFirstTuesdayOfTheMonth(this DateTime date)
        {
            return date.GetFirstDayOccurrenceOfTheMonth(DayOfWeek.Tuesday);
        }

        /// <summary>
        /// Gets the first Wednesday of the month in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the first Wednesday of the given month.</returns>
        public static DateTime GetFirstWednesdayOfTheMonth(this DateTime date)
        {
            return date.GetFirstDayOccurrenceOfTheMonth(DayOfWeek.Wednesday);
        }

        /// <summary>
        /// Gets the first Thursday of the month in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the first Thursday of the given month.</returns>
        public static DateTime GetFirstThursdayOfTheMonth(this DateTime date)
        {
            return date.GetFirstDayOccurrenceOfTheMonth(DayOfWeek.Thursday);
        }

        /// <summary>
        /// Gets the first Friday of the month in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the first Friday of the given month.</returns>
        public static DateTime GetFirstFridayOfTheMonth(this DateTime date)
        {
            return date.GetFirstDayOccurrenceOfTheMonth(DayOfWeek.Friday);
        }

        /// <summary>
        /// Gets the first Saturday of the month in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the first Saturday of the given month.</returns>
        public static DateTime GetFirstSaturdayOfTheMonth(this DateTime date)
        {
            return date.GetFirstDayOccurrenceOfTheMonth(DayOfWeek.Saturday);
        }

        /// <summary>
        /// Gets the first Sunday of the month in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the first Sunday of the given month.</returns>
        public static DateTime GetFirstSundayOfTheMonth(this DateTime date)
        {
            return date.GetFirstDayOccurrenceOfTheMonth(DayOfWeek.Sunday);
        }

        /// <summary>
        /// Gets the first configured Holiday of the month of the given DateTime. Refer app.config
        /// for holiday configuration.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the first Holiday.</returns>
        public static DateTime GetFirstHolidayOfTheMonth(this DateTime date)
        {
            var firstDayOfTheMonth = date.GetFirstDayOfTheMonth();
            return firstDayOfTheMonth.IsAHoliday() ? firstDayOfTheMonth : firstDayOfTheMonth.GetNextHoliday();
        }

        /// <summary>
        /// Gets the first configured working day of the month of the given DateTime. Refer app.config
        /// for working days configuration.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the first working day.</returns>
        public static DateTime GetFirstWorkingDayOfTheMonth(this DateTime date)
        {
            var firstDayOfTheMonth = date.GetFirstDayOfTheMonth();
            return firstDayOfTheMonth.IsAWorkingDay() ? firstDayOfTheMonth : firstDayOfTheMonth.GetNextWorkingDay();
        }

        /// <summary>
        /// Gets the first day of the month of the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the first day of the month.</returns>
        public static DateTime GetFirstDayOfTheMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        /// <summary>
        /// Gets the last day of the month of the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the last day of the month.</returns>
        public static DateTime GetLastDayOfTheMonth(this DateTime date)
        {
            return date.GetFirstDayOfTheMonth().AddAMonth().SubtractADay();
        }

        /// <summary>
        /// Gets the last occurrence of the given day of the month in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="day">The day to be found.</param>
        /// <returns>DateTime instance of the last occurrence of the given day.</returns>
        public static DateTime GetLastDayOccurrenceOfTheMonth(this DateTime date, DayOfWeek day)
        {
            var lastDayOfMonth = date.GetLastDayOfTheMonth();
            return lastDayOfMonth.DayOfWeek == day ? lastDayOfMonth : lastDayOfMonth.GetPreviousDay(day);
        }

        /// <summary>
        /// Gets the last Monday of the month in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the last Monday of the given month.</returns>
        public static DateTime GetLastMondayOfTheMonth(this DateTime date)
        {
            return date.GetLastDayOccurrenceOfTheMonth(DayOfWeek.Monday);
        }

        /// <summary>
        /// Gets the last Tuesday of the month in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the last Tuesday of the given month.</returns>
        public static DateTime GetLastTuesdayOfTheMonth(this DateTime date)
        {
            return date.GetLastDayOccurrenceOfTheMonth(DayOfWeek.Tuesday);
        }

        /// <summary>
        /// Gets the last Wednesday of the month in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the last Wednesday of the given month.</returns>
        public static DateTime GetLastWednesdayOfTheMonth(this DateTime date)
        {
            return date.GetLastDayOccurrenceOfTheMonth(DayOfWeek.Wednesday);
        }

        /// <summary>
        /// Gets the last Thursday of the month in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the last Thursday of the given month.</returns>
        public static DateTime GetLastThursdayOfTheMonth(this DateTime date)
        {
            return date.GetLastDayOccurrenceOfTheMonth(DayOfWeek.Thursday);
        }

        /// <summary>
        /// Gets the last Friday of the month in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the last Friday of the given month.</returns>
        public static DateTime GetLastFridayOfTheMonth(this DateTime date)
        {
            return date.GetLastDayOccurrenceOfTheMonth(DayOfWeek.Friday);
        }

        /// <summary>
        /// Gets the last Saturday of the month in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the last Saturday of the given month.</returns>
        public static DateTime GetLastSaturdayOfTheMonth(this DateTime date)
        {
            return date.GetLastDayOccurrenceOfTheMonth(DayOfWeek.Saturday);
        }

        /// <summary>
        /// Gets the last Sunday of the month in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the last Sunday of the given month.</returns>
        public static DateTime GetLastSundayOfTheMonth(this DateTime date)
        {
            return date.GetLastDayOccurrenceOfTheMonth(DayOfWeek.Sunday);
        }

        /// <summary>
        /// Gets the last configured holiday of the month of the given DateTime. Refer app.config file
        /// for holidays configuration.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the last holiday of the month.</returns>
        public static DateTime GetLastHolidayOfTheMonth(this DateTime date)
        {
            var lastDayOfTheMonth = date.GetLastDayOfTheMonth();
            return lastDayOfTheMonth.IsAHoliday() ? lastDayOfTheMonth : lastDayOfTheMonth.GetPreviousHoliday();
        }

        /// <summary>
        /// Gets the last configured working day of the month of the given DateTime. Refer app.config file
        /// for working days configuration.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the last working day of the month.</returns>
        public static DateTime GetLastWorkingDayOfTheMonth(this DateTime date)
        {
            var lastDayOfTheMonth = date.GetLastDayOfTheMonth();
            return lastDayOfTheMonth.IsAWorkingDay() ? lastDayOfTheMonth : lastDayOfTheMonth.GetPreviousWorkingDay();
        }
    }
}