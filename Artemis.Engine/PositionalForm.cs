#region Using Statements

using Artemis.Engine.Fixins;
using Artemis.Engine.Utilities.Dynamics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

#endregion

namespace Artemis.Engine
{
    [HasDynamicProperties(new string[] { 
        "TargetPosition",
        "ScreenPosition",
        "RelativeTargetPosition"
        }, true)]
    public class PositionalForm : Form, IPositional, IAttachableToFixin<BasePositionalFixin>
    {
        /// <summary>
        /// The position on the LayerTarget.
        /// </summary>
        public virtual Vector2 TargetPosition { get;  set; }

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
                if (Layer == null)
                    TargetPosition = value;
                else
                    TargetPosition = Layer.ScreenToTarget(value);
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

        public PositionalForm() : this(null) { }

        public PositionalForm(string name) : base(name)
        {
            OnLayerTargetChanged += _UpdateScreenPosition;
        }

        public PositionalForm(Vector2 position, CoordinateSpace positionType = CoordinateSpace.TargetSpace)
            : this(null, position, positionType) { }

        public PositionalForm(
            string name, Vector2 position, CoordinateSpace positionType = CoordinateSpace.TargetSpace)
            : this(name)
        {
            SetPosition(position, positionType);
        }

        private void _UpdateScreenPosition(RenderTarget2D previousTarget
                                          , RenderTarget2D currentTarget)
        {
            if (UseTargetRelativePositioning)
            {
                // This will reset the World position to match the new Target bounds, whilst keeping the
                // RelativeTargetPosition constant (which is what we want when using TargetRelativePositioning).
                RelativeTargetPosition = _relativeTargetPosMemo;
            }
        }

        /// <summary>
        /// Get the position of this form in the given coordinate space.
        /// </summary>
        /// <param name="coordinateSpace"></param>
        /// <returns></returns>
        public virtual Vector2 GetPosition(CoordinateSpace coordinateSpace = CoordinateSpace.TargetSpace)
        {
            switch (coordinateSpace)
            {
                case CoordinateSpace.TargetSpace:
                    return TargetPosition;
                case CoordinateSpace.ScreenSpace:
                    return ScreenPosition;
                case CoordinateSpace.WorldSpace:
                    throw new FormException(
                        String.Format(
                            "Cannot get the WorldPosition on PositionalForm '{0}'.",
                            Anonymous ? (object)this : Name));
                default:
                    throw new FormException(String.Format("Unknown CoordinateSpace '{0}'.", coordinateSpace));
            }
        }

        /// <summary>
        /// Set the position of this form.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="coordinateSpace"></param>
        public virtual void SetPosition(Vector2 position, CoordinateSpace coordinateSpace = CoordinateSpace.TargetSpace)
        {
            switch (coordinateSpace)
            {
                case CoordinateSpace.TargetSpace:
                    TargetPosition = position;
                    break;
                case CoordinateSpace.ScreenSpace:
                    ScreenPosition = position;
                    break;
                case CoordinateSpace.WorldSpace:
                    throw new FormException(
                        String.Format(
                            "Cannot set the WorldPosition on PositionalForm '{0}'.",
                            Anonymous ? (object)this : Name));
                default:
                    throw new FormException(String.Format("Unknown CoordinateSpace '{0}'.", coordinateSpace));
            }
        }

        /// <summary>
        /// Attach a given fixin to this form.
        /// </summary>
        /// <param name="fixin"></param>
        public void AttachFixin(BasePositionalFixin fixin)
        {
            AttachFixin((AbstractFixin)fixin);
        }
    }
}
