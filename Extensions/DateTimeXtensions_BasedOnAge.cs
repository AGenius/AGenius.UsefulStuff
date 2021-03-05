using System;

namespace AGenius.UsefulStuff
{
    /// <summary>
    /// This class contains set of extension methods for the <see cref="DateTime"/> type.
    /// </summary>
    public static partial class DateTimeXtensions
    {
        #region Common

        /// <summary>
        /// Checks whether the given DateTime is an Adult age according to the configuration.
        /// Refer app.config file for more information.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the age is an adult age, false otherwise.</returns>
        public static bool IsAdult(this DateTime date)
        {
            return date.IsAtLeastOfAge(AdultAgeLimit);
        }

        /// <summary>
        /// Check whether the given DateTime is older than or equal to the given age.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <param name="age">Age to be checked.</param>
        /// <returns>True if the given DateTime is older than or equal to the given age, false otherwise.</returns>
        public static bool IsAtLeastOfAge(this DateTime date, int age)
        {
            return date.Date.SubtractYears(age).Ticks >= 0;
        }

        #endregion

        #region "Older than" methods

        /// <summary>
        /// Checks if the difference between the given DateTime and DateTime.Now is more than a second.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the difference is more than a second, false otherwise.</returns>
        public static bool IsOlderThanASecond(this DateTime date)
        {
            return date.Subtract(DateTime.Now).Seconds < 0;
        }

        /// <summary>
        /// Checks if the difference between the given DateTime and DateTime.Now is more than a minute.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the difference is more than a minute, false otherwise.</returns>          
        public static bool IsOlderThanAMinute(this DateTime date)
        {
            return date.Subtract(DateTime.Now).Minutes < 0;
        }

        /// <summary>
        /// Checks if the difference between the given DateTime and DateTime.Now is more than 1 hour.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the difference is more than 1 hour, false otherwise.</returns>          
        public static bool IsOlderThanAnHour(this DateTime date)
        {          
            return date.Subtract(DateTime.Now).Hours < 0;
        }

        /// <summary>
        /// Checks if the difference between the given DateTime and DateTime.Now is more than a day.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the difference is more than a day, false otherwise.</returns>          
        public static bool IsOlderThanADay(this DateTime date)
        {
            return date.Subtract(DateTime.Now).Days < 0;
        }

        /// <summary>
        /// Checks if the difference between the given DateTime and DateTime.Now is more than a week.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the difference is more than a week, false otherwise.</returns>          
        public static bool IsOlderThanAWeek(this DateTime date)
        {
            return date.Subtract(DateTime.Now).Days < Constants.DAYS_PER_WEEK;
        }

        /// <summary>
        /// Checks if the difference between the given DateTime and DateTime.Now is more than a fortnight.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the difference is more than a fortnight, false otherwise.</returns>          
        public static bool IsOlderThanAFortnight(this DateTime date)
        {
            return date.Subtract(DateTime.Now).Days < Constants.DAYS_PER_FORTNIGHT;
        }

        /// <summary>
        /// Checks if the difference between the given DateTime and DateTime.Now is more than a month.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the difference is more than a month, false otherwise.</returns>          
        public static bool IsOlderThanAMonth(this DateTime date)
        {
            var oneMonthOlderDate = DateTime.Now.AddMonths(-1);
            return date.IsOlderThan(oneMonthOlderDate);
        }

        /// <summary>
        /// Checks if the difference between the given DateTime and DateTime.Now is more than 6 months.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the difference is more than 6 months, false otherwise.</returns>          
        public static bool IsOlderThanHalfYear(this DateTime date)
        {
            var halfAYearOlderDate = DateTime.Now.AddMonths(-6);
            return date.IsOlderThan(halfAYearOlderDate);
        }

        /// <summary>
        /// Checks if the difference between the given DateTime and DateTime.Now is more than a year.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the difference is more than a year, false otherwise.</returns>          
        public static bool IsOlderThanAYear(this DateTime date)
        {
            var oneYearOlderDate = DateTime.Now.AddYears(-1);
            return date.IsOlderThan(oneYearOlderDate);
        }

        /// <summary>
        /// Checks if the difference between the given DateTime and DateTime.Now is more than a decade (10 years).
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the difference is more than a decade, false otherwise.</returns>          
        public static bool IsOlderThanADecade(this DateTime date)
        {
            var oneDecadeOlderDate = DateTime.Now.AddYears(Constants.YEARS_PER_DECADE.Negate());
            return date.IsOlderThan(oneDecadeOlderDate);
        }

