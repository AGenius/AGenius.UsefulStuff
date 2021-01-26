using System;
namespace AGenius.UsefulStuff
{

    /// <summary>
    /// This class contains extension methods for <see cref="Int32"/>.
    /// </summary>
    public static class IntXtensions
    {
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
    }
}