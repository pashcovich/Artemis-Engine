#region Using Statements

using Artemis.Engine.Utilities.UriTree;

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Artemis.Engine.Graphics
{
    public class LayerManager : UriTreeObserver<RenderLayer>
    {

        private List<string> RenderOrder;

        public void AddLayer(RenderLayer layer)
        {
            AddObservedNode(layer.tempFullName, layer);
        }

        public void SetRenderOrder(params string[] names)
        {
            RenderOrder = names.ToList();
        }

        internal void Render()
        {
            foreach (var name in RenderOrder)
            {
                GetObservedNode(name).Render();
            }
        }
    }
}
