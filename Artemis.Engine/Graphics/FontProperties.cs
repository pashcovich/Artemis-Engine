#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Artemis.Engine.Graphics
{
    public class FontProperties
    {
        /// <summary>
        /// The color to tint the text.
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
        /// The text origin; the default is (0,0) which represents the upper-left corner.
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

        public FontProperties( Color? color                = null
                             , double rotation             = 0
                             , Vector2? scale              = null
                             , Vector2? origin             = null 
                             , SpriteEffects spriteEffects = SpriteEffects.None
                             , bool originIsRelative       = false )
        {
            Tint = color;
            Rotation = rotation;
            Scale = scale;
            Origin = origin.HasValue ? origin.Value : Vector2.Zero;
            SpriteEffects = spriteEffects;
            OriginIsRelative = originIsRelative;
        }
    }
}
