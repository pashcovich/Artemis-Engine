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
        private const string TOP_LEVEL = "ALL";
        private RenderPipeline rp;
        private World _world;
        private AbstractCamera _camera;
        private bool _requiresTargetTransformRecalc;
        private bool midRender;
        private GlobalLayerScaleType _layerScaleType;
        private UniformLayerScaleType _uniformScaleType;
        private GlobalLayerScaleType? _newLayerScaleType;
        private UniformLayerScaleType? _newUniformScaleType;
        private List<RenderableToAdd> toAdd = new List<RenderableToAdd>();

        internal Matrix _targetTransform;
        internal string tempFullName { get; private set; }

        private struct RenderableToAdd
        {
            public RenderableObject Object;
            public string Name;
            public bool Anonymous;
        }

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

        public RenderOrder RenderOrder { get; private set; }

        public List<RenderOrder.IRenderOrderAction> RenderOrderActions
        {
            get
            {
                if (RenderOrder == null)
                    return new List<RenderOrder.IRenderOrderAction>();
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
                toAdd.Add(new RenderableToAdd { Name = name, Object = item });
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
                toAdd.Add(new RenderableToAdd { Name = null, Object = item, Anonymous = true });
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
                toAdd.Add(new RenderableToAdd { Name = groupName, Object = item, Anonymous = true });
            else
                AllRenderables.AddAnonymousItem(groupName, item);
        }

        /// <summary>
        /// Add an anonymous form to this layer.
        /// </summary>
        /// <param name="form"></param>
        public void AddAnonymousForm(Form form)
        {
            AddAnonymousItem(form);
        }

        /// <summary>
        /// Add an anonymous form to the renderable group with the given name in this layer.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="form"></param>
        public void AddAnonymousForm(string groupName, Form form)
        {
            AddAnonymousItem(groupName, form);
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
                        renderOrder.RenderItem(name);
                    break;
                case RenderOrder.RenderType.Group:
                    foreach (var name in names)
                        renderOrder.RenderGroup(name);
                    break;
                default:
                    throw new Exception(); // throw an actual exception
            }
            RenderOrder = renderOrder;
        }

        public void SetRenderOrder(RenderOrder.RenderType[] types, string[] names)
        {
            if (types.Length != names.Length)
                throw new Exception(); // throw an actual exception

            var renderOrder = new RenderOrder();
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i] == RenderOrder.RenderType.Item)
                    renderOrder.RenderItem(names[i]);
                else if (types[i] == RenderOrder.RenderType.Group)
                    renderOrder.RenderGroup(names[i]);
            }
            RenderOrder = renderOrder;
        }

        public void SetRenderOrder(params RenderOrder.IRenderOrderAction[] actions)
        {
            RenderOrder = new RenderOrder(actions.ToList());
        }

        public void SetRenderOrder(RenderOrder order)
        {
            RenderOrder = order;
        }

        /// <summary>
        /// Attach this layer to a world.
        /// </summary>
        /// <param name="world"></param>
        public void AttachToWorld(World world)
        {
            _world = world;
        }

        private void RecalculateTargetTransform()
        {
            if (LayerScaleType == GlobalLayerScaleType.Uniform)
            {
                _targetTransform = Matrix.Identity;
                return;
            }

            if (ArtemisEngine.DisplayManager.IsBaseResolution)
            {
                _targetTransform = Matrix.Identity;
                return;
            }

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
                            "Invalid UniformLayerScaleType value '{0}' supplied.", UniformScaleType
                            )
                        );
            }

            _targetTransform = transform;
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

            rp.ClearRenderProperties();

            rp.SetRenderTarget(LayerTarget);
            rp.ClearGraphicsDevice(Color.Transparent);

            rp.SetRenderProperties(m: Camera.WorldToTargetTransform);
            rp.LockMatrix();

            var renderables = GetRenderables();

            if (LayerScaleType == GlobalLayerScaleType.Uniform)
            {
                foreach (var renderable in renderables)
                {
                    renderable.Render();
                }
            }
            else
            {
                var isBaseRes = ArtemisEngine.DisplayManager.IsBaseResolution;
                var crntRes   = ArtemisEngine.DisplayManager.WindowResolution;
                var resScale  = ArtemisEngine.DisplayManager.ResolutionScale;

                foreach (var renderable in renderables)
                {
                    ProcessDynamicallyScaledRenderable(renderable, isBaseRes, crntRes, resScale);
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

                _requiresTargetTransformRecalc = false;
            }

            // Add all the items that were added mid-render.
            //
            // Honestly, this is kind of an unnecessary buffer for user clumsiness. They're
            // rendering code should NOT be adding anything to a layer, that SHOULD be happening
            // in their update code. This is just in case some theirs some edge case where you
            // are forced to add an item mid-render, or in case the user isn't astute enough to
            // realize the error in their ways.

            foreach (var item in toAdd)
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

            toAdd.Clear();
        }

        /// <summary>
        /// Get the list of items to draw (dependent on the camera).
        /// </summary>
        /// <returns></returns>
        private List<RenderableObject> GetRenderables()
        {
            if (Camera is NullCamera)
                return AllRenderables.RetrieveAll();

            var renderables = new HashSet<RenderableObject>();
            var aabb = Camera.ViewAABB;
            _world.QueryAABB(f =>
                {
                    var obj = (RenderableObject)f.Body.UserData;
                    renderables.Add(obj);
                    return true;
                }, ref aabb);

            return renderables.ToList();
        }

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

            bool hasOriginalScale = false;
            Vector2 originalScale = Vector2.One;
            if (obj.RenderComponents.Scale.HasValue)
            {
                originalScale = obj.RenderComponents.Scale.Value;
                hasOriginalScale = true;
            }
                
            var resultingScale = VectorUtils.ComponentwiseProduct(originalScale, scale);

            // Swap out the original scale for the newly calculated scale.
            obj.RenderComponents.Scale = resultingScale;

            obj.Render();

            obj.RenderComponents.Scale = hasOriginalScale ? (Vector2?)originalScale : null;
        }
    }
}
