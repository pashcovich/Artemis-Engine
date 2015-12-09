#region Using Statements

using Artemis.Engine.Utilities;

using System;

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
        /// The current renderer for the multiform.
        /// </summary>
        private Action renderer;

        public Multiform() : base()
        {
            var thisType = this.GetType();
            Name = Reflection.GetFirstAttribute<NamedMultiformAttribute>(thisType).Name;
        }

        public Multiform(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The main constructor for the multiform. This is called every time this multiform
        /// instance is switched to by the MultiformManager.
        /// </summary>
        public abstract void Construct();

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
