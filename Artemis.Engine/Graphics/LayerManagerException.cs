using System;
using System.Runtime.Serialization;

namespace Artemis.Engine
{
    /// <summary>
    /// An exception thrown when something goes wrong in a LayerManager
    /// </summary>
    public class LayerManagerException : Exception
    {
        public LayerManagerException() : base() { }
        public LayerManagerException(string msg) : base(msg) { }
        public LayerManagerException(string msg, Exception inner) : base(msg, inner) { }
        public LayerManagerException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
