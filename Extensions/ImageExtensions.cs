using System;
using System.Drawing;
using System.Drawing.Drawing2D;

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
        /// <summary>
        /// Provide image resize ability
        /// </summary>
        /// <param name="image">The source image object</param>
        /// <param name="size">New size settings</param>
        /// <param name="preserveAspectRatio">Maintain aspect /true/false - [Defult:true]</param>
        /// <returns></returns>
        public static Image ResizeImage(this Image image, Size size, bool preserveAspectRatio = true)
        {
            if (image == null)
            {
                return null;
            }
            int newWidth;
            int newHeight;
            if (preserveAspectRatio)
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;
                float percentWidth = (float)size.Width / (float)originalWidth;
                float percentHeight = (float)size.Height / (float)originalHeight;
                float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                newWidth = (int)(originalWidth * percent);
                newHeight = (int)(originalHeight * percent);
            }
            else
            {
                newWidth = size.Width;
                newHeight = size.Height;
            }
            Image newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }
    }
}