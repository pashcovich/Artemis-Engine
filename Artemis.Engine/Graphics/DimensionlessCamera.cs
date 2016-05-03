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
    [ResolutionChangeListener]
    public class DimensionlessCamera : AbstractCamera
    {
        private Vector2 _position;
        private Vector2 _zoom;
        private float _rotation;
        private bool _requiresRecalc;
        private AABB _aabbMemo;
        private Matrix _w2tTransformMemo;
        
        /// <summary>
        /// The AABB that contains everything in this camera's FOV.
        /// </summary>
        public override AABB ViewAABB
        {
            get
            {
                if (_requiresRecalc)
                {
                    Resolution res;
                    if (Layer.LayerScaleType == GlobalLayerScaleType.Uniform)
                        res = GameConstants.BaseResolution;
                    else
                        res = ArtemisEngine.DisplayManager.WindowResolution;

                    var inverseViewMatrix = TargetToWorldTransform;
                    var tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
                    var tr = Vector2.Transform(new Vector2(res.Width, 0), inverseViewMatrix);
                    var bl = Vector2.Transform(new Vector2(0, res.Height), inverseViewMatrix);
                    var br = Vector2.Transform(new Vector2(res.Width, res.Height), inverseViewMatrix);

                    var min = new Vector2(
                        MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
                        MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
                    var max = new Vector2(
                        MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
                        MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
                    _aabbMemo = new AABB(new Vector2(min.X, min.Y), new Vector2(max.X - min.X, max.Y - min.Y));
                    _requiresRecalc = false;
                }
                return _aabbMemo;
            }
        }

        /// <summary>
        /// The transform matrix that transforms from world coordinates to target coordinates.
        /// </summary>
        public override Matrix WorldToTargetTransform
        {
            get 
            {
                if (_requiresRecalc)
                {
                    Resolution res;
                    if (Layer.LayerScaleType == GlobalLayerScaleType.Uniform)
                        res = GameConstants.BaseResolution;
                    else
                        res = ArtemisEngine.DisplayManager.WindowResolution;
                    float w = res.Width, h = res.Height;
                    var m = Matrix.CreateScale(ConvertUnits.SimToDisplayRatio, ConvertUnits.SimToDisplayRatio, 1f) *
                            Matrix.CreateTranslation(new Vector3(-w / 2, -h / 2, 0)) *
                            Matrix.CreateScale(_zoom.X, _zoom.Y, 1f) * Matrix.CreateRotationZ(_rotation) *
                            Matrix.CreateTranslation(new Vector3(w / 2, h / 2, 0)) *
                            Matrix.CreateTranslation(new Vector3(ConvertUnits.ToDisplayUnits(-_position.X, -_position.Y), 0));
                    _w2tTransformMemo = m;
                    _requiresRecalc = false;
                }
                return _w2tTransformMemo;
            }
        }
        
        /// <summary>
        /// The world coordinates of the top left corner of this camera.
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                _requiresRecalc = true;
            }
        }

        /// <summary>
        /// The Zoom factor of the FOV.
        /// </summary>
        public Vector2 Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value;
                _requiresRecalc = true;
            }
        }

        /// <summary>
        /// The rotation of the camera about it's center (clockwise).
        /// </summary>
        public float Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                _requiresRecalc = true;
            }
        }

        private void _onResChanged(
            Resolution previousResolution,
            Resolution currentResolution,
            Vector2 baseScaleFactors
            )
        {
            _requiresRecalc = true;
        }

        public DimensionlessCamera()
            : this(Vector2.Zero, Vector2.One, 0f) { }

        public DimensionlessCamera(Vector2 position)
            : this(position, Vector2.One, 0f) { }

        public DimensionlessCamera(Vector2 position, float rotation)
            : this(position, Vector2.One, rotation) { }

        public DimensionlessCamera(Vector2 position, Vector2 zoom)
            : this(position, zoom, 0f) { }

        public DimensionlessCamera(
            Vector2 position, Vector2 zoom, float rotation)
            : base()
        {
            Position = position;
            Zoom = zoom;
            Rotation = rotation;
            OnResolutionChanged += new ResolutionChangedDelegate(_onResChanged);
        }
        
    }
}
