using System;

namespace AGenius.UsefulStuff
{

    public static partial class DateTimeXtensions
    {
        /// <summary>
        /// Gets the first occurrence of the given day of the year in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="day">The day to be found.</param>
        /// <returns>DateTime instance of the first occurrence of the given day.</returns>
        public static DateTime GetFirstDayOccurrenceOfTheYear(this DateTime date, DayOfWeek day)
        {
            var firstDayOfYear = date.GetFirstDayOfTheYear();
            return firstDayOfYear.DayOfWeek == day ? firstDayOfYear : firstDayOfYear.GetNextDay(day);
        }

        /// <summary>
        /// Gets the first Monday of the year in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the first Monday of the given year.</returns>
        public static DateTime GetFirstMondayOfTheYear(this DateTime date)
        {
            return date.GetFirstDayOccurrenceOfTheYear(DayOfWeek.Monday);
        }

        /// <summary>
        /// Gets the first Tuesday of the year in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the first Tuesday of the given year.</returns>
        public static DateTime GetFirstTuesdayOfTheYear(this DateTime date)
        {
            return date.GetFirstDayOccurrenceOfTheYear(DayOfWeek.Tuesday);
        }

        /// <summary>
        /// Gets the first Wednesday of the year in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the first Wednesday of the given year.</returns>
        public static DateTime GetFirstWednesdayOfTheYear(this DateTime date)
        {
            return date.GetFirstDayOccurrenceOfTheYear(DayOfWeek.Wednesday);
        }

        /// <summary>
        /// Gets the first Thursday of the year in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the first Thursday of the given year.</returns>
        public static DateTime GetFirstThursdayOfTheYear(this DateTime date)
        {
            return date.GetFirstDayOccurrenceOfTheYear(DayOfWeek.Thursday);
        }

        /// <summary>
        /// Gets the first Friday of the year in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the first Friday of the given year.</returns>
        public static DateTime GetFirstFridayOfTheYear(this DateTime date)
        {
            return date.GetFirstDayOccurrenceOfTheYear(DayOfWeek.Friday);
        }

        /// <summary>
        /// Gets the first Saturday of the year in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the first Saturday of the given year.</returns>
        public static DateTime GetFirstSaturdayOfTheYear(this DateTime date)
        {
            return date.GetFirstDayOccurrenceOfTheYear(DayOfWeek.Saturday);
        }

        /// <summary>
        /// Gets the first Sunday of the year in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the first Sunday of the given year.</returns>
        public static DateTime GetFirstSundayOfTheYear(this DateTime date)
        {
            return date.GetFirstDayOccurrenceOfTheYear(DayOfWeek.Sunday);
        }

        /// <summary>
        /// Gets the first configured Holiday of the year of the given DateTime. Refer app.config
        /// for holiday configuration.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the first Holiday.</returns>
        public static DateTime GetFirstHolidayOfTheYear(this DateTime date)
        {
            var firstDayOfTheYear = date.GetFirstDayOfTheYear();
            return firstDayOfTheYear.IsAHoliday() ? firstDayOfTheYear : firstDayOfTheYear.GetNextHoliday();
        }

        /// <summary>
        /// Gets the first configured working day of the year of the given DateTime. Refer app.config
        /// for working days configuration.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the first working day.</returns>
        public static DateTime GetFirstWorkingDayOfTheYear(this DateTime date)
        {
            var firstDayOfTheYear = date.GetFirstDayOfTheYear();
            return firstDayOfTheYear.IsAWorkingDay() ? firstDayOfTheYear : firstDayOfTheYear.GetNextWorkingDay();
        }

        /// <summary>
        /// Gets the first day of the year of the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the first day of the year.</returns>
        public static DateTime GetFirstDayOfTheYear(this DateTime date)
        {
            return new DateTime(date.Year, 1, 1);
        }

        /// <summary>
        /// Gets the last day of the year of the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the last day of the year.</returns>
        public static DateTime GetLastDayOfTheYear(this DateTime date)
        {
            return new DateTime(date.Year, 12, 31);
        }

        /// <summary>
        /// Gets the last occurrence of the given day of the year in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="day">The day to be found.</param>
        /// <returns>DateTime instance of the last occurrence of the given day.</returns>
        public static DateTime GetLastDayOccurrenceOfTheYear(this DateTime date, DayOfWeek day)
        {
            var lastDayOfYear = date.GetLastDayOfTheYear();
            return lastDayOfYear.DayOfWeek == day ? lastDayOfYear : lastDayOfYear.GetPreviousDay(day);
        }

        /// <summary>
        /// Gets the last Monday of the year in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the last Monday of the given year.</returns>
        public static DateTime GetLastMondayOfTheYear(this DateTime date)
        {
            return date.GetLastDayOccurrenceOfTheYear(DayOfWeek.Monday);
        }

        /// <summary>
        /// Gets the last Tuesday of the year in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the last Tuesday of the given year.</returns>
        public static DateTime GetLastTuesdayOfTheYear(this DateTime date)
        {
            return date.GetLastDayOccurrenceOfTheYear(DayOfWeek.Tuesday);
        }

        /// <summary>
        /// Gets the last Wednesday of the year in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the last Wednesday of the given year.</returns>
        public static DateTime GetLastWednesdayOfTheYear(this DateTime date)
        {
            return date.GetLastDayOccurrenceOfTheYear(DayOfWeek.Wednesday);
        }

        /// <summary>
        /// Gets the last Thursday of the year in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the last Thursday of the given year.</returns>
        public static DateTime GetLastThursdayOfTheYear(this DateTime date)
        {
            return date.GetLastDayOccurrenceOfTheYear(DayOfWeek.Thursday);
        }

        /// <summary>
        /// Gets the last Friday of the year in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the last Friday of the given year.</returns>
        public static DateTime GetLastFridayOfTheYear(this DateTime date)
        {
            return date.GetLastDayOccurrenceOfTheYear(DayOfWeek.Friday);
        }

        /// <summary>
        /// Gets the last Saturday of the year in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the last Saturday of the given year.</returns>
        public static DateTime GetLastSaturdayOfTheYear(this DateTime date)
        {
            return date.GetLastDayOccurrenceOfTheYear(DayOfWeek.Saturday);
        }

        /// <summary>
        /// Gets the last Sunday of the year in the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the last Sunday of the given year.</returns>
        public static DateTime GetLastSundayOfTheYear(this DateTime date)
        {
            return date.GetLastDayOccurrenceOfTheYear(DayOfWeek.Sunday);
        }

        /// <summary>
        /// Gets the last configured holiday of the year of the given DateTime. Refer app.config file
        /// for holidays configuration.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the last holiday of the year.</returns>
        public static DateTime GetLastHolidayOfTheYear(this DateTime date)
        {
            var lastDayOfTheYear = date.GetLastDayOfTheYear();
            return lastDayOfTheYear.IsAHoliday() ? lastDayOfTheYear : lastDayOfTheYear.GetPreviousHoliday();
        }

        /// <summary>
        /// Gets the last configured working day of the year of the given DateTime. Refer app.config file
        /// for working days configuration.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>DateTime instance of the last working day of the year.</returns>
        public static DateTime GetLastWorkingDayOfTheYear(this DateTime date)
        {
            var lastDayOfTheYear = date.GetLastDayOfTheYear();
            return lastDayOfTheYear.IsAWorkingDay() ? lastDayOfTheYear : lastDayOfTheYear.GetPreviousWorkingDay();
        }
    }
}