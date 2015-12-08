using System;
using System.Runtime.Serialization;

namespace Artemis.Engine
{
    /// <summary>
    /// An exception thrown when something goes wrong when importing an asset.
    /// </summary>
    public class AssetImportException : Exception
    {
        public AssetImportException() : base() { }
        public AssetImportException(string msg) : base(msg) { }
        public AssetImportException(string msg, Exception inner) : base(msg, inner) { }
        public AssetImportException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
