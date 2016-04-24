#region Using Statements

using System;
using System.Runtime.Serialization;

#endregion

namespace Artemis.Engine.Graphics
{
    [Serializable]
    public class AnimationStateException : Exception
    {
        public AnimationStateException() : base() { }
        public AnimationStateException(string msg) : base(msg) { }
        public AnimationStateException(string msg, Exception inner) : base(msg, inner) { }
        public AnimationStateException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
