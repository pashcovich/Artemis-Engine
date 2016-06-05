#region Using Statements

using Microsoft.Xna.Framework;

#endregion

namespace Artemis.Engine
{
    public interface IPositional
    {
        void SetPosition(Vector2 position, CoordinateSpace coordinateSpace = CoordinateSpace.TargetSpace);

        Vector2 GetPosition(CoordinateSpace coordinateSpace = CoordinateSpace.TargetSpace);
    }
}
