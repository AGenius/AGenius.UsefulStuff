using System;

namespace AGenius.UsefulStuff
{
    /// <summary>
    /// This class contains extension methods for <see cref="Double"/>.
    /// </summary>
    public static class DoubleXtensions
    {
        /// <summary>
        /// Rounds the decimal portion of the given double number.
        /// </summary>
        /// <param name="number">The given double number.</param>
        /// <returns>The rounded double number.</returns>
        public static long Round(this double number)
        {
            return (long)Math.Round(number);
        }

        /// <summary>
        /// Truncates the decimal portion of the given double number.
        /// </summary>
        /// <param name="number">The given double number.</param>
        /// <returns>The truncated double number.</returns>
        public static long Truncate(this double number)
        {
            return (long)Math.Truncate(number);
        }
        /// <summary>
        /// Rounds the value to the nearest increment
        /// </summary>
        /// <param name="number">The given double number.</param>
        /// <param name="increment">The increment to round by</param>
        /// <returns></returns>
        public static int RoundToX(this double number, double increment)
        {
            return (int)(Math.Ceiling(number / increment) * increment);
        }
    }
}