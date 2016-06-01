﻿#region Using Statements

using Artemis.Engine.Graphics;
using Artemis.Engine.Utilities;
using Artemis.Engine.Utilities.Dynamics;

using FarseerPhysics;
using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

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
        // UNFINISHED
        /*
        private static AttributeMemoService<PhysicalForm> attrMemoService
            = new AttributeMemoService<PhysicalForm>();

        static PhysicalForm()
        {
            var emptyBodyHandler = new AttributeMemoService<PhysicalForm>
                                       .AttributeHandler(t => t.FillInBody(DefaultBodyPresets.Empty));
            attrMemoService.RegisterHandler<EmptyBodyAttribute>(emptyBodyHandler);
        }

        private enum DefaultBodyPresets { Empty }


        private void FillInBody()
        {

        }
        */

        private Body _body;

        /// <summary>
        /// The body attached to this physical object.
        /// </summary>
        public Body Body
        {
            get { return _body; }
            set
            {
                if (value == null)
                    throw new NullReferenceException(
                        string.Format("Cannot set null Body on Form '{0}'.", Anonymous ? (object)this : Name));
                _body = value;
                _body.UserData = this; // We use the UserData of a body to give a forward reference to the
                // Artemis.Engine.PhysicalForm instance it belongs to.
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
        public sealed override Vector2 TargetPosition
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

        public PhysicalForm() : base() { }

        public PhysicalForm(Body body) : this(null, body) { }
        
        public PhysicalForm(string name) : base(name) { }

        public PhysicalForm(BodyConstructor constructor)
            : this(constructor.Construct()) { }

        public PhysicalForm(string name, BodyConstructor constructor)
            : this(name, constructor.Construct()) { }

        public PhysicalForm(string name, Body body)
            : base(name)
        {
            Body = body;
        }

        public sealed override void SetPosition(Vector2 position, PositionType positionType = PositionType.TargetSpace)
        {
            if (positionType == PositionType.WorldSpace)
                WorldPosition = position;
            else
                base.SetPosition(position, positionType);
        }
    }
}