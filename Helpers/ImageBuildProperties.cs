using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace AGenius.UsefulStuff
{
    #region Image Creation
    public class ImageBuildProperties
    {
        public string ImageFileName { get; set; }
        public System.Drawing.Imaging.ImageFormat OutputImageFormat { get; set; } = System.Drawing.Imaging.ImageFormat.Png;
        public Image StartingImage { get; set; } = null;
        public bool isTransparent { get; set; } = false;
        public Color TransparentColour { get; set; } = Color.White;
        public int Height { get; set; }
        public int Width { get; set; }

        public ImageBoxProperties boxProperties;
        public ImageTextProperties textProperties;

        public ImageBuildProperties()
        {
            boxProperties = new ImageBoxProperties();
            textProperties = new ImageTextProperties();
        }

        public ImageBuildProperties(string imageFileName, ImageFormat outputImageFormat, Image startingImage, bool isTransparent, Color transparentColour, int height, int width)
        {
            ImageFileName = imageFileName ?? throw new ArgumentNullException(nameof(imageFileName));
            OutputImageFormat = outputImageFormat ?? throw new ArgumentNullException(nameof(outputImageFormat));
            StartingImage = startingImage ?? throw new ArgumentNullException(nameof(startingImage));
            this.isTransparent = isTransparent;
            TransparentColour = transparentColour;
            Height = height;
            Width = width;
        }
        public class ImageTextProperties
        {
            public int? Height { get; set; }
            public int? Width { get; set; }
            public int? Top { get; set; }
            public int? Left { get; set; }
            public string TextString { get; set; }
            public Font Font { get; set; } = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Point);
            public StringAlignment Halignment { get; set; } = StringAlignment.Center;
            public StringAlignment Valignment { get; set; } = StringAlignment.Center;
            public Color Colour { get; set; }
            public int Padding { get; set; } = 20;
        }
        public class ImageBoxProperties
        {
            public int Height { get; set; }
            public int Width { get; set; }
            public int Top { get; set; }
            public int Left { get; set; }
            public int Thickness { get; set; } = 10;
            public Color Colour { get; set; }
        }
    }
    #endregion

}
