using Artemis.Engine.Utilities.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Artemis.Engine.Graphics
{
    public class RenderLayer : UriGroup<RenderLayer, INullRenderable>
    {

        internal string tempFullName { get; private set; }

        public MultiRenderAction MultiRenderAction = MultiRenderAction.Ignore;

        public RenderLayer(string name)
            : base(UriUtilities.GetFirstPart(name))
        {
            // The name given is technically the FullName of the layer,
            // but since we don't directly add it to it's parent group
            // (that happens in LayerManager), we have to store the full
            // name temporarily.
            tempFullName = name;

        }

    }
}
