using System;
using System.Runtime.Serialization;

namespace Artemis.Engine
{
    /// <summary>
    /// An exception thrown when something goes wrong in a UriTreeGroup.
    /// </summary>
    public class UriGroupException : Exception
    {
        public UriGroupException() : base() { }
        public UriGroupException(string msg) : base(msg) { }
        public UriGroupException(string msg, Exception inner) : base(msg, inner) { }
        public UriGroupException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
