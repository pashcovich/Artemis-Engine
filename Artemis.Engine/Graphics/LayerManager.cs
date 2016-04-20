using Artemis.Engine.Utilities.UriTree;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Artemis.Engine.Graphics
{
    public class LayerManager : UriTreeObserver<RenderLayer>
    {

        public List<string> RenderOrder;

        public LayerManager()
        {
            RenderOrder = new List<string>();
        }

        public void Add(RenderLayer layer)
        {
            AddObservedNode(layer.tempName, layer);
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
                GetObservedNode(layerName).Render();
            }
        }
    }
}
