#region Using Statements

using Artemis.Engine.Utilities.UriTree;

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Artemis.Engine.Graphics
{
    public class LayerManager : UriTreeObserver<AbstractRenderLayer>
    {

        public List<string> RenderOrder;

        public LayerManager()
        {
            RenderOrder = new List<string>();
        }

        public void Add(RenderLayer layer)
        {
            AddObservedNode(layer.tempFullName, layer);
            layer.Managed = true;
        }

        public void SetRenderOrder(params string[] order)
        {
            RenderOrder = order.ToList();
        }

        public void Render()
        {
            foreach (var layerName in RenderOrder)
            {
                GetObservedNode(layerName).RenderTop();
            }
        }
    }
}
