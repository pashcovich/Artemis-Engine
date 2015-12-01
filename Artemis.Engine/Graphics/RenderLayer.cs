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

        /// <summary>
        /// What to do if the LayerManager encounters this layer multiple times when
        /// determining the FinalLayerOrder.
        /// </summary>
        public MultiRenderAction MultiRenderAction = MultiRenderAction.Ignore;

        /// <summary>
        /// The number of times the layer gets rendered.
        /// </summary>
        public int TimesRendered { get; internal set; }

        /// <summary>
        /// This is the number of times the LayerManager encountered this layer when
        /// determining the FinalLayerOrder, which is sometimes greater than the 
        /// TimesRendered since this will be incremented if the layer is encountered
        /// again but ignored, whereas the TimesRendered will only be incremented if
        /// the layer actually gets rendered.
        /// </summary>
        public int TimesQueuedForRendering { get; internal set; }

        public RenderLayer(string name)
            : base(UriUtilities.GetFirstPart(name))
        {
            // The name given is technically the FullName of the layer,
            // but since we don't directly add it to it's parent group
            // (that happens in LayerManager), we have to store the full
            // name temporarily.
            tempFullName = name;

            TimesRendered = 0;
            TimesQueuedForRendering = 0;
        }

        /// <summary>
        /// Render everything in this layer and it's sublayers.
        /// </summary>
        internal void Render() { }

    }
}
