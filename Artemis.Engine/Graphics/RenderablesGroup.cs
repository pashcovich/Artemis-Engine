#region Using Statements

using Artemis.Engine.Utilities.UriTree;

#endregion

namespace Artemis.Engine.Graphics
{
    public sealed class RenderablesGroup : UriTreeGroup<RenderablesGroup, IRenderable>
    {
        public RenderablesGroup(string name) : base(name) { }
    }
}
