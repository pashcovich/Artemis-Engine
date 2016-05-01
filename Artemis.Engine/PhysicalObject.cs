#region Using Statements

using Artemis.Engine.Graphics;
using Artemis.Engine.Utilities.Dynamics;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

#endregion

namespace Artemis.Engine
{
    [HasDynamicProperties]
    public class PhysicalObject : LayerAwareObject
    {
        public Body Body;

        /// <summary>
        /// The position of the object in the world.
        ///
        /// If this value is set, it is expected that the given value is in simulation units
        /// and not display units. Use ConvertUnits.ToSimUnits if not.
        /// </summary>
        [DynamicProperty]
        public Vector2 WorldPosition { get { return Body.Position; } set { Body.Position = value; } }

        [DynamicProperty]
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

        [DynamicProperty]
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

        public PhysicalObject() : base() { }
    }
}
