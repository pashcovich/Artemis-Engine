#region Using Statements

using System;
using System.Runtime.Serialization;

#endregion

namespace Artemis.Engine.Graphics
{

    /// <summary>
    /// An exception thrown for Camera related issues such as invalid parameters.
    /// </summary>
    [Serializable]
    public class CameraException : Exception
    {
        public CameraException() : base() { }
        public CameraException(string msg) : base(msg) { }
        public CameraException(string msg, Exception inner) : base(msg, inner) { }
        public CameraException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

}
