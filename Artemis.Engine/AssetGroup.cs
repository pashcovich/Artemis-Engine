#region Using Statements

using Artemis.Engine.Utilities;
using Artemis.Engine.Utilities.UriTree;

using System;
using System.IO;
using System.Linq;

#endregion

namespace Artemis.Engine
{
    public class AssetGroup : UriTreeGroup<AssetGroup, object>, IDisposable
    {
        internal AssetGroup( string pathName
                           , SearchOption option
                           , string fileSearchQuery   = "*"
                           , string folderSearchQuery = "*"
                           , bool pruneEmptySubgroups = true )
            : base(Path.GetFileName(pathName))
        {
            // Search for directories to turn into subgroups.
            if (option == SearchOption.AllDirectories)
            {
                var directories = Directory.EnumerateDirectories(
                    pathName, folderSearchQuery, SearchOption.TopDirectoryOnly);

                foreach (var folderName in directories)
                {
                    var subgroup = new AssetGroup(folderName, option, fileSearchQuery, folderSearchQuery);

                    if (subgroup.IsEmpty)
                    {
                        continue;
                    }

                    subgroup.SetParent(this);
                    Subgroups.Add(subgroup.Name, subgroup);
                }
            }
            // Otherwise, option == SearchOption.TopDirectoryOnly, and we only have to look for
            // asset files in the given directory rather than subdirectories as well.

            var files = Directory.EnumerateFiles(
                pathName, fileSearchQuery, SearchOption.TopDirectoryOnly);

            foreach (var fileName in files)
            {
                var assetName = Path.GetFileNameWithoutExtension(
                    DirectoryUtils.MakeRelativePath(
                        AssetLoader.ContentFolderName, fileName));

                Items.Add(assetName, AssetLoader.LoadAssetUsingExtension(fileName));
            }

            // Prune empty subgroups.
            // 
            // ...
            //
            // I really have no good reason why anyone would not want to do this.

            if (pruneEmptySubgroups)
            {
                Subgroups = Subgroups.Where(kvp => !kvp.Value.IsEmpty)
                                 .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
        }
        /// <summary>
        /// Return the asset with the given full name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public T GetAsset<T>(string fullName)
        {
            var asset = GetItem(fullName);
            if (asset is LazyAsset)
            {
                return ((LazyAsset)asset).Load<T>();
            }
            return (T)asset;
        }

        private bool disposed = false;

        ~AssetGroup()
        {
            Dispose();
        }

        /// <summary>
        /// Clean up and dispose this AssetGroup object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Clean up and dispose this AssetGroup object.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Recursively dispose subgroups.
                    foreach (var group in Subgroups)
                    {
                        group.Value.Dispose(disposing);
                    }

                    // Dispose managed disposable asset types.
                    var IDisposableType = typeof(IDisposable);
                    foreach (var asset in Items)
                    {
                        if (IDisposableType.IsAssignableFrom(asset.Value.GetType()))
                        {
                            var disposableAsset = (IDisposable)asset.Value;
                            disposableAsset.Dispose();
                        }
                    }
                }

                disposed = true;
            }
        }
    }
}
