using System;
using System.Runtime.Serialization;

namespace Artemis.Engine
{
    /// <summary>
    /// An exception thrown when a layer gets rendered multiple times and it's
    /// MultiRenderAction is set to MultiRenderAction.Fail.
    /// </summary>
    public class MultiRenderException : Exception
    {
        public MultiRenderException() : base() { }
        public MultiRenderException(string msg) : base(msg) { }
        public MultiRenderException(string msg, Exception inner) : base(msg, inner) { }
        public MultiRenderException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
