using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGenius.UsefulStuff
{
    public class UsefulPropertyChangedEventArgs : EventArgs
    {
        public UsefulPropertyChangedEventArgs(string propertyName, string message, object recordObject)
        {
            PropertyName = propertyName;
            Message = message;
            RecordObject = recordObject;
        }

        public string PropertyName { get; private set; }
        public string Message { get; private set; }
        public object RecordObject { get; private set; }
    }
}
