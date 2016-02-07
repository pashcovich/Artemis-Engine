#region Using Statements

using Microsoft.Xna.Framework;

using System;

#endregion

namespace Artemis.Engine.Graphics.Layers
{
    public class ResolutionRelativeLayer : RenderLayer
    {
        public ResolutionRelativeLayer(string fullName) : base(fullName) { }

        public override void RenderIRenderable(IRenderable item)
        {
            var relativePosition = item.Position;
            var components = item.RenderComponent;

            var crntRes = ArtemisEngine.DisplayManager.WindowResolution;
            var baseRes = ArtemisEngine.GameProperties.BaseResolution;
            
            float resScale;
            ResolutionScaleType resScaleType;
            if (components.ResolutionScaleType.HasValue)
            {
                resScaleType = components.ResolutionScaleType.Value;
            }
            else
            {
                resScaleType = ResolutionScaleType.ByMin;
            }
            switch (resScaleType)
            {
                case ResolutionScaleType.Static:
                    resScale = 1;
                    break;
                case ResolutionScaleType.ByMin:
                    resScale = Math.Min(
                        (float)crntRes.Width / baseRes.Width,
                        (float)crntRes.Height / baseRes.Height
                        );
                    break;
                case ResolutionScaleType.ByMax:
                    resScale = Math.Max(
                        (float)crntRes.Width / baseRes.Width,
                        (float)crntRes.Height / baseRes.Height
                        );
                    break;
                case ResolutionScaleType.ByWidth:
                    resScale = (float)crntRes.Width / baseRes.Width;
                    break;
                case ResolutionScaleType.ByHeight:
                    resScale = (float)crntRes.Height / baseRes.Height;
                    break;
                default:
                    throw new RenderPipelineException(
                        String.Format(
                            "Unknown ResolutionScaleType value received '{0}' for IRenderable '{1}'.",
                            resScaleType, item
                            )
                        );
            }

            var absolutePosition = new Vector2(
                relativePosition.X * crntRes.Width,
                relativePosition.Y * crntRes.Height
                );

            var absoluteScale = components.Scale * resScale;

            Vector2? absoluteOffset = null;
            if (components.Offset.HasValue)
            {
                var offset = components.Offset.Value;
                absoluteOffset = new Vector2(
                    offset.X * crntRes.Width,
                    offset.Y * crntRes.Height
                    ) * resScale;
            }
            Rectangle? absoluteSource = null;
            if (components.Source.HasValue)
            {
                var source = components.Source.Value;
                absoluteSource = new Rectangle(
                    (int)(source.X * crntRes.Width),
                    (int)(source.Y * crntRes.Height),
                    (int)(source.Width * crntRes.Width),
                    (int)(source.Height * crntRes.Height)
                    );
            }

            ArtemisEngine.RenderPipeline.Render(
                components.Texture,
                absolutePosition,
                absoluteSource,
                components.Tint,
                components.Rotation,
                absoluteOffset,
                absoluteScale,
                components.SpriteEffects
                );
        }
    }
}
