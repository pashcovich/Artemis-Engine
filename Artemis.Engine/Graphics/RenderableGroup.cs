using Artemis.Engine.Utilities.UriTree;

using System.Collections.Generic;

namespace Artemis.Engine.Graphics
{
    public class RenderableGroup : UriTreeGroup<RenderableGroup, IRenderable>
    {
        public RenderableGroup(string name) : base(name) { }

        public List<IRenderable> RetrieveAll()
        {
            var list = new List<IRenderable>();

            RetrieveAll(list);

            return list;
        }

        private void RetrieveAll(List<IRenderable> renderables)
        {
            foreach (var subgroup in Subnodes.Values)
            {
                subgroup.RetrieveAll(renderables);
            }

            foreach (var item in Items.Values)
            {
                renderables.Add(item);
            }
        }
    }
}
