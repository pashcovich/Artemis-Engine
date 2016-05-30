#region Using Statements

using Artemis.Engine.Utilities;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;

#endregion

namespace Artemis.Engine.Graphics
{

    /// <summary>
    /// Delegate invoked upon encountering an AbstractRenderOrderAction with a given type in a Layer object.
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="action"></param>
    /// <param name="renderableHandler"></param>
    public delegate void LayerRenderOrderActionHandler(
        AbstractOrderableRenderLayer layer, RenderOrder.AbstractRenderOrderAction action, RenderableHandler renderableHandler);

    /// <summary>
    /// Delegate invoked upon encountering an AbstractRenderOrderAction with a given type in a LayerManager object.
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="action"></param>
    public delegate void LayerManagerRenderOrderActionHandler(
        LayerManager manager, RenderOrder.AbstractRenderOrderAction action);

    public class RenderOrder
    {
        internal enum RenderOrderActionType
        {
            RenderItem,
            RenderGroup,
            RenderLayer,
            SetRenderProperties
        }

        /// <summary>
        /// Determines what type of object a `Render` action is associated with.
        /// </summary>
        public enum RenderType
        {
            /// <summary>
            /// Indicates to render an item.
            /// </summary>
            Item,
            /// <summary>
            /// Indicates to render a group.
            /// </summary>
            Group,
            /// <summary>
            /// Indicates to render a layer.
            /// </summary>
            Layer
        }

        public abstract class AbstractRenderOrderAction
        {
            internal RenderOrderActionType ActionType;
        }

        /// <summary>
        /// A RenderOrderAction indicating to render the item with the given name.
        /// </summary>
        public sealed class RenderItem : AbstractRenderOrderAction
        {

            /// <summary>
            /// The name of the item to render.
            /// </summary>
            public string Name;

            /// <summary>
            /// Whether or not to skip duplicate renders.
            /// </summary>
            public bool SkipDuplicates;

            public RenderType RenderType { get; private set; }

            public RenderItem(string name, bool skipDuplicates = true)
            {
                Name = name;
                RenderType = RenderOrder.RenderType.Item;
                SkipDuplicates = skipDuplicates;

                ActionType = RenderOrderActionType.RenderItem;
            }
        }

        public abstract class AbstractRenderTraversable : AbstractRenderOrderAction
        {
            /// <summary>
            /// The name of the item to render.
            /// </summary>
            public string Name;

            /// <summary>
            /// Options regarding which of the items in the group to render.
            /// </summary>
            public TraversalOptions Options;

            /// <summary>
            /// Whether or not to skip duplicate renders.
            /// </summary>
            public bool SkipDuplicates;

            public RenderType RenderType { get; protected set; }

            public AbstractRenderTraversable( string name
                                            , TraversalOptions options = TraversalOptions.Pre
                                            , bool skipDuplicates = true )
            {
                Name = name;
                Options = options;
                SkipDuplicates = skipDuplicates;
            }
        }

        /// <summary>
        /// A RenderOrderAction indicating to render the group with the given name.
        /// </summary>
        public sealed class RenderGroup : AbstractRenderTraversable
        {   
            public RenderGroup( string name
                              , TraversalOptions options = TraversalOptions.Pre
                              , bool skipDuplicates = true )
                : base(name, options, skipDuplicates)
            {
                RenderType = RenderOrder.RenderType.Group;
                ActionType = RenderOrderActionType.RenderGroup;
            }
        }

        /// <summary>
        /// A RenderOrderAction indicating to render the layer with the given name.
        /// </summary>
        public sealed class RenderLayer : AbstractRenderTraversable
        {
            public RenderLayer( string name
                              , TraversalOptions options = TraversalOptions.Pre
                              , bool skipDuplicates = true )
                : base(name, options, skipDuplicates)
            {
                RenderType = RenderOrder.RenderType.Layer;
                ActionType = RenderOrderActionType.RenderLayer;
            }
        }

