#region Using Statements

using Artemis.Engine.Utilities.UriTree;

using System;

#endregion

namespace Artemis.Engine.Graphics
{
    public class RenderablesGroup : UriTreeGroup<RenderablesGroup, IRenderable>
    {
        public RenderablesGroup(string name) : base(name) { }
    }

    public class RenderLayer : UriTreeObserverNode<RenderLayer, RenderablesGroup>
    {
        internal string tempFullName;

        public RenderLayer(string fullName)
            : base(UriUtilities.GetLastPart(fullName))
        {
            tempFullName = fullName;
        }

        public void Render()
        {

        }
    }
}
