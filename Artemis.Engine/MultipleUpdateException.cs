#region Using Statements

using System;
using System.Runtime.Serialization;

#endregion

namespace Artemis.Engine
{
    /// <summary>
    /// An exception thrown when an UpdatableObject is manually updated twice and it's
    /// DisallowMultipleUpdates property is true.
    /// </summary>
    [Serializable]
    public class MultipleUpdateException : Exception
    {
        public MultipleUpdateException() : base() { }
        public MultipleUpdateException(string msg) : base(msg) { }
        public MultipleUpdateException(string msg, Exception inner) : base(msg, inner) { }
        public MultipleUpdateException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
