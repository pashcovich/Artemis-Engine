using Artemis.Engine.Maths.Geometry;
using Artemis.Engine.Utilities.UriTree;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace Artemis.Engine.Graphics
{
    public class RenderLayer : UriTreeObserverNode<RenderLayer, RenderableGroup>
    {
        private const string TOP_LEVEL = "ALL";
        protected RenderableGroup AllRenderables;
        internal string tempName { get; private set; }

        /// <summary>
        /// Whether or not to scale the layer uniformly.
        /// </summary>
        public bool ScaleUniformly { get; set; }

        public ResolutionScaleRules LayerResolutionScaleRules { get; set; }

        protected RenderTarget2D LayerTarget { get; private set; }

        public RenderLayer(string fullName)
            : base(UriUtilities.GetLastPart(fullName))
        {
            // We have to store the full name until the layer gets added to 
            // a LayerManager.
            tempName = fullName;

            AllRenderables = new RenderableGroup(TOP_LEVEL);
            AddObservedNode(TOP_LEVEL, AllRenderables);

            LayerTarget = ArtemisEngine.RenderPipeline.CreateRenderTarget();
            
            ScaleUniformly = false;
            LayerResolutionScaleRules = null;
        }

        public void RenderAll()
        {
            foreach (var layer in Subnodes.Values)
            {
                layer.RenderAll();
            }

            if (ArtemisEngine.DisplayManager.ResolutionChanged)
            {
                // Create a new render target with dimensions matching the current resolution.
                LayerTarget = ArtemisEngine.RenderPipeline.CreateRenderTarget();
            }

            ArtemisEngine.RenderPipeline.SetRenderTarget(LayerTarget);
            ArtemisEngine.RenderPipeline.ClearGraphicsDevice(Color.Transparent);

            // Load these once here instead of each time we call ProcessManipulableRenderable.
            var resolution = ArtemisEngine.DisplayManager.WindowResolution;
            var isBaseRes  = ArtemisEngine.DisplayManager.IsBaseResolution;
            var resScale   = ArtemisEngine.DisplayManager.ResolutionScale;

            var renderables = AllRenderables.RetrieveAll();

            foreach (var renderable in renderables)
            {
                if (renderable.Valid)
                {
                    ProcessRenderable(renderable, resolution, isBaseRes, resScale);
                }
            }

            ArtemisEngine.RenderPipeline.UnsetRenderTarget();

            if (true /* should only be called when not added to a render layer */)
            {
                FinalizeRender();
            }
        }

        private void ProcessRenderable( AbstractRenderable renderable
                                      , Resolution resolution
                                      , bool isBaseRes
                                      , Vector2 resScale )
        {
            var asManipulable = renderable as AbstractManipulableRenderable;
            if (asManipulable == null)
            {
                renderable.Render();
            }
            else
            {
                ProcessManipulableRenderable(asManipulable, resolution, isBaseRes, resScale);
            }
        }

        private void ProcessManipulableRenderable( AbstractManipulableRenderable renderable
                                                 , Resolution res
                                                 , bool isBaseRes
                                                 , Vector2 resScale )
        {
            var position = renderable.Position;
            var components = renderable.Components;
            var resScaleRules = renderable.ResolutionScaleRules;

            Vector2? scale = null;
            if (ScaleUniformly)
            {
                scale = components.Scale;
            }
            else if (isBaseRes)
            {
                float scaleFactor;
                int scaleDirection; // 0 - width, 1 - height, 2 - both
                switch (resScaleRules.ScaleType)
                {
                    case ResolutionScaleType.BY_MIN:
                        scaleFactor = MathHelper.Min(resScale.X, resScale.Y);
                        scaleDirection = Convert.ToInt32(scaleFactor == resScale.X);
                        break;
                    case ResolutionScaleType.BY_MAX:
                        scaleFactor = MathHelper.Max(resScale.X, resScale.Y);
                        scaleDirection = Convert.ToInt32(scaleFactor == resScale.X);
                        break;
                    case ResolutionScaleType.BY_WIDTH:
                        scaleFactor = resScale.X;
                        scaleDirection = 0;
                        break;
                    case ResolutionScaleType.BY_HEIGHT:
                        scaleFactor = resScale.Y;
                        scaleDirection = 1;
                        break;
                    case ResolutionScaleType.WITH_RES:
                        scaleFactor = 0;
                        scaleDirection = 2;
                        break;
                    default:
                        throw new Exception();
                }

                if (scaleDirection == 2)
                {
                    if (resScaleRules.MaintainAspectRatio)
                    {
                        Console.WriteLine("Something"); // warning or something should be thrown.
                    }
                    scale = VectorUtils.ComponentwiseProduct(components.Scale ?? Vector2.One, resScale);
                }
                else if (resScaleRules.MaintainAspectRatio)
                {
                    scale = (components.Scale ?? Vector2.One) * scaleFactor;
                }
                else
                {
                    if (components.Scale.HasValue)
                    {
                        var c_scale = components.Scale.Value;
                        if (scaleDirection == 0)
                        {
                            scale = new Vector2(c_scale.X * scaleFactor, c_scale.Y);
                        }
                        else
                        {
                            scale = new Vector2(c_scale.X, c_scale.Y * scaleFactor);
                        }
                    }
                    else
                    {
                        scale = scaleDirection == 0 ?
                                new Vector2(scaleFactor, 1) :
                                new Vector2(1, scaleFactor);
                    }
                }
            }

            RelativePosition newPos = position;
            if (resScaleRules.RelativePositioning)
            {
                var oldVec = position.Position;
                var newVec = new Vector2(oldVec.X * res.Width, oldVec.Y * res.Height);
                newPos = new RelativePosition(newVec, position.RelativeTo);
            }

            var newComponents = new RenderComponents(
                components.SourceRectangle,
                components.Tint,
                components.Rotation,
                scale,
                components.SpriteEffects);
            renderable.Render(newPos, newComponents);
        }

        private void FinalizeRender()
        {
            ArtemisEngine.RenderPipeline.SetRenderProperties(
                SpriteSortMode.Immediate, BlendState.AlphaBlend);

            ArtemisEngine.RenderPipeline.Render(LayerTarget, Vector2.Zero);

            ArtemisEngine.RenderPipeline.ClearRenderProperties();
        }
    }
}
