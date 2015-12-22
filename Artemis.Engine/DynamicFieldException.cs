#region Using Statements

using System;
using System.Runtime.Serialization;

#endregion

namespace Artemis.Engine
{
    /// <summary>
    /// An exception thrown when something goes wrong in a DynamicFieldContainer.
    /// </summary>
    [Serializable]
    public class DynamicFieldException : Exception
    {
        public DynamicFieldException() : base() { }
        public DynamicFieldException(string msg) : base(msg) { }
        public DynamicFieldException(string msg, Exception inner) : base(msg, inner) { }
        public DynamicFieldException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
