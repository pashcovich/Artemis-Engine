#region Using Statements

using System;
using System.Runtime.Serialization;

#endregion

namespace Artemis.Engine
{
    /// <summary>
    /// An exception thrown when something goes wrong in an Effector.
    /// </summary>
    public class EffectorException : Exception
    {
        public EffectorException() : base() { }
        public EffectorException(string msg) : base(msg) { }
        public EffectorException(string msg, Exception inner) : base(msg, inner) { }
        public EffectorException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
