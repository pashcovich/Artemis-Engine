#region Using Statements

using System;

#endregion

namespace Artemis.Engine
{
    public abstract class AbstractAssetImporter
    {

        /// <summary>
        /// Import an asset from the given full file path with extension.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public abstract object ImportFrom(string filePath);
    }
}
