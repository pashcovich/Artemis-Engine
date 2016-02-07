#region Using Statements

using System;
using System.Runtime.Serialization;

#endregion

namespace Artemis.Engine
{
    /// <summary>
    /// An exception thrown when something goes wrong in the GameSetupReader.
    /// </summary>
    [Serializable]
    public class GameSetupException : Exception
    {
        public GameSetupException() : base() { }
        public GameSetupException(string msg) : base(msg) { }
        public GameSetupException(string msg, Exception inner) : base(msg, inner) { }
        public GameSetupException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
