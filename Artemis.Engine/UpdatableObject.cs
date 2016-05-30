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
        /// The updater.
        /// </summary>
        private Updater Updater;

        private Updater _requiredUpdater;
        protected internal Updater RequiredUpdater
        {
            get { return _requiredUpdater; }
            set
            {
                _requiredUpdater = value;
                Updater = value;
            }
        }

        public UpdatableObject() : base()
        {
            if (!IsPartial)
            {
                ArtemisEngine.GameUpdater.Add(this);
            }
        }

        /// <summary>
        /// Set the updater for this object.
        /// </summary>
        /// <param name="updater"></param>
        public void SetUpdater(Updater updater)
        {
            Updater = null;
            Updater += RequiredUpdater;
            Updater += updater;
        }

        /// <summary>
        /// Add an updater to this object.
        /// </summary>
        /// <param name="updater"></param>
        public void AddUpdater(Updater updater)
        {
            Updater += updater;
        }

        /// <summary>
        /// Remove an updater from this object.
        /// </summary>
        /// <param name="updater"></param>
        public void RemoveUpdater(Updater updater)
        {
            Updater -= updater;
        }

        /// <summary>
        /// Remove all updaters from this object.
        /// </summary>
        public void ClearUpdater()
        {
            Updater = null;
            Updater += RequiredUpdater;
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

        public void AddUpdater(Updater updater)
        {
            Updater += updater;
        }

        public virtual void Update()
        {
            if (NeedsUpdate)
            {
                InternalUpdate();

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
            }
        }

        /// <summary>
        /// This is the update method for everything that must be updated every frame
        /// regardless of the values of IsPaused or ManuallyUpdate.
        /// 
        /// InternalUpdate is always called by the engine once per game tick, unless
        /// the user manually calls "Update".
        /// 
        /// For example, TimeableObject uses InternalUpdate to update it's internal
        /// lifetime. This can only be called once every game tick and MUST be called.
        /// </summary>
        internal virtual void InternalUpdate() { }

        public virtual void AuxiliaryUpdate()
        {
            if (Updater != null)
                Updater();
        }
        
        public virtual void Kill()
        {
            if (!IsPartial)
            {
                ArtemisEngine.GameUpdater.Remove(this);
            }
        }
    }
}
