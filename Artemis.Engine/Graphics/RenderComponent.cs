#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Artemis.Engine.Graphics
{
    /// <summary>
    /// A class that represents the default render properties supplied to
    /// the SpriteBatch when rendering an IRenderable object.
    /// </summary>
    public class RenderComponent 
    {
        public virtual Texture2D Texture { get; set; }

        public virtual Rectangle? Source { get; set; }

        public virtual Color Tint { get; set; }

        public virtual double Rotation { get; set; }

        public virtual Vector2? Offset { get; set; }

        public virtual Vector2? Scale { get; set; }

        public virtual SpriteEffects SpriteEffects { get; set; }

        public virtual ResolutionScaleType? ResolutionScaleType { get; set; }
    }
}
