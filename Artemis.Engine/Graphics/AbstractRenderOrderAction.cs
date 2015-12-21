using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Artemis.Engine.Graphics
{
    public abstract class AbstractRenderOrderAction
    {
        public abstract void Perform(LayerManager manager);
    }
}
