#region Using Statements

using System;
using System.Runtime.Serialization;

#endregion

namespace Artemis.Engine.Maths
{

    /// <summary>
    /// An exception which occurs when NewtonsMethod cannot approximate or find the root of a given function.
    /// </summary>
    [Serializable]
    public class NewtonsMethodException : Exception
    {
        public NewtonsMethodException() : base() { }
        public NewtonsMethodException(string msg) : base(msg) { }
        public NewtonsMethodException(string msg, Exception inner) : base(msg, inner) { }
        public NewtonsMethodException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
