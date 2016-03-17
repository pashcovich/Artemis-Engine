using Artemis.Engine.Utilities.UriTree;

using System.Collections.Generic;

namespace Artemis.Engine.Graphics
{
    public class RenderableGroup : UriTreeGroup<RenderableGroup, AbstractRenderable>
    {
        public RenderableGroup(string name) : base(name) { }

        public List<AbstractRenderable> RetrieveAll()
        {
            var list = new List<AbstractRenderable>();

            RetrieveAll(list);

            return list;
        }

        private void RetrieveAll(List<AbstractRenderable> renderables)
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
