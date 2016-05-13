#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;

#endregion

namespace Artemis.Engine.Graphics
{
    public class RenderOrder
    {
        /// <summary>
        /// Determines what a `Render` action represents (an item or a group).
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
            Group
        }

        internal enum RenderOrderActionType
        {
            RenderItem,
            RenderGroup,
            SetRenderProperties
        }

        /// <summary>
        /// Options indicating how to render a group.
        /// </summary>
        public enum RenderGroupOptions
        {
            /// <summary>
            /// Render the subgroups before rendering the items in the top level.
            /// </summary>
            AllPre,
            /// <summary>
            /// Render the subgroups after rendering the items in the top level.
            /// </summary>
            AllPost,
            /// <summary>
            /// Only render the items in the top level.
            /// </summary>
            Top
        }

        public abstract class AbstractRenderOrderAction
        {
            internal RenderOrderActionType ActionType;
        }

        /// <summary>
        /// A RenderOrderAction indicating to render the item with the given name.
        /// </summary>
        public class RenderItem : AbstractRenderOrderAction
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

        /// <summary>
        /// A RenderOrderAction indicating to render the group with the given name.
        /// </summary>
        public class RenderGroup : AbstractRenderOrderAction
        {
            /// <summary>
            /// The name of the item to render.
            /// </summary>
            public string Name;

            /// <summary>
            /// Options regarding which of the items in the group to render.
            /// </summary>
            public RenderGroupOptions Options;

            /// <summary>
            /// Whether or not to skip duplicate renders.
            /// </summary>
            public bool SkipDuplicates;
            
            public RenderType RenderType { get; private set; }
            
            public RenderGroup(
                string name, RenderGroupOptions options = RenderGroupOptions.AllPre, bool skipDuplicates = true)
            {
                Name = name;
                Options = options;
                RenderType = RenderOrder.RenderType.Group;
                SkipDuplicates = skipDuplicates;

                ActionType = RenderOrderActionType.RenderGroup;
            }
        }

        /// <summary>
        /// A RenderOrderAction indicating to set the render properties to the given values.
        /// </summary>
        public class SetRenderProperties : AbstractRenderOrderAction
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
            string name, RenderGroupOptions options = RenderGroupOptions.AllPre, bool skipDuplicates = true)
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
