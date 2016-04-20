using Microsoft.Xna.Framework;

namespace Artemis.Engine
{
    /// <summary>
    /// RelativePosition represents a Vector2 that is offset relative to a given rectangle.
    /// 
    /// This is used by certain rectangular objects to make it easier to represent positions
    /// that don't refer to the top left corner of the rectangle. For example, when rendering
    /// an image, you can use a RelativePosition with RelativeTo set to Center to indicate that
    /// the position actually represents the center of the image rather than the top left
    /// corner.
    /// </summary>
    public class RelativePosition
    {
        public Vector2 Position { get; set; }

        public RelativePositionType RelativeTo { get; set; }

        public RelativePosition(Vector2 pos, RelativePositionType relativeTo)
        {
            Position = pos;
            RelativeTo = relativeTo;
        }

        public static implicit operator RelativePosition(Vector2 vec)
        {
            return new RelativePosition(vec, RelativePositionType.TopLeft);
        }

        public Vector2 Offset(Rectangle bounds)
        {
            var offset = new Vector2(
                RelativeTo.xOffset * bounds.Width,
                RelativeTo.yOffset * bounds.Height);
            return offset;
        }

        public Vector2 Absolute(Rectangle bounds)
        {
            return Position - Offset(bounds);
        }
    }
}
