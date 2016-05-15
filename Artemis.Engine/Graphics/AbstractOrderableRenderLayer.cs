#region Using Statements

using Artemis.Engine.Utilities.UriTree;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Artemis.Engine.Graphics
{

    /// <summary>
    /// A delegate defining what to do upon encountering a renderable
    /// object in a renderable group.
    /// </summary>
    /// <param name="?"></param>
    public delegate void RenderableHandler(RenderableObject obj);
    
    public abstract class AbstractOrderableRenderLayer : AbstractRenderLayer
    {
        // the list of the renderables we've seen so far in this render cycle.
        protected readonly HashSet<RenderableObject> SeenRenderables = new HashSet<RenderableObject>();

        // the list of renderable groups we've seen so far in this render cycle.
        protected readonly HashSet<RenderableGroup> SeenGroups = new HashSet<RenderableGroup>();

        /// <summary>
        /// The order in which objects get rendered.
        /// </summary>
        public RenderOrder RenderOrder { get; private set; }

        /// <summary>
        /// The global RenderGroupOptions, determining whether or not the top level items should be 
        /// rendered first or the subnodes first. The default value is "RenderOrder.RenderGroupOptions.AllPre".
        /// 
        /// Note: If RenderOrder is not null, this value is not used.
        /// </summary>
        public RenderOrder.RenderGroupOptions GlobalRenderGroupOptions;

        /// <summary>
        /// The list of actions representing the render order.
        /// </summary>
        public List<RenderOrder.AbstractRenderOrderAction> RenderOrderActions
        {
            get
            {
                if (RenderOrder == null)
                    return new List<RenderOrder.AbstractRenderOrderAction>();
                return RenderOrder.Actions;
            }
        }

        public AbstractOrderableRenderLayer(string fullName)
            : base(UriUtilities.GetLastPart(fullName))
        {
            GlobalRenderGroupOptions = RenderOrder.RenderGroupOptions.AllPre;
        }

        /// <summary>
        /// Set the layer's RenderOrder to the render the groups/items with the given names.
        /// </summary>
        /// <param name="type">The type that each name represents (item or group).</param>
        /// <param name="names"></param>
        public void SetRenderOrder(RenderOrder.RenderType type, params string[] names)
        {
            var renderOrder = new RenderOrder();
            switch (type)
            {
                case RenderOrder.RenderType.Item:
                    foreach (var name in names)
                        renderOrder.AddRenderItem(name);
                    break;
                case RenderOrder.RenderType.Group:
                    foreach (var name in names)
                        renderOrder.AddRenderGroup(name);
                    break;
                default:
                    throw new RenderOrderException(
                        String.Format(
                            "Can't handle RenderOrder.RenderType '{0}'" +
                            " for more options see the other overloads of `SetRenderOrder`.", type
                            )
                        );
            }
            SetRenderOrder(renderOrder);
        }

        /// <summary>
        /// Set the render order to the render the given list of groups/items with the given
        /// names and types.
        /// </summary>
        /// <param name="types"></param>
        /// <param name="names"></param>
        public void SetRenderOrder(RenderOrder.RenderType[] types, string[] names)
        {
            if (types.Length != names.Length)
                throw new RenderOrderException(
                    "The length of the given types array must match the length of the given names array.");

            var renderOrder = new RenderOrder();
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i] == RenderOrder.RenderType.Item)
                    renderOrder.AddRenderItem(names[i]);
                else if (types[i] == RenderOrder.RenderType.Group)
                    renderOrder.AddRenderGroup(names[i]);
            }
            SetRenderOrder(renderOrder);
        }

        /// <summary>
        /// Set the render order to the given list of AbstractRenderOrderActions.
        /// </summary>
        /// <param name="actions"></param>
        public void SetRenderOrder(params RenderOrder.AbstractRenderOrderAction[] actions)
        {
            SetRenderOrder(new RenderOrder(actions.ToList()));
        }

        /// <summary>
        /// Set the render order to the given RenderOrder object.
        /// </summary>
        /// <param name="order"></param>
        public void SetRenderOrder(RenderOrder order)
        {
            if (MidRender)
                throw new RenderOrderException(
                    String.Format(
                        "Cannot set render order on layer '{0}' during the render cycle.", FullName));
            RenderOrder = order;

            // Calculate render order.
        }

        /// <summary>
        /// Get the RenderableHandler, which determines how to handle each encountered
        /// RenderableObject.
        /// </summary>
        /// <returns></returns>
        protected abstract RenderableHandler GetRenderableHandler();

        /// <summary>
        /// Actually render the RenderableObjects.
        /// </summary>
        protected override void RenderItems()
        {
            var renderableHandler = GetRenderableHandler();

            SeenRenderables.Clear();
            SeenGroups.Clear();

            // If no RenderOrder was set, just render everything.
            if (RenderOrder == null)
            {
                AllRenderables.Render(
                    GlobalRenderGroupOptions, renderableHandler, SeenRenderables, SeenGroups);
            }
            // Otherwise, render in the order specified by the RenderOrder.
            else
            {
                /* TODO: Make this faster (somehow).
                 * 
                 * The problem with this is that we're iterating through EVERY RenderableObject and checking
                 * if the camera can see it, then rendering it. Ideally what we should be doing instead is
                 * iterating through every RenderableObject returned by the QueryAABB and somehow rendering
                 * only THOSE in order.
                 * 
                 * For large worlds, the number of renderables visible to the camera can be MUCH less than the
                 * total number of RenderableObjects, which is a problem.
                 * 
                 * Most likely what will be done is upon setting the layer's RenderOrder, some sort of mapping
                 * is created which assigns each RenderableObject a number indicating it's position in the sorted
                 * list of objects to render.
                 */
                foreach (var item in RenderOrder.Actions)
                {
                    switch (item.ActionType)
                    {
                        case RenderOrder.RenderOrderActionType.RenderItem:
                            HandleRenderOrderAction_RenderItem((RenderOrder.RenderItem)item, renderableHandler);
                            break;
                        case RenderOrder.RenderOrderActionType.RenderGroup:
                            HandleRenderOrderAction_RenderGroup((RenderOrder.RenderGroup)item, renderableHandler);
                            break;
                        case RenderOrder.RenderOrderActionType.SetRenderProperties:
                            HandleRenderOrderAction_SetRenderProperties((RenderOrder.SetRenderProperties)item);
                            break;
                        default:
                            HandleUnknownRenderOrderAction(item, renderableHandler);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Handle a RenderItem action in the RenderOrder.
        /// </summary>
        /// <param name="action"></param>
        private void HandleRenderOrderAction_RenderItem(
            RenderOrder.RenderItem action, RenderableHandler renderAction)
        {
            var renderable = AllRenderables.GetItem(action.Name);
            if (!(action.SkipDuplicates && SeenRenderables.Contains(renderable)))
                renderAction(renderable);
        }

        /// <summary>
        /// Handle a RenderGroup action in the RenderOrder.
        /// </summary>
        /// <param name="action"></param>
        private void HandleRenderOrderAction_RenderGroup(
            RenderOrder.RenderGroup action, RenderableHandler renderAction)
        {
            var group = AllRenderables.GetSubnode(action.Name);
            group.Render(action.Options, renderAction, SeenRenderables, SeenGroups, action.SkipDuplicates);
        }

        /// <summary>
        /// Handle a SetRenderProperties action in the RenderOrder.
        /// </summary>
        /// <param name="action"></param>
        private void HandleRenderOrderAction_SetRenderProperties(RenderOrder.SetRenderProperties action)
        {
            ArtemisEngine.RenderPipeline.SetRenderProperties(
                action.Packet, action.IgnoreDefaults, action.ApplyMatrix);
        }

        /// <summary>
        /// Handle a RenderOrderAction that the RenderLayer couldn't identify what to do with.
        /// Implement this if you have RenderOrderActions that aren't one of the builtin types.
        /// </summary>
        /// <param name="action"></param>
        protected virtual void HandleUnknownRenderOrderAction(
            RenderOrder.AbstractRenderOrderAction action, RenderableHandler renderAction)
        {
            throw new RenderOrderException(
                String.Format("Unknown render order action '{0}'.", action));
        }
    }
}
