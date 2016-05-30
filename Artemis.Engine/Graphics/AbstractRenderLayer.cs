#region Using Statements

using Artemis.Engine.Utilities;
using Artemis.Engine.Utilities.UriTree;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

#endregion

namespace Artemis.Engine.Graphics
{
    public abstract class AbstractRenderLayer : UriTreeObserverNode<AbstractRenderLayer, RenderableGroup>
    {
        private const string TOP_LEVEL = "ALL"; // The name of the top level of renderable objects.

        internal HashSet<RenderableObject> targetChangeListeners = new HashSet<RenderableObject>();

        private readonly List<RenderableToAdd> renderablesToAdd = new List<RenderableToAdd>();
        protected struct RenderableToAdd
        {
            public RenderableObject Object;
            public string Name;
            public bool Anonymous;
        }

        internal string tempFullName;

        /// <summary>
        /// Whether or not we need to recalculate the TargetToScreenTransform matrix (when the target resolution
        /// changes for example).
        /// </summary>
        protected internal bool RequiresTargetTransformRecalc;

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
        /// The target we're rendering to.
        /// </summary>
        public RenderTarget2D LayerTarget { get; protected set; }

        /// <summary>
        /// The fill colour of the target. The default value is Color.Transparent.
        /// 
        /// NOTE: If the colour you supply has no alpha value, it will be opaque, meaning
        /// when this layer is rendered it will erase any layers underneath.
        /// </summary>
        public Color TargetFill;

        /// <summary>
        /// The surface format for the LayerTarget.
        /// </summary>
        public SurfaceFormat TargetFormat;

        /// <summary>
        /// The depth format for the LayerTarget.
        /// </summary>
        public DepthFormat TargetDepthFormat;

        /// <summary>
        /// The preferred multisample count for the LayerTarget.
        /// </summary>
        public int PreferredMultiSampleCount;

        /// <summary>
        /// The usage of the LayerTarget.
        /// </summary>
        public RenderTargetUsage TargetUsage;

        /// <summary>
        /// Whether or not the LayerTarget is a mipmap.
        /// </summary>
        public bool TargetIsMipMap;

        /// <summary>
        /// The boundaries of the target.
        /// </summary>
        public Rectangle TargetBounds { get { return LayerTarget.Bounds; } }

        public Matrix TargetToScreenTransform { get; protected internal set; }

        public Matrix ScreenToTargetTransform { get { return Matrix.Invert(TargetToScreenTransform); } }

        public AbstractRenderLayer(string fullName)
            : base(UriUtilities.GetLastPart(fullName))
        {
            tempFullName = fullName;
            Managed = false;

            AllRenderables = new RenderableGroup(TOP_LEVEL);
            AddObservedNode(TOP_LEVEL, AllRenderables);

            LayerTarget = ArtemisEngine.RenderPipeline.CreateRenderTarget();
            
            // LayerTarget properties
            TargetFill                = Color.Transparent;
            TargetFormat              = SurfaceFormat.Color;
            TargetDepthFormat         = DepthFormat.None;
            PreferredMultiSampleCount = 0;
            TargetUsage               = RenderTargetUsage.DiscardContents;
            TargetIsMipMap            = false;
        }

        public Vector2 TargetToScreen(Vector2 vector)
        {
            return Vector2.Transform(vector, TargetToScreenTransform);
        }

        public Vector2 ScreenToTarget(Vector2 vector)
        {
            return Vector2.Transform(vector, ScreenToTargetTransform);
        }

        protected internal virtual void RecalculateTargetTransform()
        {
            // In an AbstractRenderLayer, no resolution relative target scaling takes place, thus
            // the target space *is* the screen space.
            TargetToScreenTransform = Matrix.Identity;
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
            item.Layer = this;
            targetChangeListeners.Add(item);
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
            item.Layer = this;
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
            item.Layer = this;
            targetChangeListeners.Add(item);
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
        /// Render all items and sublayers on this layer.
        /// </summary>
        public void Render( HashSet<AbstractRenderLayer> seenLayers
                          , TraversalOptions order = TraversalOptions.Pre
                          , bool skipDuplicates = true )
        {
            MidRender = true;
            switch (order)
            {
                case TraversalOptions.Pre:
                    RenderSublayers(seenLayers, order, skipDuplicates);
                    if (!(skipDuplicates && seenLayers.Contains(this)))
                        RenderTop();
                    break;
                case TraversalOptions.Post:
                    if (!(skipDuplicates && seenLayers.Contains(this)))
                        RenderTop();
                    RenderSublayers(seenLayers, order, skipDuplicates);
                    break;
                case TraversalOptions.Top:
                    if (!(skipDuplicates && seenLayers.Contains(this)))
                        RenderTop();
                    break;
                default:
                    throw new RenderOrderException(
                        String.Format(
                            "Unknown RenderOrder.RenderGroupOptions '{0}' supplied when rendering " +
                            "layer '{1}'.", order, FullName));
            }
            MidRender = false;

            seenLayers.Add(this);

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
        /// Render only the sublayers of this layer.
        /// </summary>
        /// <param name="order"></param>
        public virtual void RenderSublayers( HashSet<AbstractRenderLayer> seenLayers
                                           , TraversalOptions order = TraversalOptions.Pre  
                                           , bool skipDuplicates = true )
        {
            foreach (var layer in Subnodes.Values)
            {
                layer.Render(seenLayers, order, skipDuplicates);
            }
        }

        // (Michael, 5/15/2016) NOTE: RenderTop is private because if the user wants to only 
        // render the items in this layer and none of the sublayers, they can just call Render 
        // with their `order` parameter set to `RenderOrder.RenderTraversalOptions.AllPre`.

        /// <summary>
        /// Render only the items in this layer (none of the sublayers).
        /// </summary>
        private void RenderTop()
        {
            // Setup anything before rendering.
            PreRender();

            // Setup the LayerTarget.
            SetupLayerTarget();

            // Any final setup after the LayerTarget is setup and before the items are rendered.
            PostSetupLayerTarget();

            // Render the items.
            RenderItems();

            // Render the layer target to the screen.
            RenderLayerTarget();

            // Finalization after rendering.
            PostRender();
        }

        /// <summary>
        /// Called before anything is rendered.
        /// </summary>
        protected virtual void PreRender() 
        {
            // Reset the RenderTarget if the resolution has changed.
            if (ArtemisEngine.DisplayManager.ResolutionChanged)
            {
                var previousTarget = LayerTarget;
                
                LayerTarget = ArtemisEngine.RenderPipeline.CreateRenderTarget(
                    TargetFormat, TargetDepthFormat, PreferredMultiSampleCount, TargetUsage, TargetFill, TargetIsMipMap);

                foreach (var item in targetChangeListeners)
                {
                    if (item.OnLayerTargetChanged != null)
                    {
                        item.OnLayerTargetChanged(previousTarget, LayerTarget);
                    }
                }

                previousTarget.Dispose();
            }
        }

        /// <summary>
        /// Set up the layer's target.
        /// </summary>
        protected virtual void SetupLayerTarget()
        {
            ArtemisEngine.RenderPipeline.ClearRenderProperties();

            ArtemisEngine.RenderPipeline.SetRenderTarget(LayerTarget);
            ArtemisEngine.RenderPipeline.ClearGraphicsDevice(TargetFill);
        }

        /// <summary>
        /// Called directly after "SetupLayerTarget" and directly before any rendering.
        /// </summary>
        protected virtual void PostSetupLayerTarget() { }

        /// <summary>
        /// Actually render the RenderableObjects.
        /// </summary>
        protected abstract void RenderItems();

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
    }
}
