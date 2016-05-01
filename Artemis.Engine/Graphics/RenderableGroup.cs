#region Using Statements

using Artemis.Engine.Utilities.UriTree;

using System.Collections.Generic;

#endregion

namespace Artemis.Engine.Graphics
{
    public class RenderableGroup : UriTreeGroup<RenderableGroup, RenderableObject>
    {
        public RenderableGroup(string name) : base(name) { }

        public List<RenderableObject> RetrieveAll()
        {
            var list = new List<RenderableObject>();

            RetrieveAll(list);

            return list;
        }

        private void RetrieveAll(List<RenderableObject> renderables)
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