        /// <summary>
        /// A RenderOrderAction indicating to set the render properties to the given values.
        /// </summary>
        public sealed class SetRenderProperties : AbstractRenderOrderAction
        {
            /// <summary>
            /// The properties to apply.
            /// </summary>
            public SpriteBatchPropertiesPacket Packet;

            /// <summary>
            /// If this is true, and a property in the packet is set to it's default value, then
            /// it is ignored and is not set. This is useful if you have a property that's locked 
            /// </summary>
            public bool IgnoreDefaults;
            public bool ApplyMatrix;
            public SetRenderProperties( SpriteBatchPropertiesPacket packet
                                      , bool ignoreDefaults = true
                                      , bool applyMatrix = false )
            {
                Packet = packet;
                IgnoreDefaults = ignoreDefaults;
                ApplyMatrix = applyMatrix;

                ActionType = RenderOrderActionType.SetRenderProperties;
            }
            public SetRenderProperties( SpriteSortMode ssm    = SpriteSortMode.Deferred
                                      , BlendState bs         = null
                                      , SamplerState ss       = null
                                      , DepthStencilState dss = null
                                      , RasterizerState rs    = null
                                      , Effect e              = null
                                      , Matrix? m             = null
                                      , bool ignoreDefaults   = true
                                      , bool applyMatrix      = false )
                : this(new SpriteBatchPropertiesPacket(ssm, bs, ss, dss, rs, e, m), ignoreDefaults, applyMatrix) { }
        }

        /// <summary>
        /// The list of actions.
        /// </summary>
        public List<AbstractRenderOrderAction> Actions { get; private set; }

        public RenderOrder() 
        {
            Actions = new List<AbstractRenderOrderAction>();
        }

        public RenderOrder(List<AbstractRenderOrderAction> actions)
        {
            Actions = actions;
        }

        /// <summary>
        /// Set the next render order action to render the item with the given name.
        /// </summary>
        /// <param name="name"></param>
        public void AddRenderItem(string name, bool skipDuplicates = true)
        {
            Actions.Add(new RenderItem(name, skipDuplicates));
        }

        /// <summary>
        /// Set the next render order action to render the group with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options"></param>
        public void AddRenderGroup(
            string name, TraversalOptions options = TraversalOptions.Pre, bool skipDuplicates = true)
        {
            Actions.Add(new RenderGroup(name, options, skipDuplicates));
        }

        /// <summary>
        /// Set the next render order action to set the render properties to the given values.
        /// </summary>
        /// <param name="ssm"></param>
        /// <param name="bs"></param>
        /// <param name="ss"></param>
        /// <param name="dss"></param>
        /// <param name="rs"></param>
        /// <param name="e"></param>
        /// <param name="m"></param>
        /// <param name="ignoreDefaults"></param>
        /// <param name="applyMatrix"></param>
        public void AddSetRenderProperties( SpriteSortMode ssm    = SpriteSortMode.Deferred
                                       , BlendState bs         = null
                                       , SamplerState ss       = null
                                       , DepthStencilState dss = null
                                       , RasterizerState rs    = null
                                       , Effect e              = null
                                       , Matrix? m             = null
                                       , bool ignoreDefaults   = true
                                       , bool applyMatrix      = false )
        {
            Actions.Add(new SetRenderProperties(ssm, bs, ss, dss, rs, e, m, ignoreDefaults, applyMatrix));
        }

        /// <summary>
        /// Set the next render order action to set the render properties to the values
        /// in the given packet.
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="ignoreDefaults"></param>
        /// <param name="applyMatrix"></param>
        public void AddSetRenderProperties( SpriteBatchPropertiesPacket packet
                                       , bool ignoreDefaults = true
                                       , bool applyMatrix = false )
        {
            Actions.Add(new SetRenderProperties(packet, ignoreDefaults, applyMatrix));
        }

        /// <summary>
        /// Set the next render order action to the given IRenderOrderAction.
        /// </summary>
        /// <param name="action"></param>
        public void SetNextAction(AbstractRenderOrderAction action)
        {
            Actions.Add(action);
        }
    }
}