        /// <summary>
        /// Checks if the difference between the given DateTime and DateTime.Now is more than a century.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the difference is more than a century, false otherwise.</returns>          
        public static bool IsOlderThanACentury(this DateTime date)
        {
            var oneCenturyOlderDate = DateTime.Now.AddYears(Constants.YEARS_PER_CENTURY.Negate());
            return date.IsOlderThan(oneCenturyOlderDate);
        }

        /// <summary>
        /// Checks whether the first date is older than the second date.
        /// </summary>
        /// <param name="firstDate">The first DateTime to be checked.</param>
        /// <param name="secondDate">The second DateTime to be checked.</param>
        /// <returns>True if first date is older, false otherwise.</returns>
        /// <example>Returns True for (new DateTime(2012,1,1)).IsOlderThan(new DateTime(2012,6,30)).</example>
        public static bool IsOlderThan(this DateTime firstDate, DateTime secondDate)
        {
            return firstDate.Subtract(secondDate).Ticks < 0;
        }

        #endregion

        #region "Younger than" methods

        /// <summary>
        /// Checks if the difference between DateTime.Now and the given DateTime is more than a second.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the difference is more than a second, false otherwise.</returns>          
        public static bool IsYoungerThanASecond(this DateTime date)
        {
            return date.Subtract(DateTime.Now).TotalSeconds > 0;
        }

        /// <summary>
        /// Checks if the difference between DateTime.Now and the given DateTime is more than a minute.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the difference is more than a minute, false otherwise.</returns>          
        public static bool IsYoungerThanAMinute(this DateTime date)
        {
            return date.Subtract(DateTime.Now).TotalMinutes > 0;
        }

        /// <summary>
        /// Checks if the difference between DateTime.Now and the given DateTime is more than an hour.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the difference is more than an hour, false otherwise.</returns>          
        public static bool IsYoungerThanAnHour(this DateTime date)
        {
            return date.Subtract(DateTime.Now).TotalHours > 0;
        }

        /// <summary>
        /// Checks if the difference between DateTime.Now and the given DateTime is more than a day.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the difference is more than a day, false otherwise.</returns>          
        public static bool IsYoungerThanADay(this DateTime date)
        {
            return date.Subtract(DateTime.Now).Days > 0;
        }

        /// <summary>
        /// Checks if the difference between DateTime.Now and the given DateTime is more than a week.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the difference is more than a week, false otherwise.</returns>          
        public static bool IsYoungerThanAWeek(this DateTime date)
        {
            var now = DateTime.Now;
            return date.Subtract(now).Days >= Constants.DAYS_PER_WEEK && date.IsYoungerThan(now);
        }

        /// <summary>
        /// Checks if the difference between DateTime.Now and the given DateTime is more than a fortnight.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the difference is more than a fortnight, false otherwise.</returns>          
        public static bool IsYoungerThanAFortnight(this DateTime date)
        {
            var now = DateTime.Now;
            return date.Subtract(now).Days >= Constants.DAYS_PER_FORTNIGHT && date.IsYoungerThan(now);
        }

        /// <summary>
        /// Checks if the difference between DateTime.Now and the given DateTime is more than a month.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the difference is more than a month, false otherwise.</returns>          
        public static bool IsYoungerThanAMonth(this DateTime date)
        {
            var oneMonthYoungerDate = DateTime.Now.AddMonths(1);
            return date.IsYoungerThan(oneMonthYoungerDate);
        }

        /// <summary>
        /// Checks if the difference between DateTime.Now and the given DateTime is more than 6 months.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the difference is more than 6 months, false otherwise.</returns>          
        public static bool IsYoungerThanHalfYear(this DateTime date)
        {
            var halfAYearYoungerDate = DateTime.Now.AddMonths(6);
            return date.IsYoungerThan(halfAYearYoungerDate);
        }

        /// <summary>
        /// Checks if the difference between DateTime.Now and the given DateTime is more than a year.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the difference is more than a year, false otherwise.</returns>          
        public static bool IsYoungerThanAYear(this DateTime date)
        {
            var oneYearYoungerDate = DateTime.Now.AddYears(1);
            return date.IsYoungerThan(oneYearYoungerDate);
        }

