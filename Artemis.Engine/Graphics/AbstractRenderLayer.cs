using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis.Engine.Utilities.UriTree;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artemis.Engine.Graphics
{

    /// <summary>
    /// A delegate defining what to do upon encountering a renderable
    /// object in a renderable group.
    /// </summary>
    /// <param name="?"></param>
    public delegate void RenderableHandler(RenderableObject obj);
    
    public class AbstractRenderLayer<T> : UriTreeObserverNode<T, RenderableGroup>
        where T : AbstractRenderLayer<T>
    {
        private const string TOP_LEVEL = "ALL"; // The name of the top level of renderable objects.

        private readonly List<RenderableToAdd> renderablesToAdd = new List<RenderableToAdd>();
        protected struct RenderableToAdd
        {
            public RenderableObject Object;
            public string Name;
            public bool Anonymous;
        }

        // the list of the renderables we've seen so far in this render cycle.
        protected readonly HashSet<RenderableObject> SeenRenderables = new HashSet<RenderableObject>();

        // the list of renderable groups we've seen so far in this render cycle.
        protected readonly HashSet<RenderableGroup> SeenGroups = new HashSet<RenderableGroup>();
        
        internal string tempFullName;

        /// <summary>
        /// The top RenderableGroup.
        /// </summary>
        public RenderableGroup AllRenderables { get; private set; }

        /// <summary>
        /// Whether or not we are in the middle of rendering this layer.
        /// </summary>
        public bool MidRender { get; private set; }

        /// <summary>
        /// Whether or not this render layer is managed by a LayerManager or not.
        /// </summary>
        public bool Managed { get; internal set; }

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

        /// <summary>
        /// The target we're rendering to.
        /// </summary>
        public RenderTarget2D LayerTarget { get; protected set; }

        public AbstractRenderLayer(string fullName)
            : base(UriUtilities.GetLastPart(fullName))
        {
            tempFullName = fullName;
            Managed = false;

            AllRenderables = new RenderableGroup(TOP_LEVEL);
            AddObservedNode(TOP_LEVEL, AllRenderables);

            LayerTarget = ArtemisEngine.RenderPipeline.CreateRenderTarget();

            GlobalRenderGroupOptions = RenderOrder.RenderGroupOptions.AllPre;
        }

        /// <summary>
        /// Add an empty group with the given name to the layer.
        /// </summary>
        /// <param name="name"></param>
        public void AddGroup(string name)
        {
            AllRenderables.AddSubnode(name, new RenderableGroup(UriUtilities.GetLastPart(name)));
        }

        /// <summary>
        /// Add the renderable group to the layer.
        /// </summary>
        /// <param name="group"></param>
        public void AddGroup(RenderableGroup group)
        {
            AllRenderables.AddSubnode(group.Name, group);
        }

        /// <summary>
        /// Add the renderable group with the given name to the layer.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="group"></param>
        public void AddGroup(string name, RenderableGroup group)
        {
            AllRenderables.AddSubnode(name, group);
        }

        /// <summary>
        /// Add a RenderableObject to this render layer.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        public void AddItem(string name, RenderableObject item)
        {
            if (MidRender)
                renderablesToAdd.Add(new RenderableToAdd { Name = name, Object = item });
            else
                AllRenderables.InsertItem(name, item);
        }

        /// <summary>
        /// Add a Form to this render layer.
        /// </summary>
        /// <param name="form"></param>
        public void AddItem(Form form)
        {
            AddItem(form.Name, form);
        }

        /// <summary>
        /// Add an anonymous item to this layer.
        /// </summary>
        /// <param name="item"></param>
        public void AddAnonymousItem(RenderableObject item)
        {
            if (MidRender)
                renderablesToAdd.Add(new RenderableToAdd { Name = null, Object = item, Anonymous = true });
            else
                AllRenderables.AddAnonymousItem(item);
        }

        /// <summary>
        /// Add an anonymous item to the renderable group with the given name in this layer.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="item"></param>
        public void AddAnonymousItem(string groupName, RenderableObject item)
        {
            if (MidRender)
                renderablesToAdd.Add(new RenderableToAdd { Name = groupName, Object = item, Anonymous = true });
            else
                AllRenderables.AddAnonymousItem(groupName, item);
        }

        /// <summary>
        /// Add an anonymous form to this layer.
        /// </summary>
        /// <param name="form"></param>
        public void AddAnonymousItem(Form form)
        {
            AddAnonymousItem((RenderableObject)form);
        }

        /// <summary>
        /// Add an anonymous form to the renderable group with the given name in this layer.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="form"></param>
        public void AddAnonymousItem(string groupName, Form form)
        {
            AddAnonymousItem(groupName, (RenderableObject)form);
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
        /// Render all items and sublayers on this layer.
        /// </summary>
        public void RenderAll(RenderOrder.RenderGroupOptions order = RenderOrder.RenderGroupOptions.AllPre)
        {
            MidRender = true;
            switch (order)
            {
                case RenderOrder.RenderGroupOptions.AllPre:
                    RenderSublayers(order);
                    RenderTop();
                    break;
                case RenderOrder.RenderGroupOptions.AllPost:
                    RenderTop();
                    RenderSublayers(order);
                    break;
                case RenderOrder.RenderGroupOptions.Top:
                    RenderTop();
                    break;
                default:
                    throw new RenderOrderException(
                        String.Format(
                            "Unknown RenderOrder.RenderGroupOptions '{0}' supplied when rendering " +
                            "layer '{1}'.", order, FullName));
            }
            MidRender = false;
        }
        
        /// <summary>
        /// Render only the sublayers of this layer.
        /// </summary>
        /// <param name="order"></param>
        private void RenderSublayers(RenderOrder.RenderGroupOptions order)
        {
            foreach (var layer in Subnodes.Values)
            {
                layer.RenderAll(order);
            }
        }

        /// <summary>
        /// Render only the items in this layer (none of the sublayers).
        /// </summary>
        public void RenderTop()
        {
            PreRender();

            SetupLayerTarget();

            PostSetupLayerTarget();

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

            RenderLayerTarget();

            PostRender();

            // Add all the items that were added mid-render.
            //
            // Honestly, this is kind of an unnecessary buffer for user clumsiness. Their
            // rendering code should NOT be adding anything to a layer, that SHOULD be happening
            // in their update code. This is just in case there's some edge case where you're
            // forced to add an item mid-render, or in case the user isn't astute enough to
            // realize the error in their ways.

            foreach (var item in renderablesToAdd)
            {
                if (item.Anonymous)
                {
                    if (item.Name != null)
                        AddAnonymousItem(item.Name, item.Object);
                    else
                        AddAnonymousItem(item.Object);
                }
                else
                {
                    AddItem(item.Name, item.Object);
                }
            }

            renderablesToAdd.Clear();
        }

        /// <summary>
        /// Called before anything is rendered.
        /// </summary>
        protected virtual void PreRender() { }

        /// <summary>
        /// Set up the layer's target.
        /// </summary>
        protected virtual void SetupLayerTarget()
        {
            ArtemisEngine.RenderPipeline.ClearRenderProperties();

            ArtemisEngine.RenderPipeline.SetRenderTarget(LayerTarget);
            ArtemisEngine.RenderPipeline.ClearGraphicsDevice(Color.Transparent);
        }

        /// <summary>
        /// Called directly after "SetupLayerTarget" and directly before any rendering.
        /// </summary>
        protected virtual void PostSetupLayerTarget() { }

        /// <summary>
        /// Get the RenderableHandler, which determines how to handle each encountered
        /// RenderableObject.
        /// </summary>
        /// <returns></returns>
        protected abstract RenderableHandler GetRenderableHandler();

        /// <summary>
        /// Render this layer's target. This is called immediately after all the Renderables
        /// have been rendered, and immediately before PostRender.
        /// </summary>
        protected virtual void RenderLayerTarget()
        {
            ArtemisEngine.RenderPipeline.UnsetRenderTarget();
            ArtemisEngine.RenderPipeline.ClearRenderProperties();

            ArtemisEngine.RenderPipeline.SetRenderProperties(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend);

            ArtemisEngine.RenderPipeline.Render(LayerTarget, Vector2.Zero);

            ArtemisEngine.RenderPipeline.ClearRenderProperties();
        }

        /// <summary>
        /// Called after everything is rendered (also after RenderLayerTarget is called).
        /// </summary>
        protected virtual void PostRender() { }

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
