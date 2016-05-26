#region Using Statements

using Artemis.Engine.Graphics;

#endregion

namespace Artemis.Engine
{
    public abstract class Form : PhysicalObject
    {
        /// <summary>
        /// The name of this form.
        /// </summary>
        public string Name;

        /// <summary>
        /// Whether or not this form has a name.
        /// </summary>
        public bool Anonymous { get { return Name != null; } }
    }
}
