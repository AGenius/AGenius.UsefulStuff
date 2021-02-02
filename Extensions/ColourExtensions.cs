using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGenius.UsefulStuff
{
    /// <summary>
    /// Colour Extensions
    /// </summary>
    public static class ColourExtensions
    {
        public static Color LightenBy(this Color color, int percent)
        {
            return ChangeColorBrightness(color, (float)(percent / 100.0));
        }

        /// <summary>Extension method to darken a colour by a specified percentage </summary>
        /// <param name="color">Color object to darken  </param>
        /// <param name="percent">percent to darken by</param>
        /// <returns>new Color object holding the darker version of the colour</returns>
        public static Color DarkenBy(this Color color, int percent)
        {
            return ChangeColorBrightness(color, (float)(-1 * percent / 100.0));
        }

        /// <summary>Creates color with corrected brightness. </summary>
        /// <param name="color">Color to correct.</param>
        /// <param name="correctionFactor">The brightness correction factor. Must be between -1 and 1. Negative values produce darker colors.</param>
        /// <returns>
        /// Corrected <see cref="Color"/> structure.
        /// </returns>
        private static Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            var red = (float)color.R;
            var green = (float)color.G;
            var blue = (float)color.B;
            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }
            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }
    }
}