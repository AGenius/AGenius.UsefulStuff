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
        static public string SerializeToString(this Font font, string sep = ",")
        {
            return $"{font.GetType().Name}{sep}{font.FontFamily.Name}{sep}{font.Size}{sep}{font.Style}{sep}{font.Unit}{sep}{font.GdiCharSet}{sep}{font.GdiVerticalFont}";
        }
    }
}
