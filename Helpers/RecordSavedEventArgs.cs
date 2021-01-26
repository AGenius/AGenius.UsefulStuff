using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGenius.UsefulStuff.Helpers
{
    public class RecordSavedEventArgs : EventArgs
    {
        public RecordSavedEventArgs(int id, string message, bool success)
        {
            ID = id;
            Message = message;
            Success = success;
        }

        public int ID { get; private set; }
        public bool Success { get; private set; }
        public string Message { get; private set; }
    }
}
