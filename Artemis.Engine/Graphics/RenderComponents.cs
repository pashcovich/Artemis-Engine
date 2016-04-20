
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artemis.Engine.Graphics
{
    public class RenderComponents
    {
        public Rectangle? SourceRectangle { get; set; }

        public Color? Tint { get; set; }

        public double Rotation { get; set; }

        public Vector2? Scale { get; set; }

        public SpriteEffects SpriteEffects { get; set; }

        public RenderComponents( Rectangle? sourceRectangle = null
                               , Color? colour              = null
                               , double rotation            = 0
                               , Vector2? scale             = null
                               , SpriteEffects effects      = SpriteEffects.None )
        {
            SourceRectangle = sourceRectangle;
            Tint = colour;
            Rotation = rotation;
            Scale = scale;
            SpriteEffects = effects;
        }
    }
}
