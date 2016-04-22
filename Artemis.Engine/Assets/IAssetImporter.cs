#region Using Statements

using System;

#endregion

namespace Artemis.Engine.Assets
{
    public interface IAssetImporter
    {

        /// <summary>
        /// Import an asset from the given full file path with extension.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        object ImportFrom(string filePath);
    }
}
