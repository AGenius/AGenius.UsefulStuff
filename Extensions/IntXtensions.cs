using System;
namespace AGenius.UsefulStuff
{

    /// <summary>
    /// This class contains extension methods for <see cref="Int32"/>.
    /// </summary>
    public static class IntXtensions
    {
        /// <summary>
        ///  Get the Modulo using safe method to handle negatives
        /// </summary>
        /// <param name="number"></param>
        /// <param name="modulo"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int Mod(this int number, int modulo)
        {
            if (modulo == 0)
                throw new ArgumentOutOfRangeException("modulo", "(number mod 0) is undefined.");

            //puts a in the [-n+1, n-1] range using the remainder operator
            int remainder = number % modulo;

            //if the remainder is less than zero, add n to put it in the [0, n-1] range if n is positive
            //if the remainder is greater than zero, add n to put it in the [n-1, 0] range if n is negative
            if ((modulo > 0 && remainder < 0) ||
                (modulo < 0 && remainder > 0))
                return remainder + modulo;
            return remainder;
        }
        /// <summary>
        /// Negates (* -1) the given integer.
        /// </summary>
        /// <param name="number">The given integer.</param>
        /// <returns>The negated integer.</returns>
        public static int Negate(this int number)
        {
            return number * -1;
        }

        /// <summary>
        /// Strips out the sign and returns the absolute value of given integer.
        /// </summary>
        /// <param name="number">The given integer.</param>
        /// <returns>The absolute value of given integer.</returns>
        public static int AbsoluteValue(this int number)
        {
            return Math.Abs(number);
        }

        /// <summary>
        /// Convert an Integer representing minutes Duration to a Hour:Mins format
        /// </summary>
        /// <param name="TimeInMinutes">Minutes to convert</param>
        /// <param name="sep">Seperator between the hours and mins (optional, Default " ")</param>
        /// <param name="hour">Hour string (optional, Default = h)</param>
        /// <param name="mins">Mins string (optional, Default = m)</param>
        /// <returns>String result <see cref="string"/></returns>
        public static string ToDurationString(this int TimeInMinutes, string sep = " ", string hour = "h", string mins = "m")
        {
            try
            {
                if (TimeInMinutes > 0)
                {
                    int hours = (TimeInMinutes - TimeInMinutes % 60) / 60;
                    return $"{hours}{hour}{sep}{TimeInMinutes - hours * 60:00}{mins}";
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}