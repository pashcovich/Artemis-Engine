#region Using Statements

using Artemis.Engine.Utilities;
using Artemis.Engine.Utilities.UriTree;

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Artemis.Engine.Graphics
{
    public class LayerManager : UriTreeObserver<AbstractRenderLayer>
    {
        /// <summary>
        /// The order in which the layers are rendered.
        /// </summary>
        public RenderOrder RenderOrder { get; private set; }

        /// <summary>
        /// The global RenderTraversalOptions, determining whether or not the top level items of each layer should be 
        /// rendered first or the sublayers first. The default value is "RenderOrder.RenderTraversalOptions.AllPre".
        /// 
        /// Note: If RenderOrder is not null, this value is not used.
        /// </summary>
        public TraversalOptions GlobalTraversalOptions;

        private Dictionary<Type, LayerManagerRenderOrderActionHandler> UnknownRenderOrderActionHandlers
            = new Dictionary<Type, LayerManagerRenderOrderActionHandler>();

        public LayerManager() 
        {
            GlobalTraversalOptions = TraversalOptions.Pre;
        }

        /// <summary>
        /// Add a given layer.
        /// </summary>
        /// <param name="layer"></param>
        public void Add(AbstractRenderLayer layer)
        {
            AddObservedNode(layer.tempFullName, layer);
            layer.Managed = true;
        }

        /// <summary>
        /// Set the render order to render the given layers (with "AllPre" traversal).
        /// </summary>
        /// <param name="order"></param>
        public void SetRenderOrder(params string[] order)
        {
            SetRenderOrder(order);
        }

        /// <summary>
        /// Set the render order to render the given layers with the given traversal option.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="traversal"></param>
        public void SetRenderOrder( string[] order
                                  , TraversalOptions traversal = TraversalOptions.Pre)
        {
            var actions = from name in order
                          select (RenderOrder.AbstractRenderOrderAction)
                                new RenderOrder.RenderLayer(name, traversal);
            SetRenderOrder(new RenderOrder(actions.ToList()));
        }

        /// <summary>
        /// Set the render order to render the given layers with the given traversal options.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="traversalOptions"></param>
        public void SetRenderOrder( string[] order 
                                  , TraversalOptions[] traversalOptions )
        {
            if (order.Length != traversalOptions.Length)
            {
                throw new RenderOrderException();
            }
            var actions = new List<RenderOrder.AbstractRenderOrderAction>();
            for (int i = 0; i < order.Length; i++)
            {
                actions.Add(new RenderOrder.RenderLayer(order[i], traversalOptions[i]));
            }
            SetRenderOrder(new RenderOrder(actions.ToList()));
        }

        /// <summary>
        /// Set the render order to perform the given AbstractRenderOrderActions.
        /// </summary>
        /// <param name="actions"></param>
        public void SetRenderOrder(params RenderOrder.AbstractRenderOrderAction[] actions)
        {
            SetRenderOrder(new RenderOrder(actions.ToList()));
        }

        /// <summary>
        /// Set the RenderOrder to the given RenderOrder object.
        /// </summary>
        /// <param name="order"></param>
        public void SetRenderOrder(RenderOrder order)
        {
            RenderOrder = order;
        }

        /// <summary>
        /// Register a custom RenderOrderActionHandler to be invoked when an 
        /// AbstractRenderOrderAction of type "T" is encountered.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void RegisterRenderOrderActionHandler<T>(LayerManagerRenderOrderActionHandler handler)
            where T : RenderOrder.AbstractRenderOrderAction
        {
            RegisterRenderOrderActionHandler(typeof(T), handler);
        }

        /// <summary>
        /// Register a custom RenderOrderActionHandler to be invoked when an 
        /// AbstractRenderOrderAction of the given type is encountered.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="handler"></param>
        public void RegisterRenderOrderActionHandler(Type type, LayerManagerRenderOrderActionHandler handler)
        {
            if (!type.IsSubclassOf(typeof(RenderOrder.AbstractRenderOrderAction)))
            {
                throw new RenderOrderException(
                    String.Format(
                        "Cannot specify RenderOrderAction handler for type '{0}'." +
                        "The given type must be a subclass of '{1}'.",
                        type, typeof(RenderOrder.AbstractRenderOrderAction)));
            }
            UnknownRenderOrderActionHandlers.Add(type, handler);
        }

        /// <summary>
        /// Render every layer.
        /// </summary>
        public void Render()
        {
            var seenLayers = new HashSet<AbstractRenderLayer>();
            if (RenderOrder == null)
            {
                foreach (var layer in ObservedNodes.Values)
                {
                    layer.Render(seenLayers);
                }
            }
            else
            {
                foreach (var action in RenderOrder.Actions)
                {
                    switch (action.ActionType)
                    {
                        case RenderOrder.RenderOrderActionType.RenderLayer:
                            HandleRenderOrderAction_RenderLayer((RenderOrder.RenderLayer)action, seenLayers);
                            break;
                        default:
                            var handled = false;
                            if (UnknownRenderOrderActionHandlers.Count > 0)
                            {
                                var type = action.GetType();
                                if (UnknownRenderOrderActionHandlers.ContainsKey(type))
                                {
                                    UnknownRenderOrderActionHandlers[type](this, action);
                                    handled = true;
                                }
                            }
                            if (!handled)
                                HandleUnknownRenderOrderAction(action);
                            break;
                    }
                }
            }
        }

        private void HandleRenderOrderAction_RenderLayer(
            RenderOrder.RenderLayer action, HashSet<AbstractRenderLayer> seenLayers)
        {
            var layer = GetObservedNode(action.Name);
            layer.Render(seenLayers, action.Options, action.SkipDuplicates);
        }

        /// <summary>
        /// Handle a RenderOrderAction that the LayerManager couldn't identify what to do with.
        /// Implement this if you have RenderOrderActions that aren't one of the builtin types.
        /// </summary>
        /// <param name="action"></param>
        protected virtual void HandleUnknownRenderOrderAction(RenderOrder.AbstractRenderOrderAction action)
        {
            var name = Enum.GetName(typeof(RenderOrder.RenderOrderActionType), action.ActionType);
            throw new RenderOrderException(
                string.Format(
                    "The RenderOrderAction '{0}' cannot be used in the RenderOrder for a LayerManager.",
                    name == null ? action.ActionType : (object)name, action.ActionType));
        }
    }
}
