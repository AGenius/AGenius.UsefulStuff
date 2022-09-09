using System.ComponentModel;
using System.Drawing;

namespace AGenius.UsefulStuff
{
    /// <summary>
    /// Font Extensions
    /// </summary>
    [TypeConverter(typeof(FontConverter))]
    public static class FontExtensions
    {
        /// <summary>Return a given font to a string </summary>
        /// <param name="font">The font to serialize</param>
        /// <returns>String</returns>
        static public string SerializeToString(this Font font)
        {
            return $"{font.GetType().Name},{font.FontFamily.Name},{font.Size},{font.Style},{font.Unit},{font.GdiCharSet},{font.GdiVerticalFont}";
        }
    }
}
