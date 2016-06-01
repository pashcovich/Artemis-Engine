#region Using Statements

using System;
using System.Runtime.Serialization;

#endregion

namespace Artemis.Engine
{
    [Serializable]
    public class FormException : Exception
    {
        public FormException() : base() { }
        public FormException(string msg) : base(msg) { }
        public FormException(string msg, Exception inner) : base(msg, inner) { }
        public FormException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
