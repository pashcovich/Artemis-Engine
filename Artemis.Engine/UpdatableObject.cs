#region Using Statements

using Artemis.Engine.Utilities;
using Artemis.Engine.Utilities.Partial;

#endregion

namespace Artemis.Engine
{
    public delegate void Updater();

    public delegate void Renderer();

    public class UpdatableObject : PartialEngineAdapter
    {

        private static AttributeMemoService<UpdatableObject> attrMemoService
            = new AttributeMemoService<UpdatableObject>();

        static UpdatableObject()
        {
            var handler = new AttributeMemoService<UpdatableObject>.AttributeHandler(
                obj => { obj.ManuallyUpdate = true; });
            attrMemoService.RegisterHandler<ManualUpdateAttribute>(handler);
        }

        /// <summary>
        /// Whether or not this object needs to be manually updated by the user or can be
        /// automatically updated by the engine every frame.
        /// </summary>
        public bool ManuallyUpdate { get; set; }

        /// <summary>
        /// Whether or not the updating for this object is paused.
        /// </summary>
        public bool IsPaused { get; set; }

        /// <summary>
        /// Decides whether or not to update object.
        /// </summary>
        public bool NeedsUpdate { get; internal set; }

        /// <summary>
        /// Whether or not "Update" can be called multiple times in a single game tick.
        /// </summary>
        public bool DisallowMultipleUpdates;

        /// <summary>
        /// The updater exposed to the user.
        /// </summary>
        public Updater Updater { get; set; }

        public UpdatableObject() : base()
        {
            if (!IsPartial)
            {
                ArtemisEngine.GameUpdater.Add(this);
            }
        }

        /// <summary>
        /// Pause automatic updating.
        /// 
        /// This will only have an effect if ManuallyUpdate is false, in which case the
        /// engine automatically updates the object. Regardless of whether or not automatic
        /// updating is paused, directly calling "Update" will still update the object.
        /// </summary>
        public void Pause()
        {
            IsPaused = true;
        }

        /// <summary>
        /// Resume automatic updating.
        /// </summary>
        public void UnPause()
        {
            IsPaused = false;
        }

        public void SetUpdater(Updater updater)
        {
            Updater = updater;
        }

        public virtual void Update()
        {
            if (NeedsUpdate)
            {
                AutomaticUpdate();

                AuxiliaryUpdate();

                NeedsUpdate = false;
            }
            else
            {
                if (DisallowMultipleUpdates)
                {
                    throw new MultipleUpdateException(
                        string.Format(
                            "UpdatableObject '{0}' was manually updated twice in a single game tick. If you wish " +
                            "to allow this, set the object's `DisallowMultipleUpdates` property to false.", this
                            )
                        );
                }

                AuxiliaryUpdate();
            }
        }

        /// <summary>
        /// This is the update method for everything that doesn't need to appear in
        /// AutomaticUpdate. It essentially just invokes the assigned Updater, which is
        /// publicly exposed.
        /// </summary>
        internal virtual void AuxiliaryUpdate()
        {
            if (Updater != null)
            {
                Updater();
            }
        }

        /// <summary>
        /// This is the update method for everything that must be updated every frame
        /// regardless of the values of IsPaused or ManuallyUpdate.
        /// 
        /// AutomaticUpdate is always called by the engine once per game tick, unless
        /// the user manually calls "Update".
        /// 
        /// For example, TimeableObject uses AutomaticUpdate to update it's internal
        /// lifetime. This can only be called once every game tick and MUST be called.
        /// </summary>
        internal virtual void AutomaticUpdate() { }

        public virtual void Kill()
        {
            if (!IsPartial)
            {
                ArtemisEngine.GameUpdater.Remove(this);
            }
        }
    }
}
