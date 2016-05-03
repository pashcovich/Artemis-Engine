#region Using Statements

using System;
using System.Runtime.Serialization;

#endregion

namespace Artemis.Engine.Graphics
{

    [Serializable]
    public class RenderLayerException : Exception
    {
        public RenderLayerException() : base() { }
        public RenderLayerException(string msg) : base(msg) { }
        public RenderLayerException(string msg, Exception inner) : base(msg, inner) { }
        public RenderLayerException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

}
