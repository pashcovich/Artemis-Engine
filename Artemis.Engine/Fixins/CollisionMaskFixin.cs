#region Using Statements

using Artemis.Engine.Forms;

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Factories;

using System;

#endregion

namespace Artemis.Engine.Fixins
{
    public class CollisionMaskFixin :  BasePhysicalFixin
    {
        public override FixinType FixinType { get { return FixinType.Update; } }

        private Shape _shape;
        /// <summary>
        /// The shape of this collision mask.
        /// </summary>
        public Shape Shape 
        { 
            get 
            { 
                return _shape; 
            }
            set
            {
                _shape = value;
                AttachFixture();
            }
        }

        public CollisionMaskFixin(string name, Shape shape)
            : this(name, null, shape) { }

        public CollisionMaskFixin(string name, PhysicalForm form, Shape shape)
            : base(name, form)
        {
            _shape = shape;
            OnAttachToForm += AttachFixture;
        }

        private void AttachFixture()
        {
            if (Attached)
            {
                Form.Body.CreateFixture(_shape);
            }
        }
    }
}
