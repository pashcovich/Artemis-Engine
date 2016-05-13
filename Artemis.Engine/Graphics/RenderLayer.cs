#region Using Statements

using Artemis.Engine.Maths.Geometry;
using Artemis.Engine.Utilities.UriTree;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Artemis.Engine.Graphics
{
    public class RenderLayer : UriTreeObserverNode<RenderLayer, RenderableGroup>
    {
        private const string TOP_LEVEL = "ALL"; // The name of the top level of renderable objects.
        private RenderPipeline rp; // the global render pipeline,
        private World _world;
        private AbstractCamera _camera;
        private bool _requiresTargetTransformRecalc; // whether or not we need to recalculate the TargetTransform 
                                                     // matrix (when target resolution changes for example).
        private bool midRender; // whether or not we've entered a render cycle.

        private GlobalLayerScaleType _layerScaleType;
        private UniformLayerScaleType _uniformScaleType;
        private GlobalLayerScaleType? _newLayerScaleType;
        private UniformLayerScaleType? _newUniformScaleType;

        private readonly List<RenderableToAdd> renderablesToAdd = new List<RenderableToAdd>();

        // the list of the renderables we've seen so far in this render cycle.
        private readonly HashSet<RenderableObject> seenRenderables = new HashSet<RenderableObject>();

        // the list of renderable groups we've seen so far in this render cycle.
        private readonly HashSet<RenderableGroup> seenGroups = new HashSet<RenderableGroup>(); 

        internal Matrix _targetTransform;
        internal string tempFullName { get; private set; }

        private struct RenderableToAdd
        {
            public RenderableObject Object;
            public string Name;
            public bool Anonymous;
        }

        /// <summary>
        /// A delegate defining what to do upon encountering a renderable
        /// object in a renderable group.
        /// </summary>
        /// <param name="?"></param>
        public delegate void RenderAction(RenderableObject obj);

        /// <summary>
        /// The target we're rendering to.
        /// </summary>
        protected RenderTarget2D LayerTarget { get; private set; }

        /// <summary>
        /// The top level renderable group.
        /// </summary>
        protected RenderableGroup AllRenderables { get; private set; }

        /// <summary>
        /// Determines whether or not the layer is scaled "Uniformly" (the entire layer is
        /// scaled up at once) or "Dynamically" (individual objects are scaled according to
        /// their individual rules).
        /// </summary>
        public GlobalLayerScaleType LayerScaleType
        {
            get { return _layerScaleType; }
            set
            {
                if (value == _layerScaleType)
                    return;
                _layerScaleType = value;

                if (midRender)
                {
                    _newLayerScaleType = value;
                    _requiresTargetTransformRecalc = true;
                }
                else
                {
                    _layerScaleType = value;
                    RecalculateTargetTransform();
                }
            }
        }

        /// <summary>
        /// If the LayerScaleType is "Uniform", then this determines how the layer is scaled.
        /// </summary>
        public UniformLayerScaleType UniformScaleType
        {
            get { return _uniformScaleType; }
            set
            {
                if (value == _uniformScaleType)
                    return;               
                if (midRender)
                {
                    _newUniformScaleType = value;
                    _requiresTargetTransformRecalc = true;
                }
                else
                {
                    _uniformScaleType = value;
                    RecalculateTargetTransform();
                }
            }
        }

        /// <summary>
        /// The global RenderGroupOptions, determining whether or not the top level items should be 
        /// rendered first or the subnodes first. The default value is "RenderOrder.RenderGroupOptions.AllPre".
        /// 
        /// Note: If RenderOrder is not null, this value is not used.
        /// </summary>
        public RenderOrder.RenderGroupOptions GlobalRenderGroupOptions;

        /// <summary>
        /// The camera attached to this layer.
        /// </summary>
        public AbstractCamera Camera
        {
            get { return _camera; }
            set
            {
                if (midRender)
                    throw new CameraException(
                        String.Format(
                            "Cannot set Camera on render layer with name '{0}' " +
                            "until after the render cycle is complete.", tempFullName
                            )
                        );
                if (value == null)
                    value = new NullCamera();
                _camera = value;
                _camera.Layer = this;
            }
        }

        /// <summary>
        /// The world this layer is attached to.
        /// </summary>
        public World World
        {
            get { return _world; }
        }

        /// <summary>
        /// Whether or not we are in the middle of rendering this layer.
        /// </summary>
        public bool MidRender { get { return midRender; } }

        /// <summary>
        /// Whether or not this render layer is managed by a LayerManager or not.
        /// </summary>
        public bool Managed { get; internal set; }

        /// <summary>
        /// The order in which objects get rendered.
        /// </summary>
        public RenderOrder RenderOrder { get; private set; }

        /// <summary>
        /// THe list of actions representing the render order.
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

        public RenderLayer(string fullName)
            : this(fullName, new NullCamera()) { }

        public RenderLayer(string fullName, AbstractCamera camera)
            : this(fullName, camera, GlobalLayerScaleType.Dynamic, UniformLayerScaleType.Stretch, null) { }

        public RenderLayer(string fullName, AbstractCamera camera, World world)
            : this(fullName, camera, GlobalLayerScaleType.Dynamic, UniformLayerScaleType.Stretch, world) { }

        public RenderLayer( string fullName
                          , AbstractCamera camera
                          , GlobalLayerScaleType layerScaleType    = GlobalLayerScaleType.Dynamic
                          , UniformLayerScaleType uniformScaleType = UniformLayerScaleType.Stretch
                          , World world = null )
            : base(UriUtilities.GetLastPart(fullName))
        {
            rp = ArtemisEngine.RenderPipeline; // for convenience

            // We have to store the full name until the layer gets added to 
            // a LayerManager.
            tempFullName = fullName;
            Managed = false;

            AllRenderables = new RenderableGroup(TOP_LEVEL);
            AddObservedNode(TOP_LEVEL, AllRenderables);

            LayerTarget = ArtemisEngine.RenderPipeline.CreateRenderTarget();

            GlobalRenderGroupOptions = RenderOrder.RenderGroupOptions.AllPre;
            LayerScaleType = layerScaleType;
            UniformScaleType = uniformScaleType;
            RecalculateTargetTransform();

            Camera = camera;
            camera.Layer = this;

            _world = world;
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
            if (midRender)
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
            if (midRender)
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
            if (midRender)
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
            if (midRender)
                throw new RenderOrderException(
                    String.Format(
                        "Cannot set render order on layer '{0}' during the render cycle.", FullName));
            RenderOrder = order;

            // Calculate render order.
        }

        /// <summary>
        /// Attach this layer to a world.
        /// </summary>
        /// <param name="world"></param>
        public void AttachToWorld(World world)
        {
            _world = world;
        }

        /// <summary>
        /// Recalculate the TargetTransform matrix (required when resolution changes).
        /// </summary>
        private void RecalculateTargetTransform()
        {
            if (LayerScaleType == GlobalLayerScaleType.Uniform)
            {
                _targetTransform = Matrix.Identity;
            }
            else if (ArtemisEngine.DisplayManager.IsBaseResolution)
            {
                _targetTransform = Matrix.Identity;
            }
            else
            {
                var baseRes = GameConstants.BaseResolution;
                var scale = ArtemisEngine.DisplayManager.ResolutionScale;

                Matrix transform;
                switch (UniformScaleType)
                {
                    case UniformLayerScaleType.Stretch:
                        transform = Matrix.CreateScale(scale.X, scale.Y, 1);
                        break;
                    case UniformLayerScaleType.Fit:
                        if (scale.X == scale.Y)
                            transform = Matrix.CreateScale(scale.X, scale.X, 1);
                        else if (scale.X > scale.Y)
                            transform = Matrix.CreateScale(scale.Y, scale.Y, 1) *
                                        Matrix.CreateTranslation(baseRes.Width * (1 - scale.X) / 2f, 0, 0);
                        transform = Matrix.CreateScale(scale.X, scale.X, 1) *
                                    Matrix.CreateTranslation(0, baseRes.Height * (1 - scale.Y) / 2f, 0);
                        break;
                    case UniformLayerScaleType.Fill:
                        if (scale.X == scale.Y)
                            transform = Matrix.CreateScale(scale.X, scale.X, 1);
                        else if (scale.X > scale.Y)
                            transform = Matrix.CreateScale(scale.X, scale.X, 1) *
                                        Matrix.CreateTranslation(0, baseRes.Height / 2f * (scale.Y - scale.X), 0);
                        transform = Matrix.CreateScale(scale.Y, scale.Y, 1) *
                                    Matrix.CreateTranslation(baseRes.Width / 2f * (scale.X - scale.Y), 0, 0);
                        break;
                    default:
                        throw new RenderLayerException(
                            String.Format(
                                "Invalid UniformLayerScaleType value '{0}' supplied.", UniformScaleType));
                }
                _targetTransform = transform;
            }
            _requiresTargetTransformRecalc = false;
        }

        /// <summary>
        /// Process a RenderableObject when the GlobalLayerScaleType is set to Dynamic.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isBaseRes"></param>
        /// <param name="crntRes"></param>
        /// <param name="resScale"></param>
        private void ProcessDynamicallyScaledRenderable(
            RenderableObject obj, bool isBaseRes, Resolution crntRes, Vector2 resScale)
        {
            var maintainAspectRatio = obj.MaintainAspectRatio;
            var scaleType = obj.ScaleType;

            Vector2 scale;
            if (!isBaseRes)
            {
                switch (scaleType)
                {
                    case ResolutionScaleType.Min:
                        var min = MathHelper.Min(resScale.X, resScale.Y);
                        if (maintainAspectRatio)
                            scale = new Vector2(min, min);
                        else if (min == resScale.X)
                            scale = new Vector2(min, 1f);
                        else
                            scale = new Vector2(1f, min);
                        break;
                    case ResolutionScaleType.Max:
                        var max = MathHelper.Max(resScale.X, resScale.Y);
                        if (maintainAspectRatio)
                            scale = new Vector2(max, max);
                        else if (max == resScale.X)
                            scale = new Vector2(max, 1f);
                        else
                            scale = new Vector2(1f, max);
                        break;
                    case ResolutionScaleType.Width:
                        if (maintainAspectRatio)
                            scale = new Vector2(resScale.X, resScale.X);
                        else
                            scale = new Vector2(resScale.X, 1f);
                        break;
                    case ResolutionScaleType.Height:
                        if (maintainAspectRatio)
                            scale = new Vector2(resScale.Y, resScale.Y);
                        else
                            scale = new Vector2(1f, resScale.Y);
                        break;
                    case ResolutionScaleType.WithRes:
                        /*
                        if (maintainAspectRatio && resScale.X != resScale.Y)
                            // throw an error or log a warning or something...
                         */
                        scale = new Vector2(resScale.X, resScale.Y);
                        break;
                    default:
                        throw new RenderLayerException(
                            String.Format(
                                "Unknown ResolutionScaleType '{0}' received on object '{1}'.", scaleType, obj
                                )
                            );
                }
            }
            else
            {
                scale = Vector2.One;
            }

            bool hasOriginalScale = obj.RenderComponents.Scale.HasValue;
            Vector2 originalScale = hasOriginalScale ? obj.RenderComponents.Scale.Value : Vector2.One;

            var resultingScale = VectorUtils.ComponentwiseProduct(originalScale, scale);

            // Swap out the original scale for the newly calculated scale.
            obj.RenderComponents.Scale = resultingScale;

            obj.InternalRender(seenRenderables);

            obj.RenderComponents.Scale = hasOriginalScale ? (Vector2?)originalScale : null;
        }

        /// <summary>
        /// Get the RenderAction. The Layer RenderAction determines how it prepares and calls
        /// each RenderableObject's Render method it encounters.
        /// 
        /// For RenderLayer, the render action returned depends on the LayerScaleType.
        /// </summary>
        /// <returns></returns>
        private RenderAction GetRenderAction(Predicate<RenderableObject> isVisibileToCamera)
        {
            switch (LayerScaleType)
            {
                case GlobalLayerScaleType.Uniform:
                    if (isVisibileToCamera == null)
                        return obj => obj.InternalRender(seenRenderables);
                    else
                        return obj =>
                        {
                            if (isVisibileToCamera(obj))
                                obj.InternalRender(seenRenderables);
                        };
                case GlobalLayerScaleType.Dynamic:
                    var isBaseRes = ArtemisEngine.DisplayManager.IsBaseResolution;
                    var crntRes   = ArtemisEngine.DisplayManager.WindowResolution;
                    var resScale  = ArtemisEngine.DisplayManager.ResolutionScale;

                    if (isVisibileToCamera == null)
                        return obj => ProcessDynamicallyScaledRenderable(obj, isBaseRes, crntRes, resScale);
                    else
                        return obj =>
                        {
                            if (isVisibileToCamera(obj))
                                ProcessDynamicallyScaledRenderable(obj, isBaseRes, crntRes, resScale);
                        };
                default:
                    throw new RenderLayerException(
                        String.Format(
                            "Unknown GlobalLayerScaleType '{0}' supplied to layer '{1}'.",
                            LayerScaleType, tempFullName));
            }
        }

        /// <summary>
        /// Get the list of all RenderableObjects visible to the Camera.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<RenderableObject> GetCameraVisibleRenderables()
        {
            if (Camera is NullCamera)
                return AllRenderables.RetrieveAll();

            var renderables = new HashSet<RenderableObject>();
            var aabb = Camera.ViewAABB;
            _world.QueryAABB(f =>
            {
                var obj = (RenderableObject)f.Body.UserData;
                renderables.Add(obj); // A renderable object might have multiple fixtures, but
                // we only want to render the object itself once.
                return true;
            }, ref aabb);

            return renderables;
        }

        /// <summary>
        /// Render all items and sublayers on this layer.
        /// </summary>
        public void RenderAll()
        {
            foreach (var layer in Subnodes.Values)
            {
                layer.RenderAll();
            }

            Render();
        }

        /// <summary>
        /// Render all items in this layer.
        /// </summary>
        public void Render()
        {
            midRender = true;

            // Reset the RenderTarget if the resolution has changed.
            if (ArtemisEngine.DisplayManager.ResolutionChanged)
            {
                LayerTarget.Dispose();

                // If we're scaling dynamically then our layer target fills the entire screen.
                if (LayerScaleType == GlobalLayerScaleType.Dynamic)
                    LayerTarget = ArtemisEngine.RenderPipeline.CreateRenderTarget();
                else
                    LayerTarget = ArtemisEngine.RenderPipeline.CreateBaseResRenderTarget();
                RecalculateTargetTransform();
            }

            // Set the RenderPipeline's target to be this layer's target, clear it,
            // and lock the matrix property so any user matrix transformations are applied
            // to the Camera's matrix.
            rp.ClearRenderProperties();

            rp.SetRenderTarget(LayerTarget);
            rp.ClearGraphicsDevice(Color.Transparent);

            rp.SetRenderProperties(m: Camera.WorldToTargetTransform);
            rp.LockMatrix();

            // Get all the renderable objects.
            var renderables = GetCameraVisibleRenderables();

            // Create the predicate that checks if a given renderable is visible.
            Predicate<RenderableObject> isVisibleToCamera = null; // if null, then every renderable is visible.
            if (!(Camera is NullCamera))
            {
                var hashSet = (HashSet<RenderableObject>)renderables;
                isVisibleToCamera = obj => hashSet.Contains(obj);
            }

            // Get the render action for each encountered RenderableObject.
            RenderAction renderAction = GetRenderAction(isVisibleToCamera);

            seenRenderables.Clear();
            seenGroups.Clear();

            // If no RenderOrder was set, just render everything.
            if (RenderOrder == null)
            {
                AllRenderables.Render(
                    GlobalRenderGroupOptions, renderAction, seenRenderables, seenGroups);
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
                            HandleRenderOrderAction_RenderItem((RenderOrder.RenderItem)item, renderAction);
                            break;
                        case RenderOrder.RenderOrderActionType.RenderGroup:
                            HandleRenderOrderAction_RenderGroup((RenderOrder.RenderGroup)item, renderAction);
                            break;
                        case RenderOrder.RenderOrderActionType.SetRenderProperties:
                            HandleRenderOrderAction_SetRenderProperties((RenderOrder.SetRenderProperties)item);
                            break;
                        default:
                            HandleUnknownRenderOrderAction(item, renderAction, isVisibleToCamera);
                            break;
                    }
                }
            }

            rp.UnlockMatrix();
            rp.UnsetRenderTarget();
            rp.ClearRenderProperties();

            rp.SetRenderProperties(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                m: _targetTransform);

            rp.Render(LayerTarget, Vector2.Zero);

            rp.ClearRenderProperties();

            midRender = false;

            if (_requiresTargetTransformRecalc)
            {
                if (_newLayerScaleType.HasValue)
                    _layerScaleType = _newLayerScaleType.Value;
                if (_newUniformScaleType.HasValue)
                    _uniformScaleType = _newUniformScaleType.Value;

                _newLayerScaleType = null;
                _newUniformScaleType = null;

                RecalculateTargetTransform();
            }

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
        /// Handle a RenderItem action in the RenderOrder.
        /// </summary>
        /// <param name="action"></param>
        private void HandleRenderOrderAction_RenderItem(
            RenderOrder.RenderItem action, RenderAction renderAction)
        {
            var renderable = AllRenderables.GetItem(action.Name);
            if (!(action.SkipDuplicates && seenRenderables.Contains(renderable)))
                renderAction(renderable);
        }
        
        /// <summary>
        /// Handle a RenderGroup action in the RenderOrder.
        /// </summary>
        /// <param name="action"></param>
        private void HandleRenderOrderAction_RenderGroup(
            RenderOrder.RenderGroup action, RenderAction renderAction)
        {
            var group = AllRenderables.GetSubnode(action.Name);
            group.Render(action.Options, renderAction, seenRenderables, seenGroups, action.SkipDuplicates);
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
            RenderOrder.AbstractRenderOrderAction action, RenderAction renderAction, Predicate<RenderableObject> isVisibleToCamera)
        {
            throw new RenderOrderException(
                String.Format("Unknown render order action '{0}'.", action));
        }
    }
}
