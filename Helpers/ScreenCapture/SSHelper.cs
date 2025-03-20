using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace AGenius.UsefulStuff.Helpers
{
    public class SSSelectionState
    {
        public Rectangle SelectionRectangle { get; set; }
        public bool IsDrawing { get; set; }
        public Point StartPoint { get; set; }
        public event Action OnSelectionChanged;

        public void UpdateSelection(Rectangle selectionRectangle, bool isDrawing)
        {
            SelectionRectangle = selectionRectangle;
            IsDrawing = isDrawing;
            OnSelectionChanged?.Invoke();
        }
    }

    public class SSHelper
    {
        public event Action<string> ScreenshotCaptured;

        // This is used to capture the screen area
        private IKeyboardMouseEvents globalHook;
        private Keys? _hotKey;
        private string tempPathSubFolder;
        public SSHelper(Keys? keyCode, string tempPathSubFolder = "Screenshots")
        {
            _hotKey = keyCode;
            this.tempPathSubFolder = tempPathSubFolder;
        }

        /// <summary>
        /// Perform the capture process once the correct key is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlobalHook_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == _hotKey)
            {
                InitiateScreenCapture();
            }

            if (e.KeyCode == Keys.Escape)
            {
                DisposeKeyHook();
                CloseOverlays();
            }
        }
        /// <summary>
        /// Initiate the screen capture process
        /// </summary>
        /// <remarks>This can be triggered manually in code or by the key press</remarks>
        public void InitiateScreenCapture()
        {
            // Create a shared state for the selection rectangle
            var selectionState = new SSSelectionState();
            List<SSOverlay> overlays = new List<SSOverlay>();
            // Create an overlay form for each monitor               
            foreach (var screen in Screen.AllScreens)
            {
                var overlay = new SSOverlay(screen.Bounds, selectionState);
                overlays.Add(overlay);
                overlay.Show();
            }

            // Wait for the user to complete the selection
            foreach (var overlay in overlays)
            {
                overlay.FormClosed += (s, args) =>
                {
                    DisposeKeyHook();
                    if (overlay.DialogResult == DialogResult.OK)
                    {
                        HideOverlays();// Ensure the overlay is not captured

                        // Capture the screenshot                            
                        var ssFile = CaptureScreenshot(selectionState.SelectionRectangle, overlay.Bounds);
                        CloseOverlays();
                        // Raise the ScreenshotCaptured event
                        ScreenshotCaptured?.Invoke(ssFile);
                    }
                };
            }
        }
        /// <summary>
        /// Close all overlay forms
        /// </summary>
        public void CloseOverlays()
        {
            try
            {
                foreach (var overlay in Application.OpenForms.OfType<SSOverlay>().ToList())
                {
                    overlay.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// Close all overlay forms
        /// </summary>
        public void HideOverlays()
        {
            try
            {
                foreach (var overlay in Application.OpenForms.OfType<SSOverlay>().ToList())
                {
                    overlay.Hide();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Start listening for the Print Screen key and mouse events
        /// </summary>
        /// <param name="KeyCode">The key code to trigger the capture process</param>
        public void StartSSCapture()
        {
            globalHook = Hook.GlobalEvents();
            globalHook.KeyDown += GlobalHook_KeyDown;
        }
        /// <summary>
        /// Stop listening for the Print Screen key and mouse events
        /// </summary>
        public void DisposeKeyHook()
        {
            globalHook.KeyDown -= GlobalHook_KeyDown;
            globalHook.Dispose();
        }
        /// <summary>
        /// Perform the Capture of the screenshot to a file and return the path
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="screenBounds"></param>
        /// <returns></returns>
        public string CaptureScreenshot(Rectangle rect, Rectangle screenBounds)
        {
            try
            {
                // Translate the coordinates of the selection rectangle to the screen coordinates of the respective monitor
                Rectangle adjustedRect = new Rectangle(
                    rect.X + screenBounds.X,
                    rect.Y + screenBounds.Y,
                    rect.Width,
                    rect.Height
                );

                using (Bitmap bitmap = new Bitmap(rect.Width, rect.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(adjustedRect.Location, Point.Empty, adjustedRect.Size);
                    }

                    var fileName = $"Screenshot_{DateTime.Now.ToString("yyyy-mm-dd_HHss")}";
                    var screenshotPath = TemporaryFiles.GetNewAlt(fileName, ".png", tempPathSubFolder);
                    bitmap.Save(screenshotPath, System.Drawing.Imaging.ImageFormat.Png);
                    return screenshotPath;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}