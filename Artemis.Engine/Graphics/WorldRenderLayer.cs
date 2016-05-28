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
    public class WorldRenderLayer : AbstractOrderableRenderLayer
    {
        private RenderPipeline rp; // the global render pipeline,
        private World _world;
        private AbstractCamera _camera;
        private bool _requiresTargetTransformRecalc; // whether or not we need to recalculate the TargetTransform 
                                                     // matrix (when target resolution changes for example).
        internal Matrix _targetTransform;
        private Predicate<RenderableObject> isVisibleToCameraPredicate;

        private GlobalLayerScaleType _layerScaleType;
        private UniformLayerScaleType _uniformScaleType;
        private GlobalLayerScaleType? _newLayerScaleType;
        private UniformLayerScaleType? _newUniformScaleType;

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

                if (MidRender)
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
        /// 
        /// Note: This value is only used if LayerScaleType is set to GlobalLayerScaleType.Uniform.
        /// </summary>
        public UniformLayerScaleType UniformScaleType
        {
            get { return _uniformScaleType; }
            set
            {
                if (value == _uniformScaleType)
                    return;               
                if (MidRender)
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
                if (MidRender)
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

        public WorldRenderLayer(string fullName)
            : this(fullName, new NullCamera()) { }

        public WorldRenderLayer(string fullName, AbstractCamera camera)
            : this(fullName, camera, GlobalLayerScaleType.Dynamic, UniformLayerScaleType.Stretch, null) { }

        public WorldRenderLayer(string fullName, AbstractCamera camera, World world)
            : this(fullName, camera, GlobalLayerScaleType.Dynamic, UniformLayerScaleType.Stretch, world) { }

        public WorldRenderLayer( string fullName
                          , AbstractCamera camera
                          , GlobalLayerScaleType layerScaleType    = GlobalLayerScaleType.Dynamic
                          , UniformLayerScaleType uniformScaleType = UniformLayerScaleType.Stretch
                          , World world = null )
            : base(UriUtilities.GetLastPart(fullName))
        {
            rp = ArtemisEngine.RenderPipeline; // for convenience

            LayerScaleType = layerScaleType;
            UniformScaleType = uniformScaleType;
            RecalculateTargetTransform();

            Camera = camera;
            camera.Layer = this;

            _world = world;
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

            bool hasOriginalScale = obj.SpriteProperties.Scale.HasValue;
            Vector2 originalScale = hasOriginalScale ? obj.SpriteProperties.Scale.Value : Vector2.One;

            var resultingScale = VectorUtils.ComponentwiseProduct(originalScale, scale);

            // Swap out the original scale for the newly calculated scale.
            obj.SpriteProperties.Scale = resultingScale;

            obj.InternalRender(SeenRenderables);

            obj.SpriteProperties.Scale = hasOriginalScale ? (Vector2?)originalScale : null;
        }

        /// <summary>
        /// Get the RenderAction. The Layer RenderAction determines how it prepares and calls
        /// each RenderableObject's Render method it encounters.
        /// 
        /// For RenderLayer, the render action returned depends on the LayerScaleType.
        /// </summary>
        /// <returns></returns>
        protected override RenderableHandler GetRenderableHandler()
        {
            switch (LayerScaleType)
            {
                case GlobalLayerScaleType.Uniform:
                    if (isVisibleToCameraPredicate == null)
                        return obj => obj.InternalRender(SeenRenderables);
                    else
                        return obj =>
                        {
                            if (isVisibleToCameraPredicate(obj))
                                obj.InternalRender(SeenRenderables);
                        };
                case GlobalLayerScaleType.Dynamic:
                    var isBaseRes = ArtemisEngine.DisplayManager.IsBaseResolution;
                    var crntRes   = ArtemisEngine.DisplayManager.WindowResolution;
                    var resScale  = ArtemisEngine.DisplayManager.ResolutionScale;

                    if (isVisibleToCameraPredicate == null)
                        return obj => ProcessDynamicallyScaledRenderable(obj, isBaseRes, crntRes, resScale);
                    else
                        return obj =>
                        {
                            if (isVisibleToCameraPredicate(obj))
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

        protected override void PreRender()
        {
            // Reset the RenderTarget if the resolution has changed.
            if (ArtemisEngine.DisplayManager.ResolutionChanged)
            {
                var previousTarget = LayerTarget;

                // If we're scaling dynamically then our layer target fills the entire screen.
                if (LayerScaleType == GlobalLayerScaleType.Dynamic)
                    LayerTarget = ArtemisEngine.RenderPipeline.CreateRenderTarget(
                        TargetFormat, 
                        TargetDepthFormat, 
                        PreferredMultiSampleCount, 
                        TargetUsage, 
                        TargetFill, 
                        TargetIsMipMap);
                else
                    LayerTarget = ArtemisEngine.RenderPipeline.CreateRenderTarget(
                        TargetFormat, 
                        TargetDepthFormat, 
                        PreferredMultiSampleCount, 
                        TargetUsage, 
                        TargetFill, 
                        TargetIsMipMap);

                foreach (var item in targetChangeListeners)
                {
                    if (item.OnLayerTargetChanged != null)
                    {
                        item.OnLayerTargetChanged(previousTarget, LayerTarget);
                    }
                }

                previousTarget.Dispose();

                RecalculateTargetTransform();
            }

            // Get all the renderable objects.
            var renderables = GetCameraVisibleRenderables();

            // Create the predicate that checks if a given renderable is visible.
            isVisibleToCameraPredicate = null; // if null, then every renderable is visible.
            if (!(Camera is NullCamera))
            {
                var hashSet = (HashSet<RenderableObject>)renderables;
                isVisibleToCameraPredicate = obj => hashSet.Contains(obj);
            }
        }

        /// <summary>
        /// Called directly after "SetupLayerTarget" and directly before any rendering.
        /// </summary>
        protected override void PostSetupLayerTarget()
        {
            rp.SetRenderProperties(m: Camera.WorldToTargetTransform);
            rp.LockMatrix();
        }

        /// <summary>
        /// Render this layer's target. This is called immediately after all the Renderables
        /// have been rendered, and immediately before PostRender.
        /// </summary>
        protected override void RenderLayerTarget()
        {
            rp.UnlockMatrix();
            rp.UnsetRenderTarget();
            rp.ClearRenderProperties();

            rp.SetRenderProperties(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                m: _targetTransform);

            rp.Render(LayerTarget, Vector2.Zero);

            rp.ClearRenderProperties();
        }

        /// <summary>
        /// Called after everything is rendered (also after RenderLayerTarget is called).
        /// </summary>
        protected override void PostRender()
        {
            if (_requiresTargetTransformRecalc)
            {
                if (_newLayerScaleType.HasValue)
                    _layerScaleType = _newLayerScaleType.Value;
                if (_newUniformScaleType.HasValue)
                    _uniformScaleType = _newUniformScaleType.Value;

                _newLayerScaleType = null;
                _newUniformScaleType = null;

                RecalculateTargetTransform(); // sets _requiresTargetTransformRecalc to false.
            }
        }
    }
}
