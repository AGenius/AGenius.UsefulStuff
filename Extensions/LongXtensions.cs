using System;

namespace AGenius.UsefulStuff
{
    /// <summary>
    /// This class contains extension methods for <see cref="Int64"/>.
    /// </summary>
    public static class LongXtensions
    {
        /// <summary>
        /// Negates (* -1) the given long number.
        /// </summary>
        /// <param name="number">The given long number.</param>
        /// <returns>The negated long number.</returns>
        public static long Negate(this long number)
        {
            return number * -1;
        }

        /// <summary>
        /// Strips out the sign and returns the absolute value of given long number.
        /// </summary>
        /// <param name="number">The given long number.</param>
        /// <returns>The absolute value of given long number.</returns>
        public static long AbsoluteValue(this long number)
        {
            return Math.Abs(number);
        }
    }
}