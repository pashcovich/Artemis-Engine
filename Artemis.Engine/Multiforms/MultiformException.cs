#region Using Statements

using System;
using System.Runtime.Serialization;

#endregion

namespace Artemis.Engine.Multiforms
{
    /// <summary>
    /// An exception that occurs when something goes wrong in a Multiform.
    /// </summary>
    [Serializable]
    public class MultiformException : Exception
    {
        public MultiformException() : base() { }
        public MultiformException(string msg) : base(msg) { }
        public MultiformException(string msg, Exception inner) : base(msg, inner) { }
        public MultiformException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
