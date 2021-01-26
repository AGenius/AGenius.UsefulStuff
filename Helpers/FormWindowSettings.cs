using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AGenius.UsefulStuff.Helpers
{
    /// <summary>Holds various settings for a modules window (or sub window)</summary>
    /// <remarks>Window State : Maximized or Standard
    /// Window Size and position (Top Left)</remarks>
    public class FormWindowSettings
    {
        public FormWindowState WindowState
        {
            get; set;
        }
        public int WindowHeight
        {
            get; set;
        }
        public int WindowWidth
        {
            get; set;
        }
        public int WindowTop
        {
            get; set;
        }
        public int WindowLeft
        {
            get; set;
        }
    }
}
