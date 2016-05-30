#region Using Statements

using FarseerPhysics.Collision;

using Microsoft.Xna.Framework;

#endregion

namespace Artemis.Engine.Graphics
{

    public abstract class AbstractCamera : ResolutionRelativeObject
    {
        /// <summary>
        /// The layer this camera is attached to.
        /// </summary>
        public ResolutionRelativeRenderLayer Layer { get; internal set; }

        /// <summary>
        /// The AABB that contains everything in this camera's FOV.
        /// </summary>
        public abstract AABB ViewAABB { get; }

        /// <summary>
        /// The transform matrix that transforms from world coordinates to target coordinates.
        /// 
        /// This must be implemented for every other transform to make sense.
        /// </summary>
        public abstract Matrix WorldToTargetTransform { get; }

        /// <summary>
        /// The transform matrix that transforms from target coordinates to world coordinates.
        /// 
        /// This is the inverse of WorldToTargetTransform.
        /// </summary>
        public Matrix TargetToWorldTransform { get { return Matrix.Invert(WorldToTargetTransform); } }

        /// <summary>
        /// Transform from world coordinates to target coordinates.
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public Vector2 WorldToTarget(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, WorldToTargetTransform);
        }

        /// <summary>
        /// Transform from target coordinates to world coordinates.
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <returns></returns>
        public Vector2 TargetToWorld(Vector2 targetPosition)
        {
            return Vector2.Transform(targetPosition, TargetToWorldTransform);
        }

        public AbstractCamera() : base() { }
    }
}
