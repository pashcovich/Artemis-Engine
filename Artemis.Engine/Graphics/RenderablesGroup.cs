#region Using Statements

using Artemis.Engine.Utilities;
using Artemis.Engine.Utilities.UriTree;

#endregion

namespace Artemis.Engine.Graphics
{
    public sealed class RenderablesGroup : UriTreeMutableGroup<RenderablesGroup, IRenderable>
    {
        public RenderablesGroup(string name) : base(name) { }

        public void Render(AbstractLayer layer)
        {
            foreach (var subnode in Subnodes.Values)
            {
                subnode.Render(layer);
            }

            foreach (var item in Items.Values)
            {
                layer.RenderIRenderable(item);
            }
        }
    }
}
