using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AGenius.UsefulStuff
{
    /// <summary>
    /// Form Extension methods
    /// </summary>
    public static class FormExtensions
    {
        /// <summary>
        /// Determine if the form is visible on a screen 
        /// </summary>  
        /// <param name="FormObject">The Form</param>
        /// <param name="MinPercentOnScreen">Percentage visible , below this the form is treated as not visible</param>
        /// <returns>True if visible, false if not</returns>
        public static bool IsOnScreen(this Form FormObject, double MinPercentOnScreen = 0.2)
        {
            System.Drawing.Point RecLocation = FormObject.Location;
            System.Drawing.Size RecSize = FormObject.Size;
            double PixelsVisible = 0;
            System.Drawing.Rectangle Rec = new System.Drawing.Rectangle(RecLocation, RecSize);

            foreach (Screen Scrn in Screen.AllScreens)
            {
                System.Drawing.Rectangle r = System.Drawing.Rectangle.Intersect(Rec, Scrn.WorkingArea);
                // intersect rectangle with screen
                if (r.Width != 0 & r.Height != 0)
                {
                    PixelsVisible += (r.Width * r.Height);// tally visible pixels
                }
            }
            return PixelsVisible >= (Rec.Width * Rec.Height) * MinPercentOnScreen;
        }
    }
}