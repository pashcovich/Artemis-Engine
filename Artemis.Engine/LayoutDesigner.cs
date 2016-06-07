#region Using Statements

using Microsoft.Xna.Framework;

#endregion

namespace Artemis.Engine
{

    public abstract class AbstractLayoutSpacingProvider
    {
        /// <summary>
        /// Gets the position of the object with the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="totalItems"></param>
        /// <returns></returns>
        public abstract Vector2 GetPositionOf(int index, int totalItems, Vector2 initialPosition);
    }

    public static class LayoutSpacing
    {
        public class Uniform : AbstractLayoutSpacingProvider
        {
            private float _spacing;
            public float Spacing
            {
                get { return _spacing; }
                set
                {
                    _spacing = value;
                    Delta = _spacing * _direction;
                }
            }

            private Vector2 _direction;
            public Vector2 Direction
            {
                get { return _direction; }
                set
                {
                    _direction = value;
                    Delta = _spacing * _direction;
                }
            }

            public Vector2 Delta { get; private set; }

            public Uniform(float spacing, Vector2 direction)
            {
                _spacing = spacing;
                _direction = direction;
                Delta = _spacing * _direction;
            }

            public override Vector2 GetPositionOf(int index, int totalItems, Vector2 initialPosition)
            {
                return Delta*index + initialPosition;
            }
        }
    }

    public static class LayoutDesigner
    {
        /// <summary>
        /// Options for vertical spacing directions using the LayoutDesigner.
        /// </summary>
        public enum VerticalSpacingDirection
        {
            Up,
            Down
        }

        /// <summary>
        /// Options for horizontal spacing directions using the LayoutDesigner.
        /// </summary>
        public enum HorizontalSpacingDirection
        {
            Left,
            Right
        }

        /// <summary>
        /// Space the given objects vertically, starting at the given initial position and
        /// with the given amount of space between each object.
        /// </summary>
        /// <param name="spacing"></param>
        /// <param name="initialPosition"></param>
        /// <param name="direction"></param>
        /// <param name="coordinateSpace"></param>
        /// <param name="positionals"></param>
        public static void SpaceVertically( float spacing
                                          , Vector2 initialPosition
                                          , VerticalSpacingDirection direction = VerticalSpacingDirection.Down
                                          , CoordinateSpace coordinateSpace = CoordinateSpace.TargetSpace
                                          , params IPositional[] positionals )
        {
            // NOTE: Should this really be "world down" (i.e. decreasing the y coordinates), which is
            // visually moving up, or "visual down" (i.e. increasing the y coordinate)?
            Vector2 dir = new Vector2(0, 1) * (direction == VerticalSpacingDirection.Down ? 1 : -1);
            SpaceInDirection(spacing, initialPosition, dir, coordinateSpace, positionals);    
        }

        /// <summary>
        /// Space the given objects horizontally, starting at the given initial position and
        /// with the given amount of space between each object.
        /// </summary>
        /// <param name="spacing"></param>
        /// <param name="initialPosition"></param>
        /// <param name="direction"></param>
        /// <param name="coordinateSpace"></param>
        /// <param name="positionals"></param>
        public static void SpaceHorizontally( float spacing
                                            , Vector2 initialPosition
                                            , HorizontalSpacingDirection direction = HorizontalSpacingDirection.Left
                                            , CoordinateSpace coordinateSpace = CoordinateSpace.TargetSpace
                                            , params IPositional[] positionals )
        {
            Vector2 dir = new Vector2(1, 0) * (direction == HorizontalSpacingDirection.Right ? 1 : -1);
            SpaceInDirection(spacing, initialPosition, dir, coordinateSpace, positionals);
        }

        /// <summary>
        /// Space the given objects in the given direction, starting at the given initial position
        /// and with the given amount of space between each object.
        /// </summary>
        /// <param name="spacing"></param>
        /// <param name="initialPosition"></param>
        /// <param name="direction"></param>
        /// <param name="coordinateSpace"></param>
        /// <param name="positionals"></param>
        public static void SpaceInDirection( float spacing
                                           , Vector2 initialPosition
                                           , Vector2 direction
                                           , CoordinateSpace coordinateSpace = CoordinateSpace.TargetSpace
                                           , params IPositional[] positionals )
        {
            var delta = spacing * direction;
            var position = initialPosition;
            foreach (var positional in positionals)
            {
                positional.SetPosition(position, coordinateSpace);
                position += delta;
            }
        }

        /// <summary>
        /// Space the given objects using the given AbstractLayoutSpacingProvider and the initial position.
        /// </summary>
        /// <param name="spacer"></param>
        /// <param name="initialPosition"></param>
        /// <param name="coordinateSpace"></param>
        /// <param name="positionals"></param>
        public static void Space( AbstractLayoutSpacingProvider spacer
                                , Vector2 initialPosition
                                , CoordinateSpace coordinateSpace = CoordinateSpace.TargetSpace
                                , params IPositional[] positionals )
        {
            var index = 0;
            var total = positionals.Length;
            foreach (var positional in positionals)
            {
                var position = spacer.GetPositionOf(index, total, initialPosition);
                positional.SetPosition(position, coordinateSpace);
                index++;
            }
        }
    }
}
