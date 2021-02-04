using System;
using System.Collections.Generic;

namespace AGenius.UsefulStuff
{
    /// <summary>
    /// This class contains extension methods for <see cref="Int64"/>.
    /// </summary>
    public static class LongXtensions
    {
        /// <summary>Negates (* -1) the given long number.</summary>
        /// <param name="number">The given long number.</param>
        /// <returns>The negated long number.</returns>
        public static long Negate(this long number)
        {
            return number * -1;
        }

        /// <summary>Strips out the sign and returns the absolute value of given long number.</summary>
        /// <param name="number">The given long number.</param>
        /// <returns>The absolute value of given long number.</returns>
        public static long AbsoluteValue(this long number)
        {
            return Math.Abs(number);
        }

        /// <summary>Encode the given number into a Base36 string </summary>
        public static string EncodeBase36(this long input)
        {
            string CharList = "0123456789abcdefghijklmnopqrstuvwxyz";
            if (input < 0)
            {
                throw new ArgumentOutOfRangeException("input", input, "input cannot be negative");
            }

            char[] clistarr = CharList.ToCharArray();
            var result = new Stack<char>();

            while (input != 0)
            {
                result.Push(clistarr[input % 36]);
                input /= 36;
            }

            return new string(result.ToArray());
        }
        /// <summary>
        /// Return a DateTime derived from a Unix Epoch time (seconds from 01/01/1970
        /// </summary>
        /// <param name="unixTime">The long value representing the Unix Time</param>
        /// <returns>Date Time value <see cref="DateTime"/></returns>
        public static DateTime DateTimeFromUnixTime(this long unixTime)
        {
            return Utils.DateTimeFromUnixTime(unixTime);
        }
    }
}