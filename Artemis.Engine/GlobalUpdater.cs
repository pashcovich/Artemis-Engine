#region Using Statements

using System.Collections.Generic;

#endregion

namespace Artemis.Engine
{
    public sealed class GlobalUpdater
    {
        // HashSet because HashSets have fast insertion, removal, and iteration
        internal HashSet<UpdatableObject> Objects = new HashSet<UpdatableObject>();

        internal GlobalUpdater() { }

        /// <summary>
        /// Adds an object to the global ticker (unless it's already been added)
        /// </summary>
        internal void Add(UpdatableObject obj)
        {
            Objects.Add(obj);
        }

        internal void Remove(UpdatableObject obj)
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
                    obj.AutomaticUpdate();

                    if (!(obj.IsPaused || obj.ManuallyUpdate))
                    {
                        obj.AuxiliaryUpdate();
                    }
                }
                obj.NeedsUpdate = true;
            }
        }
    }
}
