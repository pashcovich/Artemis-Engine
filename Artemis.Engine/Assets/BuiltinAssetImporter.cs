#region Using Statements

using Artemis.Engine.Utilities;

using System.IO;

#endregion

namespace Artemis.Engine.Assets
{
    /// <summary>
    /// Builtin IAssetImporter for object types T already known by the ContentManager.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class BuiltinAssetImporter<T> : IAssetImporter
    {
        public object ImportFrom(string filePath)
        {
            if (AssetLoader.Content != null) // This is in case the AssetLoader is needed in a partial engine environment.
            {
                var cfName = AssetLoader.ContentFolderName;
                return AssetLoader.Content.Load<T>(
                    DirectoryUtils.MakeRelativePath(
                        cfName, Path.Combine(cfName, filePath)));
            }
            return default(T);
        }
    }
}
