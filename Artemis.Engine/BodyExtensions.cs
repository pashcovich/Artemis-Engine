#region Using Statements

using Artemis.Engine.Graphics;

using FarseerPhysics;
using FarseerPhysics.Dynamics;

using System.Linq;

#endregion

namespace Artemis.Engine
{
    public static class BodyExtensions
    {
        /// <summary>
        /// Check if this body collides with the mouse.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static bool CollidingWithMouse(this Body @this)
        {
            var pos = ConvertUnits.ToSimUnits(ArtemisEngine.Mouse.PositionVector);
            return @this.FixtureList.Any(f => f.TestPoint(ref pos));
        }

        public static bool CollidingWithMouse(this Body @this, AbstractRenderLayer layer)
        {
            var pos = ConvertUnits.ToSimUnits(layer.ScreenToTarget(ArtemisEngine.Mouse.PositionVector));
            return @this.FixtureList.Any(f => f.TestPoint(ref pos));
        }

        public static bool CollidingWithMouse(this Body @this, WorldRenderLayer layer)
        {
            var pos = layer.Camera.TargetToWorld(layer.ScreenToTarget(ArtemisEngine.Mouse.PositionVector));
            return @this.FixtureList.Any(f => f.TestPoint(ref pos));
        }
    }
}
