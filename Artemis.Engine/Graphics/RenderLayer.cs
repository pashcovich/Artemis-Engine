#region Using Statements

using Artemis.Engine.Maths.Geometry;
using Artemis.Engine.Utilities.UriTree;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

#endregion

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

        public ResolutionScaleRules LayerResolutionScaleRules { get; set; } // CURRENTLY NOT USED

        protected RenderTarget2D LayerTarget { get; private set; }

        /// <summary>
        /// Whether or not this render layer is managed by a LayerManager or not.
        /// </summary>
        public bool Managed { get; internal set; }

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
            Managed = false;
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
        /// Render all items on this layer.
        /// </summary>
        public void Render()
        {
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
                ProcessRenderable(renderable, resolution, isBaseRes, resScale);
            }

            ArtemisEngine.RenderPipeline.UnsetRenderTarget();

            FinalizeRender();
        }

        private void ProcessRenderable( IRenderable renderable
                                      , Resolution resolution
                                      , bool isBaseRes
                                      , Vector2 resScale )
        {
            var asManipulable = renderable as IManipulableRenderable;
            if (asManipulable == null)
            {
                renderable.Render();
            }
            else
            {
                ProcessManipulableRenderable(asManipulable, resolution, isBaseRes, resScale);
            }
        }

        private enum _scaleDirection
        {
            Width,
            Height,
            Both
        }

        private void ProcessManipulableRenderable( IManipulableRenderable renderable
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
                // If "ScaleUniformly" is true, then the only scaling we have to do is
                // the default scale value associated with the object. We don't have to
                // do any resolution relative scaling.
                scale = components.Scale;
            }
            else if (!isBaseRes)
            {
                float scaleFactor;
                _scaleDirection scaleDirection; // 0 - width, 1 - height, 2 - both
                switch (resScaleRules.ScaleType)
                {
                    case ResolutionScaleType.BY_MIN:
                        scaleFactor = MathHelper.Min(resScale.X, resScale.Y);
                        scaleDirection = (_scaleDirection)Convert.ToInt32(scaleFactor == resScale.X);
                        break;
                    case ResolutionScaleType.BY_MAX:
                        scaleFactor = MathHelper.Max(resScale.X, resScale.Y);
                        scaleDirection = (_scaleDirection)Convert.ToInt32(scaleFactor == resScale.X);
                        break;
                    case ResolutionScaleType.BY_WIDTH:
                        scaleFactor = resScale.X;
                        scaleDirection = _scaleDirection.Width;
                        break;
                    case ResolutionScaleType.BY_HEIGHT:
                        scaleFactor = resScale.Y;
                        scaleDirection = _scaleDirection.Height;
                        break;
                    case ResolutionScaleType.WITH_RES:
                        scaleFactor = 0;
                        scaleDirection = _scaleDirection.Both;
                        break;
                    default:
                        throw new Exception();
                }

                if (scaleDirection == _scaleDirection.Both)
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
                        if (scaleDirection == _scaleDirection.Width)
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
                        scale = scaleDirection == _scaleDirection.Width ?
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

            RenderIManipulable(renderable, newPos, newComponents);
        }

        private void RenderIManipulable(
            IManipulableRenderable renderable, RelativePosition newPos, RenderComponents newComponents)
        {
            // Swap out the old components for the new components and render.
            var oldPos = renderable.Position;
            var oldComps = renderable.Components;
            
            renderable.Position = newPos;
            renderable.Components = newComponents;

            renderable.Render();

            renderable.Position = oldPos;
            renderable.Components = oldComps;
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
