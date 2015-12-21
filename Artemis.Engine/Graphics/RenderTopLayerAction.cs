using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Artemis.Engine.Graphics
{
    public sealed class RenderTopLayerAction : AbstractRenderOrderAction
    {
        private string layerName;

        internal RenderTopLayerAction(string name)
        {
            layerName = name;
        }

        public override void Perform(LayerManager manager)
        {
            manager.GetObservedNode(layerName).RenderTop();
        }
    }
}
