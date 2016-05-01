#region Using Statements

using Artemis.Engine.Graphics;

#endregion

namespace Artemis.Engine
{
    public abstract class Form : RenderableObject
    {
        public string Name { get; private set; }

        public bool Anonymous { get { return Name != null; } }
    }
}
