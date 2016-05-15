#region Using Statements

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

        internal class LockableRenderProperty<T>
        {
            public readonly string Name;
            private T _val;
            public T Value
            {
                get { return _val; }
                set
                {
                    if (Locked)
                        throw new RenderPipelineException(string.Format("{0} is locked.", Name));
                    _val = value;
                }
            }
            public bool Locked { get; set; }
            public LockableRenderProperty(string name) { Name = name; }
        }

        private LockableRenderProperty<SpriteSortMode> _spriteSortMode       = new LockableRenderProperty<SpriteSortMode>("SpriteSortMode");
        private LockableRenderProperty<BlendState> _blendState               = new LockableRenderProperty<BlendState>("BlendState");
        private LockableRenderProperty<SamplerState> _samplerState           = new LockableRenderProperty<SamplerState>("SamplerState");
        private LockableRenderProperty<DepthStencilState> _depthStencilState = new LockableRenderProperty<DepthStencilState>("DepthStencilState");
        private LockableRenderProperty<RasterizerState> _rasterizerState     = new LockableRenderProperty<RasterizerState>("RasterizerState");
        private LockableRenderProperty<Effect> _effect                       = new LockableRenderProperty<Effect>("Effect");
        private LockableRenderProperty<Matrix?> _matrix                      = new LockableRenderProperty<Matrix?>("Matrix");

        private RenderTarget2D _target;

        /// <summary>
        /// The current SpriteSortMode applied to the sprite batch.
        /// </summary>
        public SpriteSortMode SpriteSortMode { get { return _spriteSortMode.Value; } }

        /// <summary>
        /// Whether or not the SpriteSortMode property is locked (in which case it can't be changed).
        /// </summary>
        public bool SpriteSortModeLocked { get { return _spriteSortMode.Locked; } }

        /// <summary>
        /// The current BlendState applied to the sprite batch.
        /// </summary>
        public BlendState BlendState { get { return _blendState.Value; } }

        /// <summary>
        /// Whether or not the BlendState property is locked (in which case it can't be changed).
        /// </summary>
        public bool BlendStateLocked { get { return _blendState.Locked; } }

        /// <summary>
        /// The current SamplerState applied to the sprite batch.
        /// </summary>
        public SamplerState SamplerState { get { return _samplerState.Value; } }

        /// <summary>
        /// Whether or not the SamplerState property is locked (in which case it can't be changed).
        /// </summary>
        public bool SamplerStateLocked { get { return _samplerState.Locked; } }

        /// <summary>
        /// The current DepthStencilState applied to the sprite batch.
        /// </summary>
        public DepthStencilState DepthStencilState { get { return _depthStencilState.Value; } }

        /// <summary>
        /// Whether or not the DepthStencilState property is locked (in which case it can't be changed).
        /// </summary>
        public bool DepthStencilStateLocked { get { return _depthStencilState.Locked; } }

        /// <summary>
        /// The current RasterizerState applied to the sprite batch.
        /// </summary>
        public RasterizerState RasterizerState { get { return _rasterizerState.Value; } }

        /// <summary>
        /// Whether or not the RasterizerState property is locked (in which case it can't be changed).
        /// </summary>
        public bool RasterizerStateLocked { get { return _rasterizerState.Locked; } }

        /// <summary>
        /// The current Effect applied to the sprite batch.
        /// </summary>
        public Effect Effect { get { return _effect.Value; } }

        /// <summary>
        /// Whether or not the Effect property is locked (in which case it can't be changed).
        /// </summary>
        public bool EffectLocked { get { return _effect.Locked; } }

        /// <summary>
        /// The current Matrix applied to the sprite batch.
        /// </summary>
        public Matrix? Matrix { get { return _matrix.Value; } }

        /// <summary>
        /// Whether or not the Matrix property is locked (in which case it can't be changed).
        /// </summary>
        public bool MatrixLocked { get { return _matrix.Locked; } }

        /// <summary>
        /// The target currently being rendered to.
        /// </summary>
        public RenderTarget2D Target { get { return _target; } }

        /// <summary>
        /// GraphicsDevice that will be drawn to.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get; private set; }

        /// <summary>
        /// The GameKernel's GraphicsDeviceManager.
        /// 
        /// This is mostly useless for the user, just necessary for some internal operations.
        /// </summary>
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

        /// <summary>
        /// Whether or not the user has started a RenderCycle.
        /// </summary>
        internal bool BegunRenderCycle { get; set; }

        /// <summary>
        /// Whether or not the spriteBatch has been begun.
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

            _spriteSortMode.Value = SpriteSortMode.Deferred;
            _blendState.Value = null;
            _samplerState.Value = null;
            _depthStencilState.Value = null;
            _rasterizerState.Value = null;
            _effect.Value = null;
            _matrix.Value = null;
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
                          , float layerDepth           = 0
                          , bool originIsRelative      = false)
        {
            if (BegunRenderCycle)
            {
                var _colour = colour.HasValue ? colour.Value : Color.White;
                var _origin = origin.HasValue ? origin.Value : Vector2.Zero;
                var _scale  = scale.HasValue  ? scale.Value  : Vector2.One;

                if (originIsRelative)
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
                    _origin = new Vector2(rect.Width * _origin.X, rect.Height * _origin.Y);
                }

                if (!spriteBatchBegun)
                {
                    SpriteBatch.Begin(
                        _spriteSortMode.Value,
                        _blendState.Value,
                        _samplerState.Value,
                        _depthStencilState.Value,
                        _rasterizerState.Value,
                        _effect.Value,
                        _matrix.Value);
                    spriteBatchBegun = true;
                }

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

        /// <summary>
        /// Set the SpriteSortMode of the sprite batch.
        /// </summary>
        /// <param name="spriteSortMode"></param>
        public void SetSpriteSortMode(SpriteSortMode spriteSortMode)
        {
            _spriteSortMode.Value = spriteSortMode;
            ApplyRenderProperties();
        }

        /// <summary>
        /// Set the SpriteSortMode to it's default value.
        /// </summary>
        public void ClearSpriteSortMode()
        {
            _spriteSortMode.Value = SpriteSortMode.Deferred;
            ApplyRenderProperties();
        }

        /// <summary>
        /// Lock the SpriteSortMode property so the user can't change it.
        /// </summary>
        internal void LockSpriteSortMode() { _spriteSortMode.Locked = true; }

        /// <summary>
        /// Unlock the SpriteSortMode property so the user can change it.
        /// </summary>
        internal void UnlockSpriteSortMode() { _spriteSortMode.Locked = false; }

        /// <summary>
        /// Set the BlendState of the sprite batch.
        /// </summary>
        /// <param name="blendState"></param>
        public void SetBlendState(BlendState blendState)
        {
            _blendState.Value = blendState;
            ApplyRenderProperties();
        }

        /// <summary>
        /// Set the BlendState to it's default value.
        /// </summary>
        public void ClearBlendState()
        {
            _blendState.Value = null;
            ApplyRenderProperties();
        }

        /// <summary>
        /// Lock the BlendState property so the user can't change it.
        /// </summary>
        internal void LockBlendState() { _blendState.Locked = true; }

        /// <summary>
        /// Unlock the BlendState property so ther user can change it.
        /// </summary>
        internal void UnlockBlendState() { _blendState.Locked = false; }

        /// <summary>
        /// Set the SamplerState of the sprite batch to the given value.
        /// </summary>
        /// <param name="samplerState"></param>
        public void SetSamplerState(SamplerState samplerState)
        {
            _samplerState.Value = samplerState;
            ApplyRenderProperties();
        }

        /// <summary>
        /// Set the SamplerState to it's default value.
        /// </summary>
        public void ClearSamplerState()
        {
            _samplerState.Value = null;
            ApplyRenderProperties();
        }

        /// <summary>
        /// Lock the SamplerState property so the user can't change it.
        /// </summary>
        internal void LockSamplerState() { _samplerState.Locked = true; }

        /// <summary>
        /// Unlock the SamplerState property so the user can change it.
        /// </summary>
        internal void UnlockSamplerState() { _samplerState.Locked = false; }

        /// <summary>
        /// Set the DepthStencilState of the sprite batch to the given value.
        /// </summary>
        /// <param name="depthStencilState"></param>
        public void SetDepthStencilState(DepthStencilState depthStencilState)
        {
            _depthStencilState.Value = depthStencilState;
            ApplyRenderProperties();
        }

        /// <summary>
        /// Set the DepthStencilState to it's default value.
        /// </summary>
        public void ClearDepthStencilState()
        {
            _depthStencilState.Value = null;
            ApplyRenderProperties();
        }

        /// <summary>
        /// Lock the DepthStencilState property so the user can't change it.
        /// </summary>
        internal void LockDepthStencilState() { _depthStencilState.Locked = true; }

        /// <summary>
        /// Unlock the DepthStencilState property so the user can change it.
        /// </summary>
        internal void UnlockDepthStencilState() { _depthStencilState.Locked = false; }

        /// <summary>
        /// Set the RasterizerState of the sprite batch to the given value.
        /// </summary>
        /// <param name="rasterizerState"></param>
        public void SetRasterizerState(RasterizerState rasterizerState)
        {
            _rasterizerState.Value = rasterizerState;
            ApplyRenderProperties();
        }

        /// <summary>
        /// Set the RasterizerState to it's default value.
        /// </summary>
        public void ClearRasterizerState()
        {
            _rasterizerState.Value = null;
            ApplyRenderProperties();
        }

        /// <summary>
        /// Lock the RasterizerState property so the user can't change it.
        /// </summary>
        internal void LockRasterizerState() { _rasterizerState.Locked = true; }

        /// <summary>
        /// Unlock the RasterizerState property so the user can change it.
        /// </summary>
        internal void UnlockRasterizerState() { _rasterizerState.Locked = false; }

        /// <summary>
        /// Set the Effect of the sprite batch to the given value.
        /// </summary>
        /// <param name="effect"></param>
        public void SetEffect(Effect effect)
        {
            _effect.Value = effect;
            ApplyRenderProperties();
        }

        /// <summary>
        /// Set the Effect to it's default value.
        /// </summary>
        public void ClearEffect()
        {
            _effect.Value = null;
            ApplyRenderProperties();
        }

        /// <summary>
        /// Lock the Effect property so the user can't change it.
        /// </summary>
        internal void LockEffect() { _effect.Locked = true; }

        /// <summary>
        /// Unlock the Effect property so ther user can change it.
        /// </summary>
        internal void UnlockEffect() { _effect.Locked = false; }

        /// <summary>
        /// Set the Matrix of the sprite batch to the given value.
        /// </summary>
        /// <param name="matrix"></param>
        public void SetMatrix(Matrix? matrix)
        {
            _matrix.Value = matrix;
            ApplyRenderProperties();
        }

        /// <summary>
        /// Set the Matrix of the sprite batch back to it's original value.
        /// </summary>
        public void ClearMatrix()
        {
            _matrix.Value = null;
            ApplyRenderProperties();
        }

        /// <summary>
        /// Apply a matrix transform to the sprite batch.
        /// 
        /// This is different from SetMatrix as it always works regardless of whether or not
        /// the Matrix property is locked, and it doesn't just set the matrix to the given
        /// value, but actually applies it to the previous value (by multiplying the previou
        /// matrix by the given matrix).
        /// </summary>
        /// <param name="matrix"></param>
        public void ApplyMatrix(Matrix matrix)
        {
            bool wasLocked = _matrix.Locked;
            if (_matrix.Locked)
                _matrix.Locked = false;
            if (_matrix.Value != null)
                _matrix.Value = matrix * _matrix.Value;
            else
                _matrix.Value = matrix;
            _matrix.Locked = wasLocked;
        }

        /// <summary>
        /// Unapply a matrix transform from the sprite batch.
        /// 
        /// This is the inverse operation of ApplyMatrix.
        /// </summary>
        /// <param name="matrix"></param>
        public void UnapplyMatrix(Matrix matrix)
        {
            if (_matrix.Value == null)
                return;
            bool wasLocked = _matrix.Locked;
            if (_matrix.Locked)
                _matrix.Locked = false;
            if (_matrix.Value == matrix)
                _matrix.Value = null;
            else
                _matrix.Value = Microsoft.Xna.Framework.Matrix.Invert(matrix) * _matrix.Value;
            _matrix.Locked = wasLocked;
        }

        /// <summary>
        /// Lock the Matrix property so the user can't change it.
        /// </summary>
        internal void LockMatrix() { _matrix.Locked = true; }

        /// <summary>
        /// Unlock the Matrix property so the user can change it.
        /// </summary>
        internal void UnlockMatrix() { _matrix.Locked = false; }

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
                                       , Matrix? m             = null
                                       , bool ignoreDefaults   = true
                                       , bool applyMatrix      = false )
        {
            if (ignoreDefaults)
            {
                if (ssm != SpriteSortMode.Deferred && ssm != _spriteSortMode.Value)
                    _spriteSortMode.Value = ssm;
                if (bs != null && bs != _blendState.Value)
                    _blendState.Value = bs;
                if (ss != null && ss != _samplerState.Value)
                    _samplerState.Value = ss;
                if (dss != null && dss != _depthStencilState.Value)
                    _depthStencilState.Value = dss;
                if (rs != null && rs != _rasterizerState.Value)
                    _rasterizerState.Value = rs;
                if (e != null && e != _effect.Value)
                    _effect.Value = e;
                if (m != null && m != _matrix.Value)
                {
                    if (applyMatrix)
                    {
                        if (m.HasValue)
                            ApplyMatrix(m.Value);
                        else
                            throw new RenderPipelineException("Cannot apply a null matrix.");
                    }
                    else
                        _matrix.Value = m;
                }
            }
            else
            {
                if (ssm != _spriteSortMode.Value)
                    _spriteSortMode.Value = ssm;
                if (bs != _blendState.Value)
                    _blendState.Value = bs;
                if (ss != _samplerState.Value)
                    _samplerState.Value = ss;
                if (dss != _depthStencilState.Value)
                    _depthStencilState.Value = dss;
                if (rs != _rasterizerState.Value)
                    _rasterizerState.Value = rs;
                if (e != _effect.Value)
                    _effect.Value = e;
                if (m != _matrix.Value)
                {
                    if (applyMatrix)
                    {
                        if (m.HasValue)
                            ApplyMatrix(m.Value);
                        else
                            throw new RenderPipelineException("Cannot apply a null matrix.");
                    }
                    else
                        _matrix.Value = m;
                }
            }
            ApplyRenderProperties();
        }

        /// <summary>
        /// Set the current render properties to those specified in the given packet.
        /// </summary>
        /// <param name="packet"></param>
        public void SetRenderProperties(
            SpriteBatchPropertiesPacket packet, bool ignoreDefaults = true, bool applyMatrix = false)
        {
            SetRenderProperties(
                packet.SpriteSortMode,
                packet.BlendState,
                packet.SamplerState,
                packet.DepthStencilState,
                packet.RasterizerState,
                packet.Effect,
                packet.Matrix,
                ignoreDefaults,
                applyMatrix);
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
                spriteBatchBegun = false;
            }
            _spriteSortMode.Value = SpriteSortMode.Deferred;
            _blendState.Value = null;
            _samplerState.Value = null;
            _depthStencilState.Value = null;
            _rasterizerState.Value = null;
            _effect.Value = null;
            _matrix.Value = null;
        }

        /// <summary>
        /// Lock the render properties so the user can't change them.
        /// </summary>
        internal void LockRenderProperties()
        {
            _spriteSortMode.Locked = true;
            _blendState.Locked = true;
            _samplerState.Locked = true;
            _depthStencilState.Locked = true;
            _rasterizerState.Locked = true;
            _effect.Locked = true;
            _matrix.Locked = true;
        }

        /// <summary>
        /// Unlock the render properties so the user can change them.
        /// </summary>
        internal void UnlockRenderProperties()
        {
            _spriteSortMode.Locked = false;
            _blendState.Locked = false;
            _samplerState.Locked = false;
            _depthStencilState.Locked = false;
            _rasterizerState.Locked = false;
            _effect.Locked = false;
            _matrix.Locked = false;
        }

        /// <summary>
        /// Apply changes to the render properties.
        /// </summary>
        private void ApplyRenderProperties()
        {
            if (spriteBatchBegun)
            {
                SpriteBatch.End();
            }
            SpriteBatch.Begin(
                _spriteSortMode.Value,
                _blendState.Value,
                _samplerState.Value,
                _depthStencilState.Value,
                _rasterizerState.Value,
                _effect.Value,
                _matrix.Value
                );
            spriteBatchBegun = true;
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
        /// Set the render target to the given target.
        /// </summary>
        /// <param name="target"></param>
        public void SetRenderTarget(RenderTarget2D target)
        {
            _target = target;
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
        /// Create a basic RenderTarget2D object with dimensions equal to the current
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

        /// <summary>
        /// Create a RenderTarget2D with dimensions equal to the current window resolution.
        /// </summary>
        /// <param name="fill"></param>
        /// <returns></returns>
        public RenderTarget2D CreateRenderTarget(Color? fill = null)
        {
            var res = ArtemisEngine.DisplayManager.WindowResolution;
            return CreateRenderTarget(res.Width, res.Height, fill);
        }

        /// <summary>
        /// Create a RenderTarget2D with dimensions equal to the current window resolution.
        /// </summary>
        /// <param name="preferredFormat"></param>
        /// <param name="preferredDepthFormat"></param>
        /// <param name="preferredMultiSampleCount"></param>
        /// <param name="usage"></param>
        /// <param name="fill"></param>
        /// <param name="mipMap"></param>
        /// <returns></returns>
        public RenderTarget2D CreateRenderTarget( SurfaceFormat preferredFormat
                                                , DepthFormat preferredDepthFormat
                                                , int preferredMultiSampleCount
                                                , RenderTargetUsage usage
                                                , Color? fill = null
                                                , bool mipMap = false )
        {
            return CreateRenderTarget(
                ArtemisEngine.DisplayManager.WindowResolution, 
                preferredFormat, 
                preferredDepthFormat, 
                preferredMultiSampleCount, 
                usage, 
                fill, 
                mipMap);
        }

        /// <summary>
        /// Create a RenderTarget2D with dimensions equal to the given resolution.
        /// </summary>
        /// <param name="resolution"></param>
        /// <param name="fill"></param>
        /// <returns></returns>
        public RenderTarget2D CreateRenderTarget(Resolution resolution, Color? fill = null)
        {
            return CreateRenderTarget(resolution.Width, resolution.Height, fill);
        }
        
        /// <summary>
        /// Create a RenderTarget2D.
        /// </summary>
        /// <param name="resolution"></param>
        /// <param name="preferredFormat"></param>
        /// <param name="preferredDepthFormat"></param>
        /// <param name="preferredMultiSampleCount"></param>
        /// <param name="usage"></param>
        /// <param name="fill"></param>
        /// <param name="mipMap"></param>
        /// <returns></returns>
        public RenderTarget2D CreateRenderTarget( Resolution resolution
                                                , SurfaceFormat preferredFormat
                                                , DepthFormat preferredDepthFormat
                                                , int preferredMultiSampleCount
                                                , RenderTargetUsage usage
                                                , Color? fill = null
                                                , bool mipMap = false )
        {
            Color _fill = fill.HasValue ? fill.Value : Color.Transparent;

            var target = new RenderTarget2D(
                GraphicsDevice, 
                resolution.Width, 
                resolution.Height, 
                mipMap, 
                preferredFormat, 
                preferredDepthFormat, 
                preferredMultiSampleCount, 
                usage);

            GraphicsDevice.SetRenderTarget(target);
            GraphicsDevice.Clear(_fill);
            GraphicsDevice.SetRenderTarget(null);

            return target;
        }

        /// <summary>
        /// Create a RenderTarget2D with dimensions equal to the game's base resolution.
        /// </summary>
        /// <param name="fill"></param>
        /// <returns></returns>
        public RenderTarget2D CreateBaseResRenderTarget(Color? fill = null)
        {
            var res = GameConstants.BaseResolution;
            return CreateRenderTarget(res.Width, res.Height, fill);
        }

        /// <summary>
        /// Create a RenderTarget2D with dimensions equal to the game's base resolution.
        /// </summary>
        /// <param name="preferredFormat"></param>
        /// <param name="preferredDepthFormat"></param>
        /// <param name="preferredMultiSampleCount"></param>
        /// <param name="usage"></param>
        /// <param name="fill"></param>
        /// <param name="mipMap"></param>
        /// <returns></returns>
        public RenderTarget2D CreateBaseResRenderTarget( SurfaceFormat preferredFormat
                                                       , DepthFormat preferredDepthFormat
                                                       , int preferredMultiSampleCount
                                                       , RenderTargetUsage usage
                                                       , Color? fill = null
                                                       , bool mipMap = false )
        {
            return CreateRenderTarget(
                GameConstants.BaseResolution,
                preferredFormat,
                preferredDepthFormat,
                preferredMultiSampleCount,
                usage,
                fill,
                mipMap);
        }
    }
}


