// A delegate type for hooking up close notifications.
using System;
using System.Windows.Forms;

namespace AGenius.UsefulStuff
{
    public delegate void ClosingEventHandler(object sender, EventArgs e);

    // We need to extend the basic Web Browser because when a web page calls
    // "window.close()" the containing window isn't closed.
    public class ExtendedWebBrowser : WebBrowser
    {
        // Define constants from winuser.h
        private const int WM_PARENTNOTIFY = 0x210;
        private const int WM_DESTROY = 2;

        public event ClosingEventHandler Closing;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_PARENTNOTIFY:
                    if (!DesignMode)
                    {
                        if (m.WParam.ToInt32() == WM_DESTROY)
                        {
                            Closing(this, EventArgs.Empty);
                        }
                    }
                    DefWndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}