        /// <summary>
        /// Checks if the difference between DateTime.Now and the given DateTime is more than a decade.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the difference is more than a decade, false otherwise.</returns>          
        public static bool IsYoungerThanADecade(this DateTime date)
        {
            var oneDecadeYoungerDate = DateTime.Now.AddYears(Constants.YEARS_PER_DECADE);
            return date.IsYoungerThan(oneDecadeYoungerDate);
        }

        /// <summary>
        /// Checks if the difference between DateTime.Now and the given DateTime is more than a century.
        /// </summary>
        /// <param name="date">DateTime to be checked.</param>
        /// <returns>True if the difference is more than a century, false otherwise.</returns>          
        public static bool IsYoungerThanACentury(this DateTime date)
        {
            var oneCenturyYoungerDate = DateTime.Now.AddYears(Constants.YEARS_PER_CENTURY);
            return date.IsYoungerThan(oneCenturyYoungerDate);
        }

        /// <summary>
        /// Checks whether the first date falls after the second date.
        /// </summary>
        /// <param name="firstDate">The first DateTime to be checked.</param>
        /// <param name="secondDate">THe second DateTime to be checked.</param>
        /// <returns>True if the first date is younger, false otherwise.</returns>
        public static bool IsYoungerThan(this DateTime firstDate, DateTime secondDate)
        {
            return firstDate.Subtract(secondDate).Ticks > 0;
        }

        #endregion

        #region "Add methods"

        /// <summary>
        /// Adds one second to the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The DateTime after one second is added.</returns>
        public static DateTime AddASecond(this DateTime date)
        {
            return date.AddSeconds(1);
        }

        /// <summary>
        /// Adds one minute to the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The DateTime after one minute is added.</returns>
        public static DateTime AddAMinute(this DateTime date)
        {
            return date.AddMinutes(1);
        }

        /// <summary>
        /// Adds 30 minutes to the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The DateTime after 30 minutes are added.</returns>
        public static DateTime AddHalfAnHour(this DateTime date)
        {
            return date.AddMinutes(30);
        }

        /// <summary>
        /// Adds one hour to the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The DateTime after one hour is added.</returns>
        public static DateTime AddAnHour(this DateTime date)
        {
            return date.AddHours(1);
        }

        /// <summary>
        /// Adds one day to the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The DateTime after one day is added.</returns>
        public static DateTime AddADay(this DateTime date)
        {
            return date.AddDays(1);
        }

        /// <summary>
        /// Adds one week to the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The DateTime after one week is added.</returns>
        public static DateTime AddAWeek(this DateTime date)
        {
            return date.AddWeeks(1);
        }

        /// <summary>
        /// Adds one fortnight to the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The DateTime after one fortnight is added.</returns>
        public static DateTime AddAFortnight(this DateTime date)
        {
            return date.AddFortnights(1);
        }

        /// <summary>
        /// Adds one month to the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The DateTime after one month is added.</returns>
        public static DateTime AddAMonth(this DateTime date)
        {
            return date.AddMonths(1);
        }

        /// <summary>
        /// Adds one year to the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The DateTime after one year is added.</returns>
        public static DateTime AddAYear(this DateTime date)
        {
            return date.AddYears(1);
        }

        /// <summary>
        /// Adds one decade to the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The DateTime after one decade is added.</returns>
        public static DateTime AddADecade(this DateTime date)
        {
            return date.AddDecades(1);
        }

        /// <summary>
        /// Adds the given weeks to the given DateTime.
        /// </summary>
        /// <param name="date">The DateTime to which the weeks have to be added.</param>
        /// <param name="weeks">The number of weeks to be added.</param>
        /// <returns>The DateTime after the weeks are added.</returns>
        public static DateTime AddWeeks(this DateTime date, int weeks)
        {
            return date.AddDays(weeks * Constants.DAYS_PER_WEEK);
        }

        /// <summary>
        /// Adds the given fortnights to the given DateTime.
        /// </summary>
        /// <param name="date">The DateTime to which the fortnights have to be added.</param>
        /// <param name="fortnights">The number of fortnights to be added.</param>
        /// <returns>The DateTime after the fortnights are added.</returns>
        public static DateTime AddFortnights(this DateTime date, int fortnights)
        {
            return date.AddDays(fortnights * 14);
        }

