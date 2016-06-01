#region Using Statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Artemis.Engine
{
    public class PositionalForm : Form
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

        public PositionalForm(Vector2 position, PositionType positionType = PositionType.TargetSpace)
            : this(null, position, positionType) { }

        public PositionalForm(
            string name, Vector2 position, PositionType positionType = PositionType.TargetSpace)
            : this(name)
        {
            SetPosition(position, positionType);
        }

        public virtual void SetPosition(Vector2 position, PositionType positionType = PositionType.TargetSpace)
        {
            switch (positionType)
            {
                case PositionType.TargetSpace:
                    TargetPosition = position;
                    break;
                case PositionType.ScreenSpace:
                    ScreenPosition = position;
                    break;
                case PositionType.WorldSpace:
                    throw new FormException(
                        String.Format(
                            "Cannot set the WorldPosition on PositionalForm '{0}'.",
                            Anonymous ? (object)this : Name));
                default:
                    throw new FormException(String.Format("Unknown PositionType '{0}'.", positionType));
            }
        }

        private void _UpdateScreenPosition( RenderTarget2D previousTarget
                                          , RenderTarget2D currentTarget)
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
