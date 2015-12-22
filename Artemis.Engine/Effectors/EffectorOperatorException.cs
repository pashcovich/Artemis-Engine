#region Using Statements

using System;
using System.Runtime.Serialization;

#endregion

namespace Artemis.Engine
{
    /// <summary>
    /// An exception thrown when something goes wrong with an EffectorOperator.
    /// </summary>
    [Serializable]
    public class EffectorOperatorException : Exception
    {
        public EffectorOperatorException() : base() { }
        public EffectorOperatorException(string msg) : base(msg) { }
        public EffectorOperatorException(string msg, Exception inner) : base(msg, inner) { }
        public EffectorOperatorException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
