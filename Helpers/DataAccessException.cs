using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AGenius.UsefulStuff.Helpers
{

    [Serializable]
    public class DatabaseAccessHelperException : ApplicationException
    {
        public DatabaseAccessHelperException(string Message, Exception innerException) : base(Message, innerException) { }
        public DatabaseAccessHelperException(string Message) : base(Message) { }
        public DatabaseAccessHelperException() { }

        #region Serializeable Code
        public DatabaseAccessHelperException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        #endregion Serializeable Code
    }
}