        /// <summary>
        /// Adds one century to the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The DateTime after one century is added.</returns>
        public static DateTime AddACentury(this DateTime date)
        {
            return date.AddCenturies(1);
        }

        /// <summary>
        /// Adds the given decades to the given DateTime.
        /// </summary>
        /// <param name="date">The DateTime to which the decades have to be added.</param>
        /// <param name="decades">The number of decades to be added.</param>
        /// <returns>The DateTime after the decades are added.</returns>
        public static DateTime AddDecades(this DateTime date, int decades)
        {
            return date.AddYears(decades * Constants.YEARS_PER_DECADE);
        }

        /// <summary>
        /// Adds the given centuries to the given DateTime.
        /// </summary>
        /// <param name="date">The DateTime to which the centuries have to be added.</param>
        /// <param name="centuries">The number of centuries to be added.</param>
        /// <returns>The DateTime after the centuries are added.</returns>
        public static DateTime AddCenturies(this DateTime date, int centuries)
        {
            return date.AddYears(centuries * Constants.YEARS_PER_CENTURY);
        }
        #endregion

        #region Subtract methods

        /// <summary>
        /// Subtracts one second from the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The DateTime after one second is subtracted.</returns>
        public static DateTime SubtractASecond(this DateTime date)
        {
            return date.SubtractSeconds(1);
        }

        /// <summary>
        /// Subtracts one minute from the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The DateTime after one minute is subtracted.</returns>
        public static DateTime SubtractAMinute(this DateTime date)
        {
            return date.SubtractMinutes(1);
        }

        /// <summary>
        /// Subtracts 30 minutes from the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The DateTime after 30 minutes are subtracted.</returns>
        public static DateTime SubtractHalfAnHour(this DateTime date)
        {
            return date.SubtractMinutes(30);
        }

        /// <summary>
        /// Subtracts one hour from the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The DateTime after one hour is subtracted.</returns>
        public static DateTime SubtractAnHour(this DateTime date)
        {
            return date.SubtractHours(1);
        }

        /// <summary>
        /// Subtracts one day from the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The DateTime after one day is subtracted.</returns>
        public static DateTime SubtractADay(this DateTime date)
        {
            return date.SubtractDays(1);
        }

        /// <summary>
        /// Subtracts one week from the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The DateTime after one week is subtracted.</returns>
        public static DateTime SubtractAWeek(this DateTime date)
        {
            return date.SubtractWeeks(1);
        }

        /// <summary>
        /// Subtracts one fortnight from the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The DateTime after one fortnight is subtracted.</returns>
        public static DateTime SubtractAFortnight(this DateTime date)
        {
            return date.SubtractFortnights(1);
        }

        /// <summary>
        /// Subtracts one month from the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The DateTime after one month is subtracted.</returns>
        public static DateTime SubtractAMonth(this DateTime date)
        {
            return date.SubtractMonths(1);
        }

        /// <summary>
        /// Subtracts one year from the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The DateTime after one year is subtracted.</returns>
        public static DateTime SubtractAYear(this DateTime date)
        {
            return date.SubtractYears(1);
        }

        /// <summary>
        /// Subtracts one decade from the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The DateTime after one decade is subtracted.</returns>
        public static DateTime SubtractADecade(this DateTime date)
        {
            return date.SubtractDecades(1);
        }

        /// <summary>
        /// Subtracts one century from the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <returns>The DateTime after one century is subtracted.</returns>
        public static DateTime SubtractACentury(this DateTime date)
        {
            return date.SubtractCenturies(1);
        }

        /// <summary>
        /// Subtracts given ticks from the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="ticks">Number of ticks to be subtracted.</param>
        /// <returns>The DateTime after the given ticks are subtracted.</returns>
        public static DateTime SubtractTicks(this DateTime date, int ticks)
        {
            return date.AddTicks(ticks.Negate());
        }

        /// <summary>
        /// Subtracts given milliseconds from the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="milliSeconds">Number of milliseconds to be subtracted.</param>
        /// <returns>The DateTime after the given milliseconds are subtracted.</returns>
        public static DateTime SubtractMilliSeconds(this DateTime date, int milliSeconds)
        {
            return date.AddMilliseconds(milliSeconds.Negate());
        }

