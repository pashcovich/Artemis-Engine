#region Using Statements

using Artemis.Engine.Graphics;
using Artemis.Engine.Utilities.Dynamics;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Artemis.Engine
{
    [HasDynamicProperties(new string[] {"WorldPosition", "TargetPosition", "ScreenPosition"}, true)]
    public abstract class PhysicalObject : RenderableObject
    {
        private Body _body;

        /// <summary>
        /// The body attached to this physical object.
        /// </summary>
        public Body Body
        {
            get { return _body; }
            set
            {
                _body = value;
                _body.UserData = this; // We use the UserData of a body to give a forward reference to the
                                       // Artemis Engine PhysicalObject instance it belongs to. This is used
                                       // specifically in RenderLayer to retrieve the RenderableObjects from
                                       // fixtures retrieved by an AABB query of the world. (Michael, 5/15/2016)
            }
        }

        private AbstractCamera _nullCamera = new NullCamera();
        private bool _useNullCamera;

        /// <summary>
        /// The Camera this physical object is being viewed by.
        /// </summary>
        public AbstractCamera Camera
        {
            get
            {
                if (!_useNullCamera)
                {
                    var layer = Layer as RenderLayer;
                    if (layer == null)
                    {
                        _useNullCamera = true;
                        return _nullCamera;
                    }
                    return layer.Camera;
                }
                else
                {
                    return _nullCamera;
                }
            }
        }

        /// <summary>
        /// The position of the object in the world.
        ///
        /// If this value is set, it is expected that the given value is in simulation units
        /// and not display units. Use ConvertUnits.ToSimUnits if not.
        /// </summary>
        public Vector2 WorldPosition { get { return Body.Position; } set { Body.Position = value; } }

        /// <summary>
        /// The position on the LayerTarget.
        /// </summary>
        public Vector2 TargetPosition
        {
            get
            {
                // Camera is never null, instead there's a NullCamera that's automatically supplied.
                // Also, Camera.WorldToTarget converts units.
                return Camera.WorldToTarget(Body.Position);
            }
            set
            {
                Body.Position = Camera.TargetToWorld(value);
            }
        }

        /// <summary>
        /// The position on the screen.
        /// 
        /// You should almost always use TargetPosition if you want to position an object
        /// relative to the display instead of ScreenPosition.
        /// </summary>
        public Vector2 ScreenPosition
        {
            get
            {
                return Camera.WorldToScreen(Body.Position);
            }
            set
            {
                Body.Position = Camera.ScreenToWorld(value);
            }
        }

        private Vector2 _relativeTargetPosMemo;
        /// <summary>
        /// The position on the LayerTarget as a relative coordinate (i.e. mapped so
        /// that (0, 0) is the top left of the target and (1, 1) is the bottom right).
        /// 
        /// NOTE: When `UseTargetRelativePositioning` is true, this value is held constant
        /// when the resolution changes (meaning the World position changes).
        /// </summary>
        public Vector2 RelativeTargetPosition
        {
            get
            {
                var bounds = Layer.TargetBounds;
                var targetPosition = TargetPosition;
                return new Vector2(targetPosition.X / bounds.X, targetPosition.Y / bounds.Y);
            }
            set
            {
                var bounds = Layer.TargetBounds;
                TargetPosition = new Vector2(value.X * bounds.X, value.Y * bounds.Y);
                _relativeTargetPosMemo = value;
            }
        }

        public PhysicalObject() : base() 
        {
            OnLayerTargetChanged += _UpdateScreenPosition;
        }

        private void _UpdateScreenPosition( RenderTarget2D previousTarget
                                          , RenderTarget2D currentTarget )
        {
            if (UseTargetRelativePositioning)
            {
                // This will reset the World position to match the new Target bounds, whilst keeping the
                // RelativeTargetPosition constant (which is what we want when using TargetRelativePositioning).
                RelativeTargetPosition = _relativeTargetPosMemo;
            }
        }
    }
}
