#region Using Statements

using Microsoft.Xna.Framework;

#endregion

namespace Artemis.Engine.Graphics
{
    public static class TextureOrigins
    {
        public static readonly Vector2 Center = new Vector2(0.5f, 0.5f);
        public static readonly Vector2 TopLeft = new Vector2(0f, 0f);
        public static readonly Vector2 TopRight = new Vector2(1f, 0f);
        public static readonly Vector2 BottomLeft = new Vector2(0f, 1f);
        public static readonly Vector2 BottomRight = new Vector2(1f, 1f);
        public static readonly Vector2 TopCenter = new Vector2(0.5f, 0f);
        public static readonly Vector2 BottomCenter = new Vector2(0.5f, 1f);
        public static readonly Vector2 LeftCenter = new Vector2(0f, 0.5f);
        public static readonly Vector2 RightCenter = new Vector2(1f, 0.5f);
    }
}
