#region Using Statements

using System;
using System.Runtime.Serialization;

#endregion

namespace Artemis.Engine.Multiforms
{
    /// <summary>
    /// An exception that occurs when something goes wrong in the MultiformManager.
    /// </summary>
    [Serializable]
    public class MultiformManagerException : Exception
    {
        public MultiformManagerException() : base() { }
        public MultiformManagerException(string msg) : base(msg) { }
        public MultiformManagerException(string msg, Exception inner) : base(msg, inner) { }
        public MultiformManagerException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
