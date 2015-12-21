using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Artemis.Engine.Graphics
{
    public class RenderOrderAction : AbstractRenderOrderAction
    {
        private Action<LayerManager> action;

        public RenderOrderAction(Action<LayerManager> action)
        {
            this.action = action;
        }

        public override void Perform(LayerManager manager)
        {
            action(manager);
        }
    }
}
