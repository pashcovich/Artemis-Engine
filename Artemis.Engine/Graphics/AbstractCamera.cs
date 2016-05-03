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
        public RenderLayer Layer { get; internal set; }

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
        /// The transform matrix that transforms from target coordinates to screen coordinates.
        /// 
        /// This is set by the RenderLayer this camera is attached to.
        /// </summary>
        public Matrix TargetToScreenTransform { get { return Layer._targetTransform; } }

        /// <summary>
        /// The transform matrix that transforms from screen coordinates to target coordinates.
        /// 
        /// This is the inverse of TargetToScreenTransform.
        /// </summary>
        public Matrix ScreenToTargetTransform { get { return Matrix.Invert(TargetToScreenTransform); } }

        /// <summary>
        /// The transform matrix that transforms from world coordinates to screen coordinates.
        /// </summary>
        public Matrix WorldToScreenTransform { get { return WorldToTargetTransform * TargetToScreenTransform; } }

        /// <summary>
        /// The transform matrix that transforms from screen coordinates to world coordinates.
        /// 
        /// This is the inverse of WorldToScreenTransform.
        /// </summary>
        public Matrix ScreenToWorldTransform { get { return Matrix.Invert(WorldToScreenTransform); } }

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

        /// <summary>
        /// Transform from world coordinates to screen coordinates.
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, WorldToScreenTransform);
        }

        /// <summary>
        /// Transform from screen coordinates to world coordinates.
        /// </summary>
        /// <param name="screenPosition"></param>
        /// <returns></returns>
        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, ScreenToWorldTransform);
        }

        /// <summary>
        /// Transform from target coordinates to screen coordinates.
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <returns></returns>
        public Vector2 TargetToScreen(Vector2 targetPosition)
        {
            return Vector2.Transform(targetPosition, TargetToScreenTransform);
        }

        /// <summary>
        /// Transform from screen coordinates to target coordinates.
        /// </summary>
        /// <param name="screenPosition"></param>
        /// <returns></returns>
        public Vector2 ScreenToTarget(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, ScreenToTargetTransform);
        }

        public AbstractCamera() : base() { }
    }
}
