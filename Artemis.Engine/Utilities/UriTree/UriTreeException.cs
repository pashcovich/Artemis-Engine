﻿#region Using Statements

using System;
using System.Runtime.Serialization;

#endregion

namespace Artemis.Engine
{
    /// <summary>
    /// An exception thrown when something goes wrong in a UriTreeGroup.
    /// </summary>
    public class UriTreeException : Exception
    {
        public UriTreeException() : base() { }
        public UriTreeException(string msg) : base(msg) { }
        public UriTreeException(string msg, Exception inner) : base(msg, inner) { }
        public UriTreeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
