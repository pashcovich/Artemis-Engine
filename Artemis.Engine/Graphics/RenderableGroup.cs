#region Using Statements

using Artemis.Engine.Utilities.UriTree;

using System;
using System.Collections.Generic;

#endregion

namespace Artemis.Engine.Graphics
{
    public class RenderableGroup : UriTreeMutableGroup<RenderableGroup, RenderableObject>
    {

        public RenderableGroup(string name) : base(name) 
        {
            OnItemAdded += OnRenderableAdded;
        }

        private void OnRenderableAdded(string name, RenderableObject obj)
        {
            obj._group = this;
            obj._name = name;
        }

        /// <summary>
        /// Return all top level renderable objects in this group (i.e. no objects that
        /// belong to subgroups).
        /// </summary>
        /// <returns></returns>
        public virtual List<RenderableObject> RetrieveAllTop()
        {
            return new List<RenderableObject>(Items.Values);
        }

        /// <summary>
        /// Recursively retrieve all the RenderableObjects.
        /// </summary>
        /// <returns></returns>
        public virtual List<RenderableObject> RetrieveAll()
        {
            var list = new List<RenderableObject>();

            RetrieveAll(list);

            return list;
        }

        private void RetrieveAll(List<RenderableObject> renderables)
        {
            foreach (var subgroup in Subnodes.Values)
            {
                subgroup.RetrieveAll(renderables);
            }
            foreach (var item in Items.Values)
            {
                renderables.Add(item);
            }
        }

        /// <summary>
        /// Render only the top level objects in this group (i.e. the objects that aren't members of subnodes).
        /// </summary>
        /// <param name="renderAction">The layer's RenderAction, which determines how each RenderableObject encountered is handled and rendered.</param>
        /// <param name="seenRenderables">The list of RenderableObjects already seen by the layer in this render cycle.</param>
        /// <param name="skipDuplicates">Whether or not to skip duplicate items (if they're present in the seenRenderables list).</param>
        public virtual void RenderTop( RenderableHandler renderAction
                                     , HashSet<RenderableObject> seenRenderables
                                     , bool skipDuplicates = true)
        {
            foreach (var item in Items.Values)
            {
                if (skipDuplicates && seenRenderables.Contains(item))
                    continue;
                renderAction(item);
            }
        }

        /// <summary>
        /// Render all objects.
        /// </summary>
        /// <param name="options">The options determining which order the items and subgroups are rendered in.</param>
        /// <param name="renderAction">The layer's RenderAction, which determines how each RenderableObject encountered is handled and rendered.</param>
        /// <param name="seenRenderables">The list of RenderableObjects already seen by the layer in this render cycle.</param>
        /// <param name="seenGroups">The list of RenderableGroups already seen by the layer in this render cycle.</param>
        /// <param name="skipDuplicates">Whether or not to skip duplicate items/subgroups (if they're present in the seenRenderables/seenGroups lists).</param>
        public virtual void Render( RenderOrder.RenderTraversalOptions options
                                  , RenderableHandler renderAction
                                  , HashSet<RenderableObject> seenRenderables
                                  , HashSet<RenderableGroup> seenGroups
                                  , bool skipDuplicates = true )
        {
            switch (options)
            {
                case RenderOrder.RenderTraversalOptions.Top:
                    if (!(skipDuplicates && seenGroups.Contains(this)))
                    {
                        RenderTop(renderAction, seenRenderables, skipDuplicates);
                    }
                    break;
                case RenderOrder.RenderTraversalOptions.AllPre:
                    foreach (var subgroup in Subnodes.Values)
                    {
                        subgroup.Render(options, renderAction, seenRenderables, seenGroups, skipDuplicates);
                    }
                    if (!(skipDuplicates && seenGroups.Contains(this)))
                    {
                        RenderTop(renderAction, seenRenderables, skipDuplicates);
                    }
                    break;
                case RenderOrder.RenderTraversalOptions.AllPost:
                    if (!(skipDuplicates && seenGroups.Contains(this)))
                        RenderTop(renderAction, seenRenderables, skipDuplicates);
                    foreach (var subgroup in Subnodes.Values)
                    {
                        subgroup.Render(options, renderAction, seenRenderables, seenGroups, skipDuplicates);
                    }
                    break;
                default:
                    throw new RenderOrderException(
                        String.Format(
                            "Unknown RenderOrder.RenderGroupOptions '{0}' supplied when rendering " +
                            "renderable group '{1}'.", options, FullName));
            }
            seenGroups.Add(this);
        }
    }
}
