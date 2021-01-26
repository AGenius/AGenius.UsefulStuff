using System;
using System.Linq;

namespace AGenius.UsefulStuff
{
    public static partial class DateTimeXtensions
    {
        /// <summary>
        /// Gets the next Monday from the given DateTime.
        /// </summary>
        /// <param name="date">The starting DateTime.</param>
        /// <returns>DateTime instance of next Monday.</returns>
        public static DateTime GetNextMonday(this DateTime date)
        {
            return date.GetNextDay(DayOfWeek.Monday);
        }

        /// <summary>
        /// Gets the next Tuesday from the given DateTime.
        /// </summary>
        /// <param name="date">The starting DateTime.</param>
        /// <returns>DateTime instance of next Tuesday.</returns>
        public static DateTime GetNextTuesday(this DateTime date)
        {
            return date.GetNextDay(DayOfWeek.Tuesday);
        }

        /// <summary>
        /// Gets the next Wednesday from the given DateTime.
        /// </summary>
        /// <param name="date">The starting DateTime.</param>
        /// <returns>DateTime instance of next Wednesday.</returns>
        public static DateTime GetNextWednesday(this DateTime date)
        {
            return date.GetNextDay(DayOfWeek.Wednesday);
        }

        /// <summary>
        /// Gets the next Thursday from the given DateTime.
        /// </summary>
        /// <param name="date">The starting DateTime.</param>
        /// <returns>DateTime instance of next Thursday.</returns>
        public static DateTime GetNextThursday(this DateTime date)
        {
            return date.GetNextDay(DayOfWeek.Thursday);
        }

        /// <summary>
        /// Gets the next Friday from the given DateTime.
        /// </summary>
        /// <param name="date">The starting DateTime.</param>
        /// <returns>DateTime instance of next Friday.</returns>
        public static DateTime GetNextFriday(this DateTime date)
        {
            return date.GetNextDay(DayOfWeek.Friday);
        }

        /// <summary>
        /// Gets the next Saturday from the given DateTime.
        /// </summary>
        /// <param name="date">The starting DateTime.</param>
        /// <returns>DateTime instance of next Saturday.</returns>
        public static DateTime GetNextSaturday(this DateTime date)
        {
            return date.GetNextDay(DayOfWeek.Saturday);
        }

        /// <summary>
        /// Gets the next Sunday from the given DateTime.
        /// </summary>
        /// <param name="date">The starting DateTime.</param>
        /// <returns>DateTime instance of next Sunday.</returns>
        public static DateTime GetNextSunday(this DateTime date)
        {
            return date.GetNextDay(DayOfWeek.Sunday);
        }

        /// <summary>
        /// Gets the next given day from the given DateTime.
        /// </summary>
        /// <param name="date">The starting DateTime.</param>
        /// <param name="day">The day to be found out.</param>
        /// <returns>DateTime instance of the next given day.</returns>
        public static DateTime GetNextDay(this DateTime date, DayOfWeek day)
        {
            var givenDate = date;

            while (givenDate.DayOfWeek != day)
            {
                givenDate = givenDate.AddADay();
            }

            return givenDate;
        }

        /// <summary>
        /// Gets the next working day from the given DateTime. This is according to the holidays configured in the 
        /// app.config file.
        /// </summary>
        /// <param name="date">The starting DateTime.</param>
        /// <returns>DateTime instance of the next working day.</returns>
        public static DateTime GetNextWorkingDay(this DateTime date)
        {
            if (_workdaysList.Any())
            {
                var nextDate = date;
                do
                {
                    nextDate = nextDate.AddADay();
                }
                while (!_workdaysList.Contains(nextDate.DayOfWeek));

                return nextDate;
            }

            // if no workdays are configured, just return the given DateTime
            return date;
        }

        /// <summary>
        /// Gets the next holiday from the given DateTime. This is according to the holidays configured in the 
        /// app.config file.
        /// </summary>
        /// <param name="date">The starting DateTime.</param>
        /// <returns>DateTime instance of the next holiday.</returns>
        public static DateTime GetNextHoliday(this DateTime date)
        {
            if (_holidaysList.Any())
            {
                var nextDate = date;
                do
                {
                    nextDate = nextDate.AddADay();
                }
                while (!_holidaysList.Contains(nextDate.DayOfWeek));

                return nextDate;
            }

            // if no holidays are configured, just return the given DateTime
            return date;
        }
    }
}