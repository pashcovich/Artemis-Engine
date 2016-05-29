#region Using Statements

using Artemis.Engine.Graphics;
using Artemis.Engine.Utilities.Dynamics;

using FarseerPhysics;
using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Artemis.Engine
{
    [HasDynamicProperties(new string[] {
        "WorldPosition", 
        "TargetPosition", 
        "RelativeTargetPosition", 
        "ScreenPosition" 
    }, true)]
    public class PhysicalForm : PositionalForm
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
                // Artemis.Engine.PhysicalObject instance it belongs to. This is used
                // specifically in RenderLayer to retrieve the RenderableObjects from
                // fixtures retrieved by an AABB query of the world. (Michael, 5/15/2016)
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
                var cam = Camera;
                if (cam == null)
                    return ConvertUnits.ToDisplayUnits(Body.Position);
                return cam.WorldToTarget(Body.Position);
            }
            set
            {
                var cam = Camera;
                if (cam == null)
                    Body.Position = ConvertUnits.ToSimUnits(value);
                else
                    Body.Position = cam.TargetToWorld(value);
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
                if (Layer == null)
                    return TargetPosition;
                return Layer.TargetToScreen(TargetPosition);
            }
            set
            {
                var cam = Camera;
                if (cam == null)
                    Body.Position = ConvertUnits.ToSimUnits(value);
                else
                {
                    if (Layer == null)
                        Body.Position = cam.TargetToWorld(value);
                    else
                        Body.Position = cam.TargetToWorld(Layer.ScreenToTarget(value));
                }
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
                Rectangle bounds;
                if (Layer == null)
                    bounds = (Rectangle)ArtemisEngine.DisplayManager.WindowResolution;
                else
                    bounds = Layer.TargetBounds;
                var targetPosition = TargetPosition;
                return new Vector2(targetPosition.X / bounds.X, targetPosition.Y / bounds.Y);
            }
            set
            {
                Rectangle bounds;
                if (Layer == null)
                    bounds = (Rectangle)ArtemisEngine.DisplayManager.WindowResolution;
                else
                    bounds = Layer.TargetBounds;
                
                TargetPosition = new Vector2(value.X * bounds.X, value.Y * bounds.Y);
                _relativeTargetPosMemo = value;
            }
        }

        public PhysicalForm(string name) : base(name) 
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
