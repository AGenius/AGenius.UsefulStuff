using System;

namespace AGenius.UsefulStuff
{
    /// <summary>
    /// This class contains extension methods for <see cref="TimeSpan"/>.
    /// </summary>
    public static class TimeSpanXtensions
    {
        /// <summary>
        /// Gets the number of weeks in the given time span.
        /// </summary>
        /// <param name="span">The given time span.</param>
        /// <returns>The number of weeks.</returns>
        public static int GetWeeks(this TimeSpan span)
        {
            return span.Days / Constants.DAYS_PER_WEEK;
        }

        /// <summary>
        /// Gets the number of fortnights in the given time span.
        /// </summary>
        /// <param name="span">The given time span.</param>
        /// <returns>The number of fortnights.</returns>          
        public static int GetFortnights(this TimeSpan span)
        {
            return span.GetWeeks() / Constants.WEEKS_PER_FORTNIGHT;
        }
        /// <summary>
        /// Gets a nicely formatted string representing the time span
        /// </summary>
        /// <param name="span">The given time span.</param>             
        public static string GetNiceTime(this TimeSpan span)
        {
            string answer;
            if (span.TotalMinutes < 1.0)
            {
                answer = String.Format("{0}s", span.Seconds);
            }
            else if (span.TotalHours < 1.0)
            {
                answer = String.Format("{0}m:{1:D2}s", span.Minutes, span.Seconds);
            }
            else // more than 1 hour
            {
                answer = String.Format("{0}h:{1:D2}m:{2:D2}s", (int)span.TotalHours, span.Minutes, span.Seconds);
            }

            return answer;

        }
    }
}