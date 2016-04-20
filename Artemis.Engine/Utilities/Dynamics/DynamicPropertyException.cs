#region Using Statements

using System;
using System.Runtime.Serialization;

#endregion

namespace Artemis.Engine.Utilities.Dynamics
{
    /// <summary>
    /// An exception thrown when something goes wrong defining, setting, or retrieving
    /// a DynamicProperty from a DynamicPropertyCollection subclass.
    /// </summary>
    [Serializable]
    public class DynamicPropertyException : Exception
    {
        internal DynamicPropertyException() : base() { }
        internal DynamicPropertyException(string msg) : base(msg) { }
        internal DynamicPropertyException(string msg, Exception inner) : base(msg, inner) { }
        internal DynamicPropertyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