        /// <summary>
        /// Subtracts given seconds from the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="seconds">Number of seconds to be subtracted.</param>
        /// <returns>The DateTime after the given seconds are subtracted.</returns>
        public static DateTime SubtractSeconds(this DateTime date, int seconds)
        {
            return date.AddSeconds(seconds.Negate());
        }

        /// <summary>
        /// Subtracts given minutes from the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="minutes">Number of minutes to be subtracted.</param>
        /// <returns>The DateTime after the given minutes are subtracted.</returns>
        public static DateTime SubtractMinutes(this DateTime date, int minutes)
        {
            return date.AddMinutes(minutes.Negate());
        }

        /// <summary>
        /// Subtracts given hours from the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="hours">Number of hours to be subtracted.</param>
        /// <returns>The DateTime after the given hours are subtracted.</returns>
        public static DateTime SubtractHours(this DateTime date, int hours)
        {
            return date.AddHours(hours.Negate());
        }

        /// <summary>
        /// Subtracts given days from the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="days">Number of days to be subtracted.</param>
        /// <returns>The DateTime after the given days are subtracted.</returns>
        public static DateTime SubtractDays(this DateTime date, int days)
        {
            return date.AddDays(days.Negate());
        }

        /// <summary>
        /// Subtracts given weeks from the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="weeks">Number of weeks to be subtracted.</param>
        /// <returns>The DateTime after the given weeks are subtracted.</returns>
        public static DateTime SubtractWeeks(this DateTime date, int weeks)
        {
            return date.AddWeeks(weeks.Negate());
        }

        /// <summary>
        /// Subtracts given fortnights from the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="fortnights">Number of fortnights to be subtracted.</param>
        /// <returns>The DateTime after the given fortnights are subtracted.</returns>
        public static DateTime SubtractFortnights(this DateTime date, int fortnights)
        {
            return date.AddFortnights(fortnights.Negate());
        }

        /// <summary>
        /// Subtracts given months from the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="months">Number of months to be subtracted.</param>
        /// <returns>The DateTime after the given months are subtracted.</returns>          
        public static DateTime SubtractMonths(this DateTime date, int months)
        {
            return date.AddMonths(months.Negate());
        }

        /// <summary>
        /// Subtracts given years from the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="years">Number of years to be subtracted.</param>
        /// <returns>The DateTime after the given years are subtracted.</returns>
        public static DateTime SubtractYears(this DateTime date, int years)
        {
            return date.AddYears(years.Negate());
        }

        /// <summary>
        /// Subtracts given decades from the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="decades">Number of decades to be subtracted.</param>
        /// <returns>The DateTime after the given decades are subtracted.</returns>
        public static DateTime SubtractDecades(this DateTime date, int decades)
        {
            return date.AddDecades(decades.Negate());
        }

        /// <summary>
        /// Subtracts given centuries from the given DateTime.
        /// </summary>
        /// <param name="date">The given DateTime.</param>
        /// <param name="centuries">Number of centuries to be subtracted.</param>
        /// <returns>The DateTime after the given centuries are subtracted.</returns>
        public static DateTime SubtractCenturies(this DateTime date, int centuries)
        {
            return date.AddCenturies(centuries.Negate());
        }

        #endregion

        #region "Get Duration since" methods

        /// <summary>
        /// Gets the ticks between the given time and DateTime.Now.
        /// </summary>
        /// <param name="time">The given DateTime.</param>
        /// <returns>Number of ticks.</returns>              
        public static long GetTicksSince(this DateTime time)
        {
            DateTime now = DateTime.Now;

            var ticks = now.Subtract(time).Ticks;
            return ticks.AbsoluteValue();
        }

        /// <summary>
        /// Gets the milliseconds between the given time and DateTime.Now.
        /// </summary>
        /// <param name="time">The given DateTime.</param>
        /// <returns>Number of milliseconds.</returns>
        public static long GetMilliSecondsSince(this DateTime time)
        {
            DateTime now = DateTime.Now;

            var ticks = now.Subtract(time).Milliseconds;
            return ticks.AbsoluteValue();
        }

        /// <summary>
        /// Gets the seconds between the given time and DateTime.Now.
        /// </summary>
        /// <param name="time">The given DateTime.</param>
        /// <returns>Number of seconds.</returns>
        public static long GetSecondsSince(this DateTime time)
        {
            DateTime now = DateTime.Now;

            var ticks = now.Subtract(time).Seconds;
            return ticks.AbsoluteValue();
        }

