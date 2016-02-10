#region Using Statements

using Artemis.Engine.Utilities;

using System;
using System.Collections.Generic;

#endregion

namespace Artemis.Engine
{
    /// <summary>
    /// A class that records currently registered and active multiforms and controls
    /// updating them, rendering them, and other actions.
    /// </summary>
    public sealed class MultiformManager
    {

        /// <summary>
        /// An event that indicates to activate the multiform with the given name.
        /// </summary>
        private class ActivateEvent : MultiformPostUpdateEvent
        {
            string name;
            MultiformConstructionArgs args;
            public ActivateEvent(string name, MultiformConstructionArgs args)
            {
                this.name = name;
                this.args = args;
            }
            public override void Perform(
                Dictionary<string, Multiform> registered, 
                Dictionary<string, Multiform> active)
            {
                if (active.ContainsKey(name))
                {
                    throw new MultiformManagerException(
                        String.Format("Multiform with name '{0}' has already been constructed.", name));
                }

                if (!registered.ContainsKey(name))
                {
                    throw new MultiformManagerException(
                        String.Format("No multiform with name '{0}' exists.", name));
                }
                var multiform = registered[name];
                multiform.DelegateConstruction(args);
                active.Add(name, multiform);
            }
        }

        /// <summary>
        /// An event that indicates to deactivate the multiform with the given name.
        /// </summary>
        private class DeactivateEvent : MultiformPostUpdateEvent
        {
            string name;
            public DeactivateEvent(string name)
            {
                this.name = name;
            }
            public override void Perform(
                Dictionary<string, Multiform> registered, 
                Dictionary<string, Multiform> active)
            {
                if (!active.ContainsKey(name))
                {
                    throw new MultiformManagerException(
                        String.Format("Multiform with name '{0}' has not been constructed.", name));
                }
                var multiform = active[name];
                multiform.Deconstruct();
                multiform.ResetTime();
                active.Remove(name);
            }
        }

        /// <summary>
        /// The dictionary of all registered multiform instances by name.
        /// </summary>
        private Dictionary<string, Multiform> RegisteredMultiforms = new Dictionary<string, Multiform>();

        /// <summary>
        /// The dictionary of currently active multiforms.
        /// </summary>
        private Dictionary<string, Multiform> ActiveMultiforms = new Dictionary<string, Multiform>();

        /// <summary>
        /// Whether or we are in the middle of updating the currently active multiforms.
        /// </summary>
        private bool Updating = false;
        
        /// <summary>
        /// Whether or not we are in the middle of applying PostUpdateEvents.
        /// </summary>
        private bool ApplyingPostUpdateEvents = false;

        /// <summary>
        /// The list of PostUpdateEvents aggregated during the Update loop. These are
        /// events that in some way or another alter the dictionary of currently active
        /// multiforms, and thus have to be performed after the main update loop.
        /// </summary>
        private List<MultiformPostUpdateEvent> PostUpdateEvents = new List<MultiformPostUpdateEvent>();

        /// <summary>
        /// The list of PostUpdateEvents that have to be queued for the next call to
        /// Update. The reason we keep this list as well is because some PostUpdateEvents
        /// can alter the list of PostUpdateEvents whilst iterating through it, which would
        /// cause a ConcurrentModificationException to get thrown. Thus, when iterating through
        /// said list, we instead aggregate a list of newly added PostUpdateEvents and wait for
        /// the next call to Update to apply them.
        /// </summary>
        private List<MultiformPostUpdateEvent> PostUpdateEventQueue = new List<MultiformPostUpdateEvent>();

        public MultiformManager() { }

        /// <summary>
        /// Register a multiform instance with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="instance"></param>
        internal void RegisterMultiform(Multiform instance)
        {
            if (RegisteredMultiforms.ContainsKey(instance.Name) || instance.Registered)
            {
                throw new MultiformManagerException(
                    String.Format(
                        "The multiform with name '{0}' has already been registered.", instance.Name
                        )
                    );
            }
            RegisteredMultiforms.Add(instance.Name, instance);
            instance.PostRegister(this);
        }

        /// <summary>
        /// Register a multiform with the given type.
        /// </summary>
        /// <param name="multiformType"></param>
        internal void RegisterMultiform(Type multiformType)
        {
            if (!typeof(Multiform).IsAssignableFrom(multiformType))
            {
                throw new MultiformRegistrationException(
                    String.Format(
                        "The given multiform type {0} does not inherit from Multiform.", multiformType)
                        );
            }
            RegisterMultiform((Multiform)Activator.CreateInstance(multiformType));
        }

        private void ApplyOrQueueEvent(MultiformPostUpdateEvent evt)
        {
            if (Updating)
            {
                PostUpdateEvents.Add(evt);
            }
            else if (ApplyingPostUpdateEvents)
            {
                PostUpdateEventQueue.Add(evt);
            }
            else
            {
                // Perform it immediately if we're not updating or applying PostUpdateEvents.
                evt.Perform(RegisteredMultiforms, ActiveMultiforms);
            }
        }

        /// <summary>
        /// Activate the multiform with the given name.
        /// </summary>
        /// <param name="name"></param>
        public void Activate(string name, MultiformConstructionArgs args)
        {
            ApplyOrQueueEvent(new ActivateEvent(name, args));
        }

        /// <summary>
        /// Deactivate the multiform with the given name.
        /// </summary>
        /// <param name="name"></param>
        public void Deactivate(string name)
        {
            ApplyOrQueueEvent(new DeactivateEvent(name));
        }

        /// <summary>
        /// Deactivate the given multiform instance.
        /// </summary>
        /// <param name="multiform"></param>
        public void Deactivate(Multiform multiform)
        {
            Deactivate(multiform.Name);
        }

        /// <summary>
        /// Update all the multiforms.
        /// </summary>
        internal void Update()
        {
            Updating = true;

            foreach (var kvp in ActiveMultiforms)
            {
                kvp.Value.Update();
            }

            Updating = false;

            // Apply PostUpdateEvents.

            ApplyingPostUpdateEvents = true;

            foreach (var evt in PostUpdateEvents)
            {
                evt.Perform(RegisteredMultiforms, ActiveMultiforms);
            }
            PostUpdateEvents.Clear();

            ApplyingPostUpdateEvents = false;

            // Add the queued PostUpdate events to the list of PostUpdateEvents to be performed 
            // next time Update is called. The reason we need this is because there are certain 
            // post update events that can alter the PostUpdateEvents list whilst iterating. For 
            // example, a PostUpdateEvent can Activate a multiform, which can in turn call something 
            // that adds a PostUpdateEvent to the list.

            foreach (var queuedEvt in PostUpdateEventQueue)
            {
                PostUpdateEvents.Add(queuedEvt);
            }

            PostUpdateEventQueue.Clear();
        }

        /// <summary>
        /// Render all the multiforms.
        /// </summary>
        internal void Render()
        {
            foreach (var kvp in ActiveMultiforms)
            {
                kvp.Value.Render();
            }
        }
    }
}
