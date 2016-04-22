#region Using Statements

using System;
using System.Runtime.Serialization;

#endregion

namespace Artemis.Engine.Assets
{
    /// <summary>
    /// An exception thrown when something goes wrong attempting to register an asset importer.
    /// </summary>
    [Serializable]
    public class AssetImporterRegistrationException : Exception
    {
        public AssetImporterRegistrationException() : base() { }
        public AssetImporterRegistrationException(string msg) : base(msg) { }
        public AssetImporterRegistrationException(string msg, Exception inner) : base(msg, inner) { }
        public AssetImporterRegistrationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
