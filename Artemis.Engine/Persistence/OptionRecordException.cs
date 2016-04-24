#region Using Statements

using System;
using System.Runtime.Serialization;

#endregion

namespace Artemis.Engine.Persistence
{
    [Serializable]
    public class OptionRecordException : Exception
    {
        public OptionRecordException() : base() { }
        public OptionRecordException(string msg) : base(msg) { }
        public OptionRecordException(string msg, Exception inner) : base(msg, inner) { }
        public OptionRecordException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
