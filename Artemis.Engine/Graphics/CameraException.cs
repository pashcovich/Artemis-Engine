using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Artemis.Engine.Graphics
{
    [Serializable]
    public class CameraException : Exception
    {
        public CameraException() : base() { }
        public CameraException(string msg) : base(msg) { }
        public CameraException(string msg, Exception inner) : base(msg, inner) { }
        public CameraException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
