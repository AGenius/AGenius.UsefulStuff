using System;
using System.Collections.Generic;
using System.Linq;

namespace AGenius.UsefulStuff
{
    /// <summary>
    /// Extension for the Enum  and struct type
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Converts a string to the specified enum type.
        /// </summary>
        /// <typeparam name="T">The enum type to convert to.</typeparam>
        /// <param name="value">The string value to convert.</param>
        /// <param name="defaultValue">The default enum value to return if conversion fails.</param>
        /// <param name="ignoreCase">Whether to ignore case during the conversion.</param>
        /// <returns>The enum value if the conversion is successful; otherwise, the default enum value.</returns>
        public static T ToEnum<T>(this string value, T defaultValue, bool ignoreCase = true) where T : struct, Enum
        {
            if (Enum.TryParse(value, ignoreCase, out T result))
            {
                return result;
            }
            return defaultValue;
        }
        /// <summary>
        /// Converts a string to the specified enum type.
        /// </summary>
        /// <typeparam name="T">The enum type to convert to.</typeparam>
        /// <param name="value">The string value to convert.</param>
        /// <param name="ignoreCase">Whether to ignore case during the conversion.</param>
        /// <returns>The enum value if the conversion is successful; otherwise, the default enum value.</returns>
        public static T ToEnum<T>(this string value, bool ignoreCase = true) where T : struct, Enum
        {
            if (Enum.TryParse(value, ignoreCase, out T result))
            {
                return result;
            }
            throw new ArgumentException($"Invalid value '{value}' for enum type '{typeof(T).Name}'");
        }
        /// <summary>Gets all items for an enum type.</summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <returns>An enumerable of all enum values.</returns>
        public static IEnumerable<T> GetAllItems<T>() where T : struct, Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}