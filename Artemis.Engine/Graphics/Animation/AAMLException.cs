using System;
using System.Runtime.Serialization;

namespace Artemis.Engine.Graphics.Animation
{
    [Serializable]
    class AAMLSyntaxException : Exception
    {
        public AAMLSyntaxException() : base() { }
        public AAMLSyntaxException(string msg) : base(msg) { }
        public AAMLSyntaxException(string msg, Exception inner) : base(msg, inner) { }
        public AAMLSyntaxException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    class AAMLConfigurationException : Exception
    {
        public AAMLConfigurationException() : base() { }
        public AAMLConfigurationException(string msg) : base(msg) { }
        public AAMLConfigurationException(string msg, Exception inner) : base(msg, inner) { }
        public AAMLConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
