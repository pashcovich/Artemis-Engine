#region Using Statements

using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

#endregion

namespace Artemis.Engine
{
    [Serializable]
    public class BodyConstructorException : Exception
    {
        public BodyConstructorException() : base() { }
        public BodyConstructorException(string msg) : base(msg) { }
        public BodyConstructorException(string msg, Exception inner) : base(msg, inner) { }
        public BodyConstructorException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public sealed class BodyConstructor
    {
        private class FixtureData
        {
            public float? Restitution;
            public float? Friction;
            public Category? CollisionCategories;
            public Category? CollidesWith;
            public Category? IgnoreCCDWith;
            public short? CollisionGroup;
            public bool? IsSensor;
            public AfterCollisionEventHandler AfterCollision;
            public BeforeCollisionEventHandler BeforeCollision;
            public OnCollisionEventHandler OnCollision;
            public OnSeparationEventHandler OnSeparation;
            public Shape Shape;
        }

        public World World { get; private set; }
        public BodyType? BodyType { get; private set; }
        public List<Shape> Shapes { get; private set; }
        public int? IslandIndex { get; private set; }
        public float? GravityScale { get; private set; }
        public Vector2? InitialLinearVelocity { get; private set; }
        public float? InitialAngularVelocity { get; private set; }
        public float? LinearDamping { get; private set; }
        public float? AngularDamping { get; private set; }
        public bool? IsBullet { get; private set; }
        public bool? SleepingAllowed { get; private set; }
        public bool? InitiallyAwake { get; private set; }
        public bool? InitiallyEnabled { get; private set; }
        public bool? FixedRotation { get; private set; }
        public bool? IgnoreCCD { get; private set; }
        public Vector2? Position { get; private set; }
        public float? Rotation { get; private set; }
        public bool? IgnoreGravity { get; private set; }
        public Vector2? LocalCenter { get; private set; }
        public float? Mass { get; private set; }
        public float? Inertia { get; private set; }

        private FixtureData anonymousFixtureData = new FixtureData();
        private Dictionary<string, FixtureData> fixtureBuilderData
            = new Dictionary<string, FixtureData>();

        private bool? fixtureSpecificShapes;

        public BodyConstructor() : this(null) { }

        public BodyConstructor(World world)
        {
            World = world;
            Shapes = new List<Shape>();
            fixtureSpecificShapes = null;
        }

        public Body Construct()
        {
            if (World == null)
                throw new BodyConstructorException("Cannot construct body with no World object supplied.");

            var body = new Body(World, Position, Rotation.HasValue ? Rotation.Value : 0);
            if (BodyType.HasValue)
                body.BodyType = BodyType.Value;

            if (IslandIndex.HasValue)
                body.IslandIndex = IslandIndex.Value;

            if (GravityScale.HasValue)
                body.GravityScale = GravityScale.Value;

            if (InitialLinearVelocity.HasValue)
                body.LinearVelocity = InitialLinearVelocity.Value;

            if (InitialAngularVelocity.HasValue)
                body.AngularVelocity = InitialAngularVelocity.Value;

            if (LinearDamping.HasValue)
                body.LinearDamping = LinearDamping.Value;

            if (AngularDamping.HasValue)
                body.AngularDamping = AngularDamping.Value;

            if (IsBullet.HasValue)
                body.IsBullet = IsBullet.Value;

            if (SleepingAllowed.HasValue)
                body.SleepingAllowed = SleepingAllowed.Value;

            if (InitiallyAwake.HasValue)
                body.Awake = InitiallyAwake.Value;

            if (InitiallyEnabled.HasValue)
                body.Enabled = InitiallyEnabled.Value;

            if (FixedRotation.HasValue)
                body.FixedRotation = FixedRotation.Value;

            if (IgnoreCCD.HasValue)
                body.IgnoreCCD = IgnoreCCD.Value;

            if (IgnoreGravity.HasValue)
                body.IgnoreGravity = IgnoreGravity.Value;

            if (LocalCenter.HasValue)
                body.LocalCenter = LocalCenter.Value;

            if (Mass.HasValue)
                body.Mass = Mass.Value;

            if (Inertia.HasValue)
                body.Inertia = Inertia.Value;

            if (fixtureBuilderData.Count > 0)
            {
                foreach (var fixtureData in fixtureBuilderData.Values) // names don't matter other than
                                                                       // just for specifying which fixture
                                                                       // certain properties are to be applied
                                                                       // to.
                {
                    var fixture = body.CreateFixture(fixtureData.Shape);
                    SetFixtureData(fixture, fixtureData);
                }
            }
            else
            {
                foreach (var shape in Shapes)
                {
                    var fixture = body.CreateFixture(shape);
                    SetFixtureData(fixture, anonymousFixtureData);
                }
            }

            return body;
        }

        private void SetFixtureData(Fixture fixture, FixtureData fixtureData)
        {
            if (fixtureData.Restitution.HasValue)
                fixture.Restitution = anonymousFixtureData.Restitution.Value;

            if (fixtureData.Friction.HasValue)
                fixture.Friction = anonymousFixtureData.Friction.Value;

            if (fixtureData.CollisionCategories.HasValue)
                fixture.CollisionCategories = anonymousFixtureData.CollisionCategories.Value;

            if (fixtureData.CollidesWith.HasValue)
                fixture.CollidesWith = anonymousFixtureData.CollidesWith.Value;

            if (fixtureData.IgnoreCCDWith.HasValue)
                fixture.IgnoreCCDWith = anonymousFixtureData.IgnoreCCDWith.Value;

            if (fixtureData.CollisionGroup.HasValue)
                fixture.CollisionGroup = anonymousFixtureData.CollisionGroup.Value;

            if (fixtureData.IsSensor.HasValue)
                fixture.IsSensor = anonymousFixtureData.IsSensor.Value;

            if (fixtureData.AfterCollision != null)
                fixture.AfterCollision = anonymousFixtureData.AfterCollision;

            if (fixtureData.BeforeCollision != null)
                fixture.BeforeCollision = anonymousFixtureData.BeforeCollision;

            if (fixtureData.OnCollision != null)
                fixture.OnCollision = anonymousFixtureData.OnCollision;

            if (fixtureData.OnSeparation != null)
                fixture.OnSeparation = anonymousFixtureData.OnSeparation;
        }

        public BodyConstructor InWorld(World world)
        {
            World = world;
            return this;
        }

        public BodyConstructor AsBodyType(BodyType type)
        {
            BodyType = type;
            return this;
        }

        public BodyConstructor IsStatic()
        {
            BodyType = FarseerPhysics.Dynamics.BodyType.Static;
            return this;
        }

        public BodyConstructor IsKinematic()
        {
            BodyType = FarseerPhysics.Dynamics.BodyType.Kinematic;
            return this;
        }

        public BodyConstructor IsDynamic()
        {
            BodyType = FarseerPhysics.Dynamics.BodyType.Dynamic;
            return this;
        }

        public BodyConstructor WithIslandIndex(int index)
        {
            IslandIndex = index;
            return this;
        }

        public BodyConstructor WithGravityScale(float scale)
        {
            GravityScale = scale;
            return this;
        }

        public BodyConstructor WithInitialLinearVelocity(Vector2 linearVelocity)
        {
            InitialLinearVelocity = linearVelocity;
            return this;
        }

        public BodyConstructor WithInitialAngularVelocity(float angularVelocity)
        {
            InitialAngularVelocity = angularVelocity;
            return this;
        }

        public BodyConstructor WithLinearDamping(float linearDamping)
        {
            LinearDamping = linearDamping;
            return this;
        }

        public BodyConstructor WithAngularDamping(float damping)
        {
            AngularDamping = damping;
            return this;
        }

        public BodyConstructor AsBullet()
        {
            IsBullet = true;
            return this;
        }

        public BodyConstructor AllowsSleeping()
        {
            SleepingAllowed = true;
            return this;
        }

        public BodyConstructor IsInitiallyAwake()
        {
            InitiallyAwake = true;
            return this;
        }

        public BodyConstructor IsInitiallyEnabled()
        {
            InitiallyEnabled = true;
            return this;
        }

        public BodyConstructor HasFixedRotation()
        {
            FixedRotation = true;
            return this;
        }

        public BodyConstructor WithPosition(Vector2 position)
        {
            Position = position;
            return this;
        }

        public BodyConstructor WithRotation(float rotation)
        {
            Rotation = rotation;
            return this;
        }

        public BodyConstructor IgnoresGravity()
        {
            IgnoreGravity = true;
            return this;
        }

        public BodyConstructor WithLocalCenter(Vector2 pos)
        {
            LocalCenter = pos;
            return this;
        }

        public BodyConstructor WithMass(float mass)
        {
            Mass = mass;
            return this;
        }

        public BodyConstructor WithInertia(float inertia)
        {
            Inertia = inertia;
            return this;
        }

        public BodyConstructor WithShape(Shape shape)
        {
            if (fixtureSpecificShapes.HasValue && fixtureSpecificShapes.Value)
                throw new BodyConstructorException();
            Shapes.Add(shape);
            fixtureSpecificShapes = false;
            return this;
        }

        public BodyConstructor WithShape(string fixtureName, Shape shape)
        {
            if (fixtureSpecificShapes.HasValue && !fixtureSpecificShapes.Value)
                throw new BodyConstructorException();

            if (fixtureBuilderData.ContainsKey(fixtureName))
            {
                var newData = new FixtureData { Shape = shape };
                fixtureBuilderData.Add(fixtureName, newData);
            }
            else
                fixtureBuilderData[fixtureName].Shape = shape;

            fixtureSpecificShapes = true;
            return this;
        }

        public BodyConstructor WithRestitution(float restitution)
        {
            anonymousFixtureData.Restitution = restitution;
            return this;
        }

        public BodyConstructor WithRestitution(string fixtureName, float restitution)
        {
            if (fixtureBuilderData.ContainsKey(fixtureName))
            {
                var newData = new FixtureData { Restitution = restitution };
                fixtureBuilderData.Add(fixtureName, newData);
            }
            else
                fixtureBuilderData[fixtureName].Restitution = restitution;
            return this;
        }

        public BodyConstructor WithFriction(float friction)
        {
            anonymousFixtureData.Friction = friction;
            return this;
        }

        public BodyConstructor WithFriction(string fixtureName, float friction)
        {
            if (fixtureBuilderData.ContainsKey(fixtureName))
            {
                var newData = new FixtureData { Friction = friction };
                fixtureBuilderData.Add(fixtureName, newData);
            }
            else
                fixtureBuilderData[fixtureName].Friction = friction;
            return this;
        }

        public BodyConstructor WithCollisionCategories(Category category)
        {
            anonymousFixtureData.CollisionCategories = category;
            return this;
        }

        public BodyConstructor WithCollisionCategories(string fixtureName, Category category)
        {
            if (fixtureBuilderData.ContainsKey(fixtureName))
            {
                var newData = new FixtureData { CollisionCategories = category };
                fixtureBuilderData.Add(fixtureName, newData);
            }
            else
                fixtureBuilderData[fixtureName].CollisionCategories = category;
            return this;
        }

        public BodyConstructor CollidesWith(Category category)
        {
            anonymousFixtureData.CollidesWith = category;
            return this;
        }

        public BodyConstructor CollidesWith(string fixtureName, Category category)
        {
            if (fixtureBuilderData.ContainsKey(fixtureName))
            {
                var newData = new FixtureData { CollidesWith = category };
                fixtureBuilderData.Add(fixtureName, newData);
            }
            else
                fixtureBuilderData[fixtureName].CollidesWith = category;
            return this;
        }

        public BodyConstructor IgnoresCCDWith(Category category)
        {
            anonymousFixtureData.IgnoreCCDWith = category;
            return this;
        }

        public BodyConstructor IgnoresCCDWith(string fixtureName, Category category)
        {
            if (fixtureBuilderData.ContainsKey(fixtureName))
            {
                var newData = new FixtureData { IgnoreCCDWith = category };
                fixtureBuilderData.Add(fixtureName, newData);
            }
            else
                fixtureBuilderData[fixtureName].IgnoreCCDWith = category;
            return this;
        }

        public BodyConstructor WithCollisionGroup(short collisionGroup)
        {
            anonymousFixtureData.CollisionGroup = collisionGroup;
            return this;
        }

        public BodyConstructor WithCollisionGroup(string fixtureName, short collisionGroup)
        {
            if (fixtureBuilderData.ContainsKey(fixtureName))
            {
                var newData = new FixtureData { CollisionGroup = collisionGroup };
                fixtureBuilderData.Add(fixtureName, newData);
            }
            else
                fixtureBuilderData[fixtureName].CollisionGroup = collisionGroup;
            return this;
        }

        public BodyConstructor AsSensor()
        {
            anonymousFixtureData.IsSensor = true;
            return this;
        }

        public BodyConstructor AsSensor(string fixtureName)
        {
            if (fixtureBuilderData.ContainsKey(fixtureName))
            {
                var newData = new FixtureData { IsSensor = true };
                fixtureBuilderData.Add(fixtureName, newData);
            }
            else
                fixtureBuilderData[fixtureName].IsSensor = true;
            return this;
        }

        public BodyConstructor IgnoresCCD()
        {
            IgnoreCCD = true;
            return this;
        }

        public BodyConstructor WithAfterCollision(AfterCollisionEventHandler handler)
        {
            anonymousFixtureData.AfterCollision = handler;
            return this;
        }

        public BodyConstructor WithAfterCollision(string fixtureName, AfterCollisionEventHandler handler)
        {
            if (fixtureBuilderData.ContainsKey(fixtureName))
            {
                var newData = new FixtureData { AfterCollision = handler };
                fixtureBuilderData.Add(fixtureName, newData);
            }
            else
                fixtureBuilderData[fixtureName].AfterCollision = handler;
            return this;
        }

        public BodyConstructor WithBeforeCollision(BeforeCollisionEventHandler handler)
        {
            anonymousFixtureData.BeforeCollision = handler;
            return this;
        }

        public BodyConstructor WithBeforeCollision(string fixtureName, BeforeCollisionEventHandler handler)
        {
            if (fixtureBuilderData.ContainsKey(fixtureName))
            {
                var newData = new FixtureData { BeforeCollision = handler };
                fixtureBuilderData.Add(fixtureName, newData);
            }
            else
                fixtureBuilderData[fixtureName].BeforeCollision = handler;
            return this;
        }

        public BodyConstructor WithOnCollision(OnCollisionEventHandler handler)
        {
            anonymousFixtureData.OnCollision = handler;
            return this;
        }

        public BodyConstructor WithOnCollision(string fixtureName, OnCollisionEventHandler handler)
        {
            if (fixtureBuilderData.ContainsKey(fixtureName))
            {
                var newData = new FixtureData { OnCollision = handler };
                fixtureBuilderData.Add(fixtureName, newData);
            }
            else
                fixtureBuilderData[fixtureName].OnCollision = handler;
            return this;
        }

        public BodyConstructor WithOnSeparation(OnSeparationEventHandler handler)
        {
            anonymousFixtureData.OnSeparation = handler;
            return this;
        }

        public BodyConstructor WithOnSeparation(string fixtureName, OnSeparationEventHandler handler)
        {
            if (fixtureBuilderData.ContainsKey(fixtureName))
            {
                var newData = new FixtureData { OnSeparation = handler };
                fixtureBuilderData.Add(fixtureName, newData);
            }
            else
                fixtureBuilderData[fixtureName].OnSeparation = handler;
            return this;
        }
    }
}
