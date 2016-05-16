#region Using Statements

using Artemis.Engine.Graphics;
using Artemis.Engine.Multiforms;

#endregion

namespace Artemis.Engine
{
    public abstract class Form : PhysicalObject
    {
        /// <summary>
        /// The name of this form.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Whether or not this form has a name.
        /// </summary>
        public bool Anonymous { get { return Name != null; } }

        /// <summary>
        /// The parent of this form.
        /// </summary>
        public Multiform Parent { get; internal set; }

        /// <summary>
        /// Whether or not this form is managed by a Multiform (i.e. if Parent != null).
        /// </summary>
        public bool Managed { get { return Parent != null; } }

        public Form(string name) : base() { Name = name; }
    }
}
