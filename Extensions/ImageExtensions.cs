using System;
using System.Drawing;

namespace AGenius.UsefulStuff
{
    /// <summary>
    /// Extensions for Image object
    /// </summary>
    public static class ImageExtensions
    {
        /// <summary>Make a bitmap image transparent</summary>
        /// <param name="image">The Image object</param>
        /// <param name="color">color to convert</param>
        /// <param name="tolerance">The colour tolerance</param>
        /// <returns>Converted Bitmap object</returns>
        public static Bitmap MakeTransparent(this Bitmap image, Color color, int tolerance)
        {
            var resultBitmap = new Bitmap(image);

            resultBitmap.ForEachPixel((i, j, pixelColor) =>
            {
                if (pixelColor.IsCloseTo(color, tolerance))
                    resultBitmap.SetPixel(i, j, color);
            });

            resultBitmap.MakeTransparent(color);

            return resultBitmap;
        }
        private static void ForEachPixel(this Bitmap image, Action<int, int, Color> onPixel)
        {
            for (int i = image.Size.Width - 1; i >= 0; i--)
            {
                for (int j = image.Size.Height - 1; j >= 0; j--)
                {
                    onPixel(i, j, image.GetPixel(i, j));
                }
            }
        }
        private static bool IsCloseTo(this Color color, Color anotherColor, int tolerance)
        {
            return Math.Abs(color.R - anotherColor.R) < tolerance &&
                 Math.Abs(color.G - anotherColor.G) < tolerance &&
                 Math.Abs(color.B - anotherColor.B) < tolerance;
        }
    }
}