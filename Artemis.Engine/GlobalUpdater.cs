#region Using Statements

using System.Collections.Generic;

#endregion

namespace Artemis.Engine
{
    public sealed class GlobalUpdater
    {
        // HashSet because HashSets have fast insertion, removal, and iteration
        internal HashSet<ArtemisObject> Objects = new HashSet<ArtemisObject>();

        internal GlobalUpdater() { }

        /// <summary>
        /// Adds an object to the global ticker (unless it's already been added)
        /// </summary>
        internal void Add(ArtemisObject obj)
        {
            Objects.Add(obj);
        }

        internal void Remove(ArtemisObject obj)
        {
            Objects.Remove(obj);
        }

        /// <summary>
        /// Iterate through all internally stored ArtemisObjects and see 
        /// which ones have and haven’t been updated. If they haven’t, call 
        /// their update method. Set their 'NeedsUpdate' flag to true.
        /// </summary>
        internal void FinalizeUpdate()
        {
            foreach (var obj in Objects)
            {
                if (obj.NeedsUpdate)
                {
                    obj.Update();
                }
                obj.NeedsUpdate = true;
            }
        }
    }
}
