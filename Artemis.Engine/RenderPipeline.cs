﻿#region Using Statements

using Artemis.Engine.Graphics;
using Artemis.Engine.Maths.Geometry;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Artemis.Engine
{
    /// <summary>
    /// RenderPipeline Class that draws using the Monogame tools. 
    /// Having this allows us to keep the user away from Monogame 
    /// and simplify some common drawing procedures
    /// </summary>
    public class RenderPipeline
    {

        /// <summary>
        /// GraphicsDevice that will be drawn to.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get; private set; }

        /// <summary>
        /// Mostly useless.
        /// </summary>
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

        /// <summary>
        /// If the user has started a RenderCycle.
        /// </summary>
        internal bool BegunRenderCycle { get; set; }

        /// <summary>
        /// If the spriteBatch has been begun.
        /// </summary>
        private bool spriteBatchBegun { get; set; }

        internal SpriteBatch SpriteBatch; // SpriteBatch that draws everything.

        /// <summary>
        /// Constructs a RenderPipeline with everything a user might need to draw things
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="gd"></param>
        /// <param name="gdm"></param>
        public RenderPipeline(SpriteBatch sb, GraphicsDevice gd, GraphicsDeviceManager gdm)
        {
            SpriteBatch = sb;
            GraphicsDevice = gd;
            GraphicsDeviceManager = gdm;
        }

        /// <summary>
        /// Begin the render cycle, in which rendering can take place.
        /// </summary>
        internal void BeginRenderCycle()
        {
            BegunRenderCycle = true;
            spriteBatchBegun = true;

            GraphicsDevice.Clear(ArtemisEngine.DisplayManager.BackgroundColour);
            SpriteBatch.Begin();
        }

        /// <summary>
        /// End the render cycle. This ends the spriteBatch as well.
        /// </summary>
        internal void EndRenderCycle()
        {
            BegunRenderCycle = false;
            spriteBatchBegun = false;
            SpriteBatch.End();
        }

        /// <summary>
        /// Directly render a texture to the screen with the given parameters.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        /// <param name="sourceRectangle"></param>
        /// <param name="colour"></param>
        /// <param name="rotation"></param>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        /// <param name="effects"></param>
        /// <param name="layerDepth"></param>
        public void Render(Texture2D texture
                          , Vector2 position
                          , Rectangle? sourceRectangle = null
                          , Color? colour              = null
                          , double rotation            = 0
                          , Vector2? origin            = null
                          , Vector2? scale             = null
                          , SpriteEffects effects      = SpriteEffects.None
                          , float layerDepth           = 0 )
        {
            if (BegunRenderCycle)
            {
                var _colour = colour.HasValue ? colour.Value : Color.White;
                var _origin = origin.HasValue ? origin.Value : Vector2.Zero;
                var _scale  = scale.HasValue  ? scale.Value  : Vector2.One;

                SpriteBatch.Draw(
                    texture, position, sourceRectangle, _colour, (float)rotation,
                    _origin, _scale, effects, layerDepth
                    );
            }
            else
            {
                throw new RenderPipelineException(
                    "Rendering must occur in the render cycle.");
            }
        }

        public void Render(Texture2D texture
                          , RelativePosition position
                          , Rectangle? sourceRectangle = null
                          , Color? colour              = null
                          , double rotation            = 0
                          , Vector2? scale             = null
                          , SpriteEffects effects      = SpriteEffects.None
                          , float layerDepth           = 0 )
        {
            Rectangle rect;
            if (sourceRectangle.HasValue)
            {
                rect = (Rectangle)sourceRectangle;
            }
            else
            {
                rect = texture.Bounds;
            }
            var rel = position.RelativeTo;
            var origin = new Vector2(rect.Width * rel.xOffset, rect.Height * rel.yOffset);

            Render( texture
                  , position.Position
                  , sourceRectangle
                  , colour
                  , rotation
                  , origin
                  , scale
                  , effects
                  , layerDepth );
        }

        /// <summary>
        /// Set the current render properties, which are applied to everything rendered up
        /// until ClearRenderProperties is called or SetRenderProperties is called with
        /// different arguments.
        /// </summary>
        /// <param name="ssm"></param>
        /// <param name="bs"></param>
        /// <param name="ss"></param>
        /// <param name="dss"></param>
        /// <param name="rs"></param>
        /// <param name="e"></param>
        /// <param name="m"></param>
        public void SetRenderProperties( SpriteSortMode ssm    = SpriteSortMode.Deferred
                                       , BlendState bs         = null
                                       , SamplerState ss       = null
                                       , DepthStencilState dss = null
                                       , RasterizerState rs    = null
                                       , Effect e              = null
                                       , Matrix? m             = null)
        {
            if (spriteBatchBegun)
            {
                SpriteBatch.End();
            }
            SpriteBatch.Begin(ssm, bs, ss, dss, rs, e, m);
            spriteBatchBegun = true;
        }

        /// <summary>
        /// Set the current render properties to those specified in the given packet.
        /// </summary>
        /// <param name="packet"></param>
        public void SetRenderProperties(RenderPropertiesPacket packet)
        {
            SetRenderProperties(
                packet.SpriteSortMode,
                packet.BlendState,
                packet.SamplerState,
                packet.DepthStencilState,
                packet.RasterizerState,
                packet.Effect,
                packet.Matrix);
        }

        /// <summary>
        /// Clear the graphics device with the given color.
        /// </summary>
        /// <param name="color"></param>
        public void ClearGraphicsDevice(Color color)
        {
            GraphicsDevice.Clear(color);
        }

        /// <summary>
        /// Reset the current render properties to their default values. For the default
        /// values, see the default values of SetRenderProperties' parameters.
        /// </summary>
        public void ClearRenderProperties()
        {
            if (spriteBatchBegun)
            {
                SpriteBatch.End();
            }
            SpriteBatch.Begin();
        }

        /// <summary>
        /// Set the render target to the given target.
        /// </summary>
        /// <param name="target"></param>
        public void SetRenderTarget(RenderTarget2D target)
        {
            GraphicsDevice.SetRenderTarget(target);
        }

        /// <summary>
        /// Unset the current render target.
        /// </summary>
        public void UnsetRenderTarget()
        {
            GraphicsDevice.SetRenderTarget(null);
        }

        /// <summary>
        /// Create a basic RenderTarget object with dimensions equal to the current
        /// resolution and fill colour equal to the given colour (or transparent if
        /// null).
        /// </summary>
        /// <param name="fill"></param>
        /// <returns></returns>
        public RenderTarget2D CreateRenderTarget(int width, int height, Color? fill = null)
        {
            Color _fill = fill.HasValue ? fill.Value : Color.Transparent;

            var target = new RenderTarget2D(GraphicsDevice, width, height);

            GraphicsDevice.SetRenderTarget(target);
            GraphicsDevice.Clear(_fill);
            GraphicsDevice.SetRenderTarget(null);

            return target;
        }

        public RenderTarget2D CreateRenderTarget(Color? fill = null)
        {
            var res = ArtemisEngine.DisplayManager.WindowResolution;
            return CreateRenderTarget(res.Width, res.Height, fill);
        }

        public RenderTarget2D CreateRenderTarget(Resolution resolution, Color? fill = null)
        {
            return CreateRenderTarget(resolution.Width, resolution.Height, fill);
        }
    }
}


