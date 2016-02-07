#region Using Statements

using System;
using System.Runtime.Serialization;

#endregion

namespace Artemis.Engine
{
    /// <summary>
    /// An exception thrown when something goes wrong in a DynamicProperty.
    /// </summary>
    [Serializable]
    public class DynamicPropertyException : Exception
    {
        public DynamicPropertyException() : base() { }
        public DynamicPropertyException(string msg) : base(msg) { }
        public DynamicPropertyException(string msg, Exception inner) : base(msg, inner) { }
        public DynamicPropertyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
