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
            this.Opacity = 0.2;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.DoubleBuffered = true;
            this.Cursor = Cursors.Cross;
            this.selectionState = selectionState;
            this.selectionState.OnSelectionChanged += SelectionState_OnSelectionChanged;
        }

        private void SelectionState_OnSelectionChanged()
        {
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
                    e.Graphics.DrawRectangle(pen, localRectangle);
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
    }

}
