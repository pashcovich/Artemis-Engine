#region Using Statements

using System;
using System.Runtime.Serialization;

#endregion

namespace Artemis.Engine.Graphics
{

    [Serializable]
    public class RenderOrderException : Exception
    {
        public RenderOrderException() : base() { }
        public RenderOrderException(string msg) : base(msg) { }
        public RenderOrderException(string msg, Exception inner) : base(msg, inner) { }
        public RenderOrderException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

}
