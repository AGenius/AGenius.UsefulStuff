using System;

namespace AGenius.UsefulStuff
{
    public static partial class DateTimeXtensions
    {
        /// <summary>
        /// Checks whether the month of the given day is January.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the month of given day is January, False otherwise.</returns>
        public static bool IsInJanuary(this DateTime date)
        {
            return date.Month.Equals(Constants.JANUARY);
        }

        /// <summary>
        /// Checks whether the month of the given DateTime is February.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the month of given day is February, False otherwise.</returns>
        public static bool IsInFebruary(this DateTime date)
        {
            return date.Month.Equals(Constants.FEBRUARY);
        }

        /// <summary>
        /// Checks whether the month of the given DateTime is March.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the month of given day is March, False otherwise.</returns>
        public static bool IsInMarch(this DateTime date)
        {
            return date.Month.Equals(Constants.MARCH);
        }

        /// <summary>
        /// Checks whether the month of the given DateTime is April.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the month of given day is April, False otherwise.</returns>
        public static bool IsInApril(this DateTime date)
        {
            return date.Month.Equals(Constants.APRIL);
        }

        /// <summary>
        /// Checks whether the month of the given DateTime is May.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the month of given day is May, False otherwise.</returns>
        public static bool IsInMay(this DateTime date)
        {
            return date.Month.Equals(Constants.MAY);
        }

        /// <summary>
        /// Checks whether the month of the given DateTime is June.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the month of given day is June, False otherwise.</returns>
        public static bool IsInJune(this DateTime date)
        {
            return date.Month.Equals(Constants.JUNE);
        }

        /// <summary>
        /// Checks whether the month of the given DateTime is July.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the month of given day is July, False otherwise.</returns>
        public static bool IsInJuly(this DateTime date)
        {
            return date.Month.Equals(Constants.JULY);
        }

        /// <summary>
        /// Checks whether the month of the given DateTime is August.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the month of given day is August, False otherwise.</returns>
        public static bool IsInAugust(this DateTime date)
        {
            return date.Month.Equals(Constants.AUGUST);
        }

        /// <summary>
        /// Checks whether the month of the given DateTime is September.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the month of given day is January, False otherwise.</returns>
        public static bool IsInSeptember(this DateTime date)
        {
            return date.Month.Equals(Constants.SEPTEMBER);
        }

        /// <summary>
        /// Checks whether the month of the given DateTime is October.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the month of given day is October, False otherwise.</returns>
        public static bool IsInOctober(this DateTime date)
        {
            return date.Month.Equals(Constants.OCTOBER);
        }

        /// <summary>
        /// Checks whether the month of the given DateTime is November.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the month of given day is November, False otherwise.</returns>
        public static bool IsInNovember(this DateTime date)
        {
            return date.Month.Equals(Constants.NOVEMBER);
        }

        /// <summary>
        /// Checks whether the month of the given DateTime is December.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the month of given day is December, False otherwise.</returns>
        public static bool IsInDecember(this DateTime date)
        {
            return date.Month.Equals(Constants.DECEMBER);
        }
    }
}