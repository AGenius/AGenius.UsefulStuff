using System;
using System.Linq;

namespace AGenius.UsefulStuff
{
    public static partial class DateTimeXtensions
    {
        /// <summary>
        /// Gets the previous Monday from the given DateTime.
        /// </summary>
        /// <param name="date">The starting DateTime.</param>
        /// <returns>DateTime instance of previous Monday.</returns>
        public static DateTime GetPreviousMonday(this DateTime date)
        {
            return date.GetPreviousDay(DayOfWeek.Monday);
        }

        /// <summary>
        /// Gets the previous Tuesday from the given DateTime.
        /// </summary>
        /// <param name="date">The starting DateTime.</param>
        /// <returns>DateTime instance of previous Tuesday.</returns>
        public static DateTime GetPreviousTuesday(this DateTime date)
        {
            return date.GetPreviousDay(DayOfWeek.Tuesday);
        }

        /// <summary>
        /// Gets the previous Wednesday from the given DateTime.
        /// </summary>
        /// <param name="date">The starting DateTime.</param>
        /// <returns>DateTime instance of previous Wednesday.</returns>
        public static DateTime GetPreviousWednesday(this DateTime date)
        {
            return date.GetPreviousDay(DayOfWeek.Wednesday);
        }

        /// <summary>
        /// Gets the previous Thursday from the given DateTime.
        /// </summary>
        /// <param name="date">The starting DateTime.</param>
        /// <returns>DateTime instance of previous Thursday.</returns>
        public static DateTime GetPreviousThursday(this DateTime date)
        {
            return date.GetPreviousDay(DayOfWeek.Thursday);
        }

        /// <summary>
        /// Gets the previous Friday from the given DateTime.
        /// </summary>
        /// <param name="date">The starting DateTime.</param>
        /// <returns>DateTime instance of previous Friday.</returns>
        public static DateTime GetPreviousFriday(this DateTime date)
        {
            return date.GetPreviousDay(DayOfWeek.Friday);
        }

        /// <summary>
        /// Gets the previous Saturday from the given DateTime.
        /// </summary>
        /// <param name="date">The starting DateTime.</param>
        /// <returns>DateTime instance of previous Saturday.</returns>
        public static DateTime GetPreviousSaturday(this DateTime date)
        {
            return date.GetPreviousDay(DayOfWeek.Saturday);
        }

        /// <summary>
        /// Gets the previous Sunday from the given DateTime.
        /// </summary>
        /// <param name="date">The starting DateTime.</param>
        /// <returns>DateTime instance of previous Sunday.</returns>
        public static DateTime GetPreviousSunday(this DateTime date)
        {
            return date.GetPreviousDay(DayOfWeek.Sunday);
        }

        /// <summary>
        /// Gets the previous given day from the given DateTime.
        /// </summary>
        /// <param name="date">The starting DateTime.</param>
        /// <param name="day">The day to be found out.</param>
        /// <returns>DateTime instance of the previous given day.</returns>
        public static DateTime GetPreviousDay(this DateTime date, DayOfWeek day)
        {
            var givenDate = date;
            while (givenDate.DayOfWeek != day)
            {
                givenDate = givenDate.SubtractADay();
            }

            return givenDate;
        }

        /// <summary>
        /// Gets the previous working day from the given DateTime. This is according to the holidays configured in the 
        /// app.config file.
        /// </summary>
        /// <param name="date">The starting DateTime.</param>
        /// <returns>DateTime instance of the previous working day.</returns>
        public static DateTime GetPreviousWorkingDay(this DateTime date)
        {
            if (_workdaysList.Any())
            {
                var previousDate = date;
                do
                {
                    previousDate = previousDate.SubtractADay();
                }
                while (!_workdaysList.Contains(previousDate.DayOfWeek));

                return previousDate;
            }

            // if no workdays are configured, just return the given DateTime
            return date;
        }

        /// <summary>
        /// Gets the previous holiday from the given DateTime. This is according to the holidays configured in the 
        /// app.config file.
        /// </summary>
        /// <param name="date">The starting DateTime.</param>
        /// <returns>DateTime instance of the previous holiday.</returns>
        public static DateTime GetPreviousHoliday(this DateTime date)
        {
            if (_holidaysList.Any())
            {
                var previousDate = date;
                do
                {
                    previousDate = previousDate.SubtractADay();
                }
                while (!_holidaysList.Contains(previousDate.DayOfWeek));

                return previousDate;
            }

            // if no holidays are configured, just return the given DateTime
            return date;
        }
    }
}