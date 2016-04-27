#region Using Statements

using Artemis.Engine.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Artemis.Engine.Multiforms
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

        /// <summary>
        /// The order in which multiforms are updated.
        /// 
        /// Note: the order in which the multiforms are rendered is the reverse of this order.
        /// To visualize this, consider the multiforms as sheets of paper layered on top of each other.
        /// The ones at the top are updated first, but they have to be rendered on top of everything else,
        /// meaning they have to be rendered last.
        /// </summary>
        private string[] GlobalProcessOrder = null;

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
        public void Activate(Multiform sender, string name, MultiformConstructionArgs args = null)
        {
            if (ActiveMultiforms.ContainsKey(name))
            {
                throw new MultiformManagerException(
                    String.Format("Multiform with name '{0}' has already been constructed.", name));
            }

            if (!RegisteredMultiforms.ContainsKey(name))
            {
                throw new MultiformManagerException(
                    String.Format("No multiform with name '{0}' exists.", name));
            }

            var multiform = RegisteredMultiforms[name];
            if (multiform.TransitionConstraints != null)
            {
                var constraints = multiform.TransitionConstraints;
                if ((constraints.AllowedFrom != null && !constraints.AllowedFrom.Contains(sender.Name)) ||
                    (constraints.NotAllowedFrom != null && constraints.NotAllowedFrom.Contains(sender.Name)))
                {
                    throw new MultiformManagerException(
                        String.Format(
                            "The transition constraints on multiform '{0}' prevent the multiform '{1}' from " +
                            "being able to transition to it.", name, sender.Name
                        )
                    );
                }
            }
            args = args == null ? new MultiformConstructionArgs(sender) : args;
            ApplyOrQueueEvent(new ActivateEvent(name, args));
        }

        /// <summary>
        /// Deactivate the multiform with the given name.
        /// </summary>
        /// <param name="name"></param>
        public void Deactivate(string name)
        {
            if (!ActiveMultiforms.ContainsKey(name))
            {
                throw new MultiformManagerException(
                    String.Format("Multiform with name '{0}' has not been constructed.", name));
            }
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
        /// Set the global process order, which is the order multiforms are updated in.
        /// 
        /// The global process order may only be set once before the game begins running.
        /// </summary>
        /// <param name="names"></param>
        public void SetProcessOrder(string[] names)
        {
            if (Updating)
            {
                throw new MultiformException(
                    "Cannot set global multiform process order mid-update. This " +
                    "can only be set once before the game begins.");
            }
            GlobalProcessOrder = names;
        }

        /// <summary>
        /// Update all the multiforms.
        /// </summary>
        internal void Update()
        {
            Updating = true;

            foreach (var name in GlobalProcessOrder)
            {
                if (!ActiveMultiforms.ContainsKey(name))
                    continue;
                ActiveMultiforms[name].Update();
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
            //
            // Potential problem: this might cause a sort of "waterfall" effect of queued events, causing
            // a number of events which were intended to happen simultaneously to happen sequentially instead.
            // For example, consider a multiform, which constructs another multiform, which constructs another
            // multiform in it's constructor, which does the same, and so on. Each consecutive multiform would
            // be constructed the next frame.

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
            foreach (var name in GlobalProcessOrder.Reverse())
            {
                if (!ActiveMultiforms.ContainsKey(name))
                    continue;
                ActiveMultiforms[name].Render();
            }
        }
    }
}
