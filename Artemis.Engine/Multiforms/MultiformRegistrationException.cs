#region Using Statements

using System;
using System.Runtime.Serialization;

#endregion

namespace Artemis.Engine.Multiforms
{
    /// <summary>
    /// An exception that occurs when something goes wrong whilst registering a multiform.
    /// </summary>
    [Serializable]
    public class MultiformRegistrationException : Exception
    {
    	public MultiformRegistrationException() : base() { }
    	public MultiformRegistrationException(string msg) : base(msg) { }
    	public MultiformRegistrationException(string msg, Exception inner) : base(msg, inner) { }
    	public MultiformRegistrationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