        /// <summary>
        /// Gets the minutes between the given time and DateTime.Now.
        /// </summary>
        /// <param name="time">The given DateTime.</param>
        /// <returns>Number of minutes.</returns>
        public static long GetMinutesSince(this DateTime time)
        {
            DateTime now = DateTime.Now;

            var ticks = now.Subtract(time).Minutes;
            return ticks.AbsoluteValue();
        }

        /// <summary>
        /// Gets the hours between the given time and DateTime.Now.
        /// </summary>
        /// <param name="time">The given DateTime.</param>
        /// <returns>Number of hours.</returns>
        public static long GetHoursSince(this DateTime time)
        {
            DateTime now = DateTime.Now;

            var ticks = now.Subtract(time).Hours;
            return ticks.AbsoluteValue();
        }

        /// <summary>
        /// Gets the days between the given time and DateTime.Now.
        /// </summary>
        /// <param name="time">The given DateTime.</param>
        /// <returns>Number of days.</returns>
        public static long GetDaysSince(this DateTime time)
        {
            DateTime now = DateTime.Now;

            var ticks = now.Subtract(time).Days;
            return ticks.AbsoluteValue();
        }

        /// <summary>
        /// Gets the weeks between the given time and DateTime.Now.
        /// </summary>
        /// <param name="time">The given DateTime.</param>
        /// <returns>Number of weeks.</returns>
        public static long GetWeeksSince(this DateTime time)
        {
            DateTime now = DateTime.Now;

            var ticks = now.Subtract(time).GetWeeks();
            return ticks.AbsoluteValue();
        }

        /// <summary>
        /// Gets the fortnights between the given time and DateTime.Now.
        /// </summary>
        /// <param name="time">The given DateTime.</param>
        /// <returns>Number of fortnights.</returns>
        public static long GetFortnightsSince(this DateTime time)
        {
            DateTime now = DateTime.Now;

            var ticks = now.Subtract(time).GetFortnights();
            return ticks.AbsoluteValue();
        }

        /// <summary>
        /// Gets the months between the given time and DateTime.Now.
        /// </summary>
        /// <param name="time">The given DateTime.</param>
        /// <returns>Number of months.</returns>
        public static long GetMonthsSince(this DateTime time)
        {
            DateTime now = DateTime.Now;
            long months = -1;

            if (time.IsYoungerThan(now))
            {
                // Date lies in future so try adding months and see
                do
                {
                    months++;
                    now = now.AddMonths(1);
                }
                while (now.IsOlderThan(time));
            }
            else
            {
                // Date is in past, so try subtracting months and see
                do
                {
                    months++;
                    now = now.SubtractMonths(1);
                }
                while (now.IsYoungerThan(time));
            }

            return months;
        }

        /// <summary>
        /// Gets the years between the given time and DateTime.Now.
        /// </summary>
        /// <param name="time">The given DateTime.</param>
        /// <returns>Number of years.</returns>
        public static long GetYearsSince(this DateTime time)
        {
            DateTime now = DateTime.Now;
            long years = -1;

            if (time.IsYoungerThan(now))
            {
                // Date lies in future so try adding years and see
                do
                {
                    years++;
                    now = now.AddYears(1);
                }
                while (now.IsOlderThan(time));
            }
            else
            {
                // Date is in past, so try subtracting years and see
                do
                {
                    years++;
                    now = now.SubtractYears(1);
                }
                while (now.IsYoungerThan(time));
            }

            return years;
        }

        /// <summary>
        /// Gets the decades between the given time and DateTime.Now.
        /// </summary>
        /// <param name="time">The given DateTime.</param>
        /// <returns>Number of decades.</returns>
        public static long GetDecadesSince(this DateTime time)
        {
            var years = time.GetYearsSince();
            return (years / Constants.YEARS_PER_DECADE).AbsoluteValue();
        }

        /// <summary>
        /// Gets the centuries between the given time and DateTime.Now.
        /// </summary>
        /// <param name="time">The given DateTime.</param>
        /// <returns>Number of centuries.</returns>
        public static long GetCenturiesSince(this DateTime time)
        {
            var years = time.GetYearsSince();
            return (years / Constants.YEARS_PER_CENTURY).AbsoluteValue();
        }

        #endregion
    }

    /// <summary>
    /// This Namespace contains set of extension methods for <see cref="DateTime"/> class.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }
}