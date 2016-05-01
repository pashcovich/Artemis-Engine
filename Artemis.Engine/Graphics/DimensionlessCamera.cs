#region Using Statements

using FarseerPhysics;
using FarseerPhysics.Collision;
using Microsoft.Xna.Framework;

#endregion

namespace Artemis.Engine.Graphics
{

    /// <summary>
    /// A camera with no dimensions (just shows all objects in a rectangle equal to the
    /// size of the render layer's target, or the window resolution).
    /// </summary>
    public class DimensionlessCamera : AbstractCamera
    {
        public override AABB ViewAABB
        {
            get
            {
                var _screenSize = ArtemisEngine.DisplayManager.WindowResolution;
                var inverseViewMatrix = TargetToWorldTransform;
                var tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
                var tr = Vector2.Transform(new Vector2(_screenSize.Width, 0), inverseViewMatrix);
                var bl = Vector2.Transform(new Vector2(0, _screenSize.Height), inverseViewMatrix);
                var br = Vector2.Transform(new Vector2(_screenSize.Width, _screenSize.Height), inverseViewMatrix);
                var min = new Vector2(
                    MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
                    MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
                var max = new Vector2(
                    MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
                    MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
                return new AABB(new Vector2(min.X, min.Y), new Vector2(max.X - min.X, max.Y - min.Y));
            }
        }

        public override Matrix WorldToTargetTransform
        {
            get 
            {
                var res = ArtemisEngine.DisplayManager.WindowResolution;
                float w = res.Width, h = res.Height;
                return Matrix.CreateScale(ConvertUnits.SimToDisplayRatio, ConvertUnits.SimToDisplayRatio, 1f) *
                       Matrix.CreateTranslation(new Vector3(-w / 2, -h / 2, 0)) *
                       Matrix.CreateScale(Zoom.X, Zoom.Y, 1f) * Matrix.CreateRotationZ(Rotation) *
                       Matrix.CreateTranslation(new Vector3(w / 2, h / 2, 0)) *
                       Matrix.CreateTranslation(new Vector3(ConvertUnits.ToDisplayUnits(-Location.X, -Location.Y), 0));
            }
        }

        public Vector2 Zoom { get; set; }
        // World coordinates
        public Vector2 Location { get; set; }
        public float Rotation { get; set; }
    }
}
