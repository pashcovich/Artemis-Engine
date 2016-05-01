#region Using Statements

using FarseerPhysics;
using FarseerPhysics.Collision;

using Microsoft.Xna.Framework;

#endregion

namespace Artemis.Engine.Graphics
{
    public sealed class NullCamera : AbstractCamera
    {
        public override AABB ViewAABB
        {
            get
            {
                return new AABB(Vector2.Zero, Vector2.Zero);
            }
        }

        public override Matrix WorldToTargetTransform
        {
            get
            {
                return Matrix.CreateScale(ConvertUnits.SimToDisplayRatio, ConvertUnits.SimToDisplayRatio, 1f);
            }
        }
    }
}
