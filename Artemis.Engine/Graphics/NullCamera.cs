#region Using Statements

using FarseerPhysics;
using FarseerPhysics.Collision;

using Microsoft.Xna.Framework;

#endregion

namespace Artemis.Engine.Graphics
{
    /// <summary>
    /// The camera used when no
    /// </summary>
    public sealed class NullCamera : AbstractCamera
    {
        /// <summary>
        /// The AABB that contains everything in this camera's FOV.
        /// 
        /// This is never actually used by the engine, since a NullCamera just indicates for a
        /// RenderLayer to render everything in the world. This it doesn't really matter what the
        /// value of the AABB is.
        /// </summary>
        public override AABB ViewAABB
        {
            get
            {
                return new AABB(Vector2.Zero, Vector2.Zero);
            }
        }

        /// <summary>
        /// The transform matrix that transforms from world coordinates to target coordinates.
        /// </summary>
        public override Matrix WorldToTargetTransform
        {
            get
            {
                return Matrix.CreateScale(ConvertUnits.SimToDisplayRatio, ConvertUnits.SimToDisplayRatio, 1f);
            }
        }
    }
}
