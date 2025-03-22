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

    public class SSHelper : IDisposable
    {
        public event Action<string> ScreenshotCaptured;
        // This is used to capture the screen area
        private IKeyboardMouseEvents globalHook;
        private Keys? _hotKey;
        private string tempPathSubFolder;
        private bool disposed = false;
        public SSHelper(Keys? keyCode = Keys.None, string tempPathSubFolder = "Screenshots")
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
                        overlay.Opacity = 0;
                        HideOverlays();// Ensure the overlay is not captured

                        // Capture the screenshot                            
                        var ssFile = CaptureScreenshot(selectionState.SelectionRectangle);
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
            if (_hotKey == Keys.None)
            {
                // Still need key hook to close the overlay
                globalHook = Hook.GlobalEvents();
                globalHook.KeyDown += GlobalHook_KeyDown;
                InitiateScreenCapture(); // Just initiate the screen capture process
            }
            else
            {
                // Start listening for the required hot key key and mouse events
                globalHook = Hook.GlobalEvents();
                globalHook.KeyDown += GlobalHook_KeyDown;
            }
        }
        /// <summary>
        /// Stop listening for the Print Screen key and mouse events
        /// </summary>
        public void DisposeKeyHook()
        {
            try
            {
                if (globalHook != null)
                {
                    globalHook.KeyDown -= GlobalHook_KeyDown;
                    globalHook.Dispose();
                }
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// Perform the Capture of the screenshot to a file and return the path
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="screenBounds"></param>
        /// <returns></returns>
        public string CaptureScreenshot(Rectangle rect)
        {
            try
            {
                // Get the correct screen where the selection is happening
                Screen selectedScreen = Screen.FromPoint(rect.Location);
                Rectangle screenBounds = selectedScreen.Bounds;

                // No need to re-adjust rect.X/Y since it's already in absolute screen coordinates
                using (Bitmap bitmap = new Bitmap(rect.Width, rect.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(rect.Location, Point.Empty, rect.Size);
                    }

                    var fileName = $"Screenshot_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
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
        /// <summary>
        /// Dispose the resources used by the SSHelper
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the resources used by the SSHelper
        /// </summary>
        /// <param name="disposing">Indicates whether the method is called from Dispose or the finalizer</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    DisposeKeyHook();
                }

                // Dispose unmanaged resources

                disposed = true;
            }
        }

        ~SSHelper()
        {
            Dispose(false);
        }
    }
}