using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AGenius.UsefulStuff.Helpers
{
    public partial class SSOverlay : Form
    {
        private SSSelectionState selectionState;

        public SSOverlay(Rectangle bounds, SSSelectionState selectionState)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Bounds = bounds;
            this.BackColor = Color.Wheat;
            this.Opacity = 0.4;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.DoubleBuffered = true;
            //this.Cursor = Cursors.Cross;
            this.Cursor = CreateWhiteCrossCursor();
            this.selectionState = selectionState;
            this.selectionState.OnSelectionChanged += SelectionState_OnSelectionChanged;
        }

        private void SelectionState_OnSelectionChanged()
        {
            UpdateRegion();
            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectionState.StartPoint = PointToScreen(e.Location);
                selectionState.IsDrawing = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (selectionState.IsDrawing)
            {
                var endPoint = PointToScreen(e.Location);
                var selectionRectangle = new Rectangle(
                    Math.Min(selectionState.StartPoint.X, endPoint.X),
                    Math.Min(selectionState.StartPoint.Y, endPoint.Y),
                    Math.Abs(selectionState.StartPoint.X - endPoint.X),
                    Math.Abs(selectionState.StartPoint.Y - endPoint.Y));
                selectionState.UpdateSelection(selectionRectangle, true);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectionState.IsDrawing = false;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (selectionState.IsDrawing)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    var localRectangle = new Rectangle(
                        selectionState.SelectionRectangle.X - this.Bounds.X,
                        selectionState.SelectionRectangle.Y - this.Bounds.Y,
                        selectionState.SelectionRectangle.Width,
                        selectionState.SelectionRectangle.Height);
                    e.Graphics.DrawRectangle(pen, localRectangle); // Draw the rectangle border
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private Cursor CreateWhiteCrossCursor()
        {
            Bitmap bitmap = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.Transparent);
                Pen pen = new Pen(Color.White, 2);
                g.DrawLine(pen, 16, 0, 16, 32);
                g.DrawLine(pen, 0, 16, 32, 16);
            }

            IntPtr ptr = bitmap.GetHicon();
            return new Cursor(ptr);
        }
        private void UpdateRegion()
        {
            if (selectionState.IsDrawing)
            {
                var localRectangle = new Rectangle(
                    selectionState.SelectionRectangle.X - this.Bounds.X,
                    selectionState.SelectionRectangle.Y - this.Bounds.Y,
                    selectionState.SelectionRectangle.Width,
                    selectionState.SelectionRectangle.Height);

                Region region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                region.Exclude(localRectangle);
                this.Region = region;
            }
            else
            {
                this.Region = new Region(new Rectangle(0, 0, this.Width, this.Height));
            }
        }
    }

}