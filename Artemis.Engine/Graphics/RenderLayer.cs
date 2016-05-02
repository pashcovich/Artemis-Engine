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
        private Matrix _targetTransform;
        private World _world;

        internal string tempName { get; private set; }

        protected RenderTarget2D LayerTarget { get; private set; }

        protected RenderableGroup AllRenderables { get; private set; }

        public GlobalLayerScaleType LayerScaleType;

        public UniformLayerScaleType UniformScaleType;

        public AbstractCamera Camera;

        /// <summary>
        /// Whether or not this render layer is managed by a LayerManager or not.
        /// </summary>
        public bool Managed { get; internal set; }

        public RenderLayer(string fullName)
            : this(fullName, new NullCamera()) { }

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
            tempName = fullName;
            Managed = false;

            AllRenderables = new RenderableGroup(TOP_LEVEL);
            AddObservedNode(TOP_LEVEL, AllRenderables);

            LayerTarget = ArtemisEngine.RenderPipeline.CreateRenderTarget();

            LayerScaleType = layerScaleType;
            UniformScaleType = uniformScaleType;
            _targetTransform = RecalculateTargetTransform();

            Camera = camera;
            camera.TargetToScreenTransform = _targetTransform;

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

        private Matrix RecalculateTargetTransform()
        {
            if (LayerScaleType == GlobalLayerScaleType.Uniform)
                return Matrix.Identity;

            if (ArtemisEngine.DisplayManager.IsBaseResolution)
                return Matrix.Identity;

            var baseRes = GameConstants.BaseResolution;
            var scale = ArtemisEngine.DisplayManager.ResolutionScale;
                
            switch (UniformScaleType)
            {
                case UniformLayerScaleType.Stretch:
                    return Matrix.CreateScale(scale.X, scale.Y, 1);
                case UniformLayerScaleType.Fit:
                    if (scale.X == scale.Y)
                        return Matrix.CreateScale(scale.X, scale.X, 1);
                    else if (scale.X > scale.Y)
                    {
                        return Matrix.CreateScale(scale.Y, scale.Y, 1) *
                               Matrix.CreateTranslation(baseRes.Width * (1 - scale.X) / 2f, 0, 0);
                    }
                    return Matrix.CreateScale(scale.X, scale.X, 1) *
                           Matrix.CreateTranslation(0, baseRes.Height * (1 - scale.Y) / 2f, 0);
                case UniformLayerScaleType.Fill:
                    if (scale.X == scale.Y)
                        return Matrix.CreateScale(scale.X, scale.X, 1);
                    else if (scale.X > scale.Y)
                    {
                        return Matrix.CreateScale(scale.X, scale.X, 1) *
                               Matrix.CreateTranslation(0, baseRes.Height / 2f * (scale.Y - scale.X), 0);
                    }
                    return Matrix.CreateScale(scale.Y, scale.Y, 1) *
                           Matrix.CreateTranslation(baseRes.Width / 2f * (scale.X - scale.Y), 0, 0);
                default:
                    throw new Exception(); // throw an actual exception...
            }
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
            if (ArtemisEngine.DisplayManager.ResolutionChanged)
            {
                LayerTarget.Dispose();
                LayerTarget = ArtemisEngine.RenderPipeline.CreateRenderTarget();
                _targetTransform = RecalculateTargetTransform();
            }

            rp.ClearRenderProperties();
            rp.SetRenderTarget(LayerTarget);
            rp.ClearGraphicsDevice(Color.Transparent);
            rp.SetRenderProperties(m: Camera.WorldToTargetTransform);

            var renderables = GetRenderables();

            if (LayerScaleType == GlobalLayerScaleType.Uniform)
            {
                foreach (var renderable in renderables)
                {
                    renderable.Render();
                }

                rp.UnsetRenderTarget();
                rp.ClearRenderProperties();
                rp.SetRenderProperties(
                    SpriteSortMode.Immediate,
                    BlendState.AlphaBlend,
                    m: _targetTransform);

                rp.Render(LayerTarget, Vector2.Zero);

                rp.ClearRenderProperties();
            }
            else
            {
                var isBaseRes = ArtemisEngine.DisplayManager.IsBaseResolution;
                var crntRes = ArtemisEngine.DisplayManager.WindowResolution;
                var resScale = ArtemisEngine.DisplayManager.ResolutionScale;

                foreach (var renderable in renderables)
                {
                    ProcessDynamicallyScaledRenderable(renderable, isBaseRes, crntRes, resScale);
                }
            }
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
                        throw new Exception(); // throw actual exception...
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
