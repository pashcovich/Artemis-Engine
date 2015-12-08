using Artemis.Engine.Utilities;
using System;

namespace Artemis.Engine
{
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

        public abstract void Construct();

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
