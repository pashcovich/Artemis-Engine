#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Artemis.Engine.Graphics
{

    /// <summary>
    /// An object representing the properties that can be passed to
    /// the RenderPipeline.
    /// </summary>
    public sealed class RenderPropertiesPacket
    {
        public SpriteSortMode SpriteSortMode;
        public BlendState BlendState;
        public SamplerState SamplerState;
        public DepthStencilState DepthStencilState;
        public RasterizerState RasterizerState;
        public Effect Effect;
        public Matrix? Matrix;

        public RenderPropertiesPacket( SpriteSortMode ssm    = SpriteSortMode.Deferred
                                     , BlendState bs         = null
                                     , SamplerState ss       = null
                                     , DepthStencilState dss = null
                                     , RasterizerState rs    = null
                                     , Effect e              = null
                                     , Matrix? m             = null )
        {
            SpriteSortMode    = ssm;
            BlendState        = bs;
            SamplerState      = ss;
            DepthStencilState = dss;
            RasterizerState   = rs;
            Effect            = e;
            Matrix            = m;
        }
    }
}
