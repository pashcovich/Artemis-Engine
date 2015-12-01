using Artemis.Engine.Utilities.Groups;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Artemis.Engine.Graphics
{
    public sealed class LayerManager
    {

        /// <summary>
        /// The dictionary of all managed layers. This maps from a name to all top-level
        /// layers (top-level means if "a" and "a.b" are added to the manager, then only
        /// "a" will be stored in this dictionary and "a.b" will be stored as a sublayer
        /// of "a").
        /// </summary>
        public Dictionary<string, RenderLayer> ManagedLayers { get; private set; }

        /// <summary>
        /// The order in which layers and layer groups are to be rendered.
        /// </summary>
        public List<string> RenderOrder { get; private set; }

        /// <summary>
        /// Whether or not a RenderOrder has been supplied.
        /// </summary>
        public bool RenderOrderSet { get { return RenderOrder != null; } }

        /// <summary>
        /// The list of remaining layers not included in the RenderOrder.
        /// </summary>
        private List<RenderLayer> RemainingLayers = new List<RenderLayer>();

        /// <summary>
        /// The action that determines what to do with the remaining layers.
        /// </summary>
        private RemainingLayersAction RemainingLayersAction = RemainingLayersAction.Ignore;

        /// <summary>
        /// The final order in which layers are to be rendered. We include this because there
        /// are certain things that have to be performed before actually rendering the layers
        /// (such as checking for duplicates and performing the appropriate action), and doing
        /// that every single time "Render" gets called would be wasteful.
        /// </summary>
        private List<RenderLayer> FinalLayerOrder = new List<RenderLayer>();

        public LayerManager() 
        {
            ManagedLayers = new Dictionary<string, RenderLayer>();
        }

        /// <summary>
        /// Add a layer to the manager.
        /// </summary>
        /// <param name="layer"></param>
        public void Add(RenderLayer layer)
        {
            var parts = UriUtilities.GetParts(layer.tempFullName);
            if (parts.Length == 1)
            {
                ManagedLayers.Add(parts[0], layer);
            }
            ManagedLayers[parts[0]].AddSubgroup(
                String.Join(
                    UriUtilities.URI_SEPARATOR.ToString(), 
                    parts.Skip(1)), 
                layer);
        }

        /// <summary>
        /// Set the order in which the layers are to be rendered.
        /// </summary>
        /// <param name="layers"></param>
        public void SetRenderOrder(params string[] layers)
        {
            if (ArtemisEngine.RenderPipeline.BegunRenderCycle)
            {
                throw new LayerManagerException(
                    "Cannot change render order during render cycle.");
            }

            // Reset everything first...
            ResetRenderOrder();

            // Set the render order...
            RenderOrder = layers.ToList();

            // Collect the layers not included in the render order...
            CollectRemainingLayers();

            // Finally, determine the actual (final) order in which layers
            // are to be rendered.
            DetermineFinalRenderOrder();
        }

        /// <summary>
        /// Reset the necessary lists for maintaining render order.
        /// </summary>
        private void ResetRenderOrder()
        {
            RemainingLayers.Clear();
            FinalLayerOrder.Clear();
        }

        /// <summary>
        /// Collect the list of remaining layers not included in the render order.
        /// </summary>
        private void CollectRemainingLayers()
        {
            var known = new HashSet<string>(RenderOrder);
            foreach (var kvp in ManagedLayers)
            {
                var name = kvp.Key;
                var layer = kvp.Value;

                RecursiveCollectRemainingLayers(layer, known);
            }
        }

        /// <summary>
        /// Recursively collect all the remaining sublayers of a given
        /// layer not included in the render order.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="known"></param>
        private void RecursiveCollectRemainingLayers(RenderLayer layer, HashSet<string> known)
        {
            if (!known.Contains(layer.FullName))
            {
                if (layer.IsLeaf)
                {
                    RemainingLayers.Add(layer);
                }
                else
                {
                    RecursiveCollectRemainingLayers(layer, known);
                }
            }
            // If the layer is not a leaf node, and it's name
            // is known, then all of it's sublayers will be
            // rendered as well.
        }

        /// <summary>
        /// Setup the final list of layers in the exact order they are to
        /// be rendered.
        /// </summary>
        private void DetermineFinalRenderOrder()
        {
            var visited = new HashSet<string>();

            foreach (var name in RenderOrder)
            {
                var layer = GetLayer(name);
                if (visited.Contains(name) && layer.TimesRendered <= 2)
                {
                    // If the TimesRendered is > 2, then the layer has already
                    // passed through this rigmarole.
                    var multiRenderAction = layer.MultiRenderAction;

                    switch (multiRenderAction)
                    {
                        case MultiRenderAction.Ignore:
                            layer.TimesQueuedForRendering++;
                            continue;
                        case MultiRenderAction.Fail:
                            throw new MultiRenderException(
                                String.Format(
                                    "The layer with full name '{0}' was rendered multiple times.", layer.FullName
                                    )
                                );
                        case MultiRenderAction.RenderAgain:
                            break;
                        case MultiRenderAction.LogWarn:
                        case MultiRenderAction.LogCritical:
                            // do logging stuff.
                            break;
                    }
                }

                layer.TimesRendered++;
                layer.TimesQueuedForRendering++;

                FinalLayerOrder.Add(layer);
                visited.Add(name);
            }
        }

        /// <summary>
        /// Return a layer with the given full name.
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public RenderLayer GetLayer(string fullName)
        {
            if (!fullName.Contains(UriUtilities.URI_SEPARATOR))
            {
                return ManagedLayers[fullName];
            }
            var parts = UriUtilities.GetParts(fullName);
            return ManagedLayers[parts[0]].GetSubgroup(parts.Skip(1).ToArray(), false);
        }

        /// <summary>
        /// Set the RemainingLayersAction (i.e. what to do with the remaining layers
        /// not included in the RenderOrder). By default, this is set to Ignore.
        /// </summary>
        /// <param name="action"></param>
        public void SetRemainingLayersAction(RemainingLayersAction action)
        {
            RemainingLayersAction = action;
        }

        /// <summary>
        /// Render everything.
        /// </summary>
        internal void Render()
        {
            foreach (var layer in FinalLayerOrder)
            {
                layer.Render();
            }

            foreach (var layer in RemainingLayers)
            {
                layer.Render();
            }
        }
    }
}
