#region Using Statements

using Artemis.Engine.Maths.Geometry;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

#endregion

namespace Artemis.Engine.Graphics
{
    public class ResolutionRelativeRenderLayer : AbstractOrderableRenderLayer
    {
        private RenderPipeline rp; // the global render pipeline,

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
                    RequiresTargetTransformRecalc = true;
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
                    RequiresTargetTransformRecalc = true;
                }
                else
                {
                    _uniformScaleType = value;
                    RecalculateTargetTransform();
                }
            }
        }

        public ResolutionRelativeRenderLayer(string fullName)
            : this(fullName, GlobalLayerScaleType.Dynamic, UniformLayerScaleType.Stretch) { }

        public ResolutionRelativeRenderLayer( string fullName
                                            , GlobalLayerScaleType layerScaleType 
                                            , UniformLayerScaleType uniformScaleType )
            : base(fullName)
        {
            rp = ArtemisEngine.RenderPipeline; // for convenience

            LayerScaleType = layerScaleType;
            UniformScaleType = uniformScaleType;
            RecalculateTargetTransform();
        }

        /// <summary>
        /// Recalculate the TargetToScreenTransform matrix (required when resolution changes).
        /// </summary>
        protected internal override void RecalculateTargetTransform()
        {
            if (LayerScaleType == GlobalLayerScaleType.Dynamic)
            {
                TargetToScreenTransform = Matrix.Identity;
            }
            else if (ArtemisEngine.DisplayManager.IsBaseResolution)
            {
                TargetToScreenTransform = Matrix.Identity;
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
                TargetToScreenTransform = transform;
            }
            RequiresTargetTransformRecalc = false;
        }

        /// <summary>
        /// Process a RenderableObject when the GlobalLayerScaleType is set to Dynamic.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isBaseRes"></param>
        /// <param name="crntRes"></param>
        /// <param name="resScale"></param>
        protected void ProcessDynamicallyScaledRenderable(
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
                                "Unknown ResolutionScaleType '{0}' received on object '{1}'.", scaleType, obj));
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
                    return obj => obj.InternalRender(SeenRenderables);
                case GlobalLayerScaleType.Dynamic:
                    var isBaseRes = ArtemisEngine.DisplayManager.IsBaseResolution;
                    var crntRes   = ArtemisEngine.DisplayManager.WindowResolution;
                    var resScale  = ArtemisEngine.DisplayManager.ResolutionScale;

                    return obj => ProcessDynamicallyScaledRenderable(obj, isBaseRes, crntRes, resScale);
                default:
                    throw new RenderLayerException(
                        String.Format(
                            "Unknown GlobalLayerScaleType '{0}' supplied to layer '{1}'.",
                            LayerScaleType, tempFullName));
            }
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
                m: TargetToScreenTransform);

            rp.Render(LayerTarget, Vector2.Zero);

            rp.ClearRenderProperties();
        }

        /// <summary>
        /// Called after everything is rendered (also after RenderLayerTarget is called).
        /// </summary>
        protected override void PostRender()
        {
            if (RequiresTargetTransformRecalc)
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
