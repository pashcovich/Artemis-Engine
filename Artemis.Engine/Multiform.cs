#region Using Statements

using Artemis.Engine.Utilities;

using System;
using System.Collections.Generic;

#endregion

namespace Artemis.Engine
{

    /// <summary>
    /// A Multiform represents a specific part of a game with a specific
    /// update loop and a specific render loop.
    /// </summary>
    public abstract class Multiform : ArtemisObject
    {

        /// <summary>
        /// The name of the multiform instance.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The MultiformManager this multiform is registered to.
        /// </summary>
        public MultiformManager Manager { get; private set; }

        /// <summary>
        /// Whether or not this multiform has been registered to a multiform manager.
        /// </summary>
        public bool Registered { get { return Manager != null; } }

        /// <summary>
        /// The number of times this multiform has been activated.
        /// </summary>
        public int TimesActivated { get; private set; }

        /// <summary>
        /// The current renderer for the multiform.
        /// </summary>
        private Action renderer;

        /// <summary>
        /// Whether or not the multiform is reconstructable.
        /// </summary>
        private bool reconstructable;

        public Multiform() : base()
        {
            var thisType = this.GetType();
            Name = Reflection.HasAttribute<NamedMultiformAttribute>(thisType) 
                 ? Reflection.GetFirstAttribute<NamedMultiformAttribute>(thisType).Name
                 : thisType.Name;

            reconstructable = Reflection.HasAttribute<ReconstructMultiformAttribute>(thisType);
        }

        public Multiform(string name)
        {
            Name = name;

            reconstructable = Reflection.HasAttribute<ReconstructMultiformAttribute>(this.GetType());
        }

        /// <summary>
        /// Called after the multiform is registered to a manager.
        /// </summary>
        /// <param name="manager"></param>
        internal void PostRegister(MultiformManager manager)
        {
            Manager = manager;
        }

        internal void DelegateConstruction()
        {
            TimesActivated++;
            if (reconstructable && TimesActivated > 1)
            {
                Reconstruct();
            }
            else
            {
                Construct();
            }
        }

        /// <summary>
        /// The main constructor for the multiform. This is called every time this multiform
        /// instance is switched to by the MultiformManager.
        /// </summary>
        public abstract void Construct();

        /// <summary>
        /// The auxiliary constructor called every time after the first time the multiform is
        /// activated. This is only used if the multiform is decorated with a ReconstructMultiform
        /// attribute.
        /// </summary>
        public virtual void Reconstruct() { }

        /// <summary>
        /// The deconstructor for the multiform. This is called when the multiform is deactivated.
        /// </summary>
        public virtual void Deconstruct() { }

        /// <summary>
        /// Deactivate this multiform.
        /// </summary>
        public void Deactivate()
        {
            Manager.Deactivate(this);
        }

        /// <summary>
        /// Set the current renderer for this multiform.
        /// </summary>
        /// <param name="action"></param>
        protected void SetRenderer(Action action)
        {
            renderer = action;
        }

        internal void Render()
        {
            renderer();
        }
    }
}
