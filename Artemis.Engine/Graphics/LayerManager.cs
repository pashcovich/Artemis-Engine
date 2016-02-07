#region Using Statements

using Artemis.Engine.Utilities.UriTree;

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Artemis.Engine.Graphics
{
    public class LayerManager : UriTreeObserver<AbstractLayer>
    {
        /// <summary>
        /// The list of names of layers to render.
        /// </summary>
        private List<string> RenderOrder;

        public LayerManager()
        {
            RenderOrder = new List<string>();
        }

        /// <summary>
        /// Add a layer.
        /// </summary>
        /// <param name="layer"></param>
        public void AddLayer(AbstractLayer layer)
        {
            // This will automatically set the layer's parent to the proper
            // parent in the UriTree.
            AddObservedNode(layer.tempFullName, layer);
        }

        /// <summary>
        /// Set the order in which to render the layers.
        /// </summary>
        /// <param name="names"></param>
        public void SetRenderOrder(params string[] names)
        {
            RenderOrder = names.ToList();
        }

        internal void Render()
        {
            // For now, remaining layers are just ignored.

            foreach (var name in RenderOrder)
            {
                var layer = GetObservedNode(name);
                if (layer.PreRender != null)
                {
                    layer.PreRender();
                }
                layer.Render();
                if (layer.PostRender != null)
                {
                    layer.PostRender();
                }
            }
        }
    }
}
