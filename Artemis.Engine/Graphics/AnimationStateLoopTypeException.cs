#region Using Statements

using System;
using System.Runtime.Serialization;

#endregion

namespace Artemis.Engine.Graphics
{
    [Serializable]
    public class AnimationStateLoopTypeException : Exception
    {
        public AnimationStateLoopTypeException() : base() { }
        public AnimationStateLoopTypeException(string msg) : base(msg) { }
        public AnimationStateLoopTypeException(string msg, Exception inner) : base(msg, inner) { }
        public AnimationStateLoopTypeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
