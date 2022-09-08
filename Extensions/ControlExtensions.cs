using System;
using System.Drawing;
using System.Windows.Forms;

namespace AGenius.UsefulStuff.Extensions
{
    public static class ControlExtensions
    {
        public static Rectangle GetBoundsRelativeToForm(this Control c)
        {
            if (c == null)
                throw new ArgumentNullException(nameof(c));

            var form = c.FindForm();
            if (form == null)
                throw new InvalidOperationException("The control is not located on a form.");

            var parent = c.Parent;
            if (parent == null)
                throw new InvalidOperationException("The control does not have a parent.");

            var p = form.PointToClient(parent.PointToScreen(c.Location));
            return new Rectangle(p, c.Size);
        }
        public static Point GetPositionInForm(this Control ctrl)
        {
            Point p = ctrl.Location;
            Control parent = ctrl.Parent;
            Form frm = ctrl.FindForm();
            Rectangle screenRectangle = frm.RectangleToScreen(frm.ClientRectangle);
            int titleHeight = screenRectangle.Top - frm.Top;
            int leftMargin = screenRectangle.Left - frm.Left;
            p.Offset(leftMargin, titleHeight);

            while (!(parent is Form))
            {
                p.Offset(parent.Location.X, parent.Location.Y);
                parent = parent.Parent;
            }
            return p;
        }
    }
}