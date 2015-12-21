#region Using Statements

using System;
using System.Runtime.Serialization;

#endregion

namespace Artemis.Engine.Graphics
{
    /// <summary>
    /// An exception thrown when something goes wrong in when setting a render order.
    /// </summary>
    public class RenderOrderException : Exception
    {
        public RenderOrderException() : base() { }
        public RenderOrderException(string msg) : base(msg) { }
        public RenderOrderException(string msg, Exception inner) : base(msg, inner) { }
        public RenderOrderException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
