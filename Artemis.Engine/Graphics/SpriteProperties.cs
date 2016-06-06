#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Artemis.Engine.Graphics
{
    public class SpriteProperties
    {
        /// <summary>
        /// A rectangle that specifies (in texels) the source texels from 
        /// a texture. Use null to draw the entire texture.
        /// </summary>
        public Rectangle? SourceRectangle;

        /// <summary>
        /// The color to tint a sprite. Use Color.White for full color with no tinting.
        /// </summary>
        public Color? Tint;

        /// <summary>
        /// Specifies the angle (in radians) to rotate the sprite about its center.
        /// </summary>
        public double Rotation;

        /// <summary>
        /// Scale factor.
        /// </summary>
        public Vector2? Scale;

        /// <summary>
        /// The sprite origin; the default is (0,0) which represents the upper-left corner.
        /// </summary>
        public Vector2 Origin;

        /// <summary>
        /// Whether or not the `Origin` value is a relative or absolute value.
        /// </summary>
        public bool OriginIsRelative;

        /// <summary>
        /// Effects to apply.
        /// </summary>
        public SpriteEffects SpriteEffects;

        public SpriteProperties( Rectangle? sourceRectangle = null
                               , Color? colour              = null
                               , double rotation            = 0
                               , Vector2? scale             = null
                               , Vector2? origin            = null
                               , SpriteEffects effects      = SpriteEffects.None 
                               , bool originIsRelative      = false )
        {
            SourceRectangle = sourceRectangle;
            Tint = colour;
            Rotation = rotation;
            Scale = scale;
            Origin = origin.HasValue ? origin.Value : PositionOffsets.TopLeft;
            SpriteEffects = effects;
            OriginIsRelative = originIsRelative;
        }
    }
}
