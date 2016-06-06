#region Using Statements

using Artemis.Engine.Input;
using Artemis.Engine.Utilities;

using FarseerPhysics;
using FarseerPhysics.Dynamics;

using System.Linq;

#endregion

namespace Artemis.Engine.Forms.Menu
{
    public delegate void OnMouseEnterDelegate();
    public delegate void OnMouseLeaveDelegate();
    public delegate void OnMouseHoverDelegate();
    public delegate void OnClickedDelegate(MouseButton button);
    public delegate void OnHeldDelegate(MouseButton button);
    public delegate void OnReleasedDelegate(MouseButton button);
    public delegate void OnFocusGainedDelegate();
    public delegate void OnFocusLostDelegate();

    public class BaseMenuForm : PhysicalForm
    {
        /// <summary>
        /// Whether or not this form can be focused by a MenuNavigator.
        /// </summary>
        public bool Focusable;

        /// <summary>
        /// Whether or not the mouse is colliding with this form.
        /// </summary>
        public bool MouseColliding { get; private set; }

        /// <summary>
        /// The number of frames the mouse has been hovering on this form for.
        /// </summary>
        public int FramesMouseHovered { get; private set; }

        /// <summary>
        /// The number of milliseconds the mouse has been hovering on this form for.
        /// </summary>
        public double TimeMouseHovered { get; private set; }

        /// <summary>
        /// Delegate invoked when the user's mouse enters this form.
        /// </summary>
        public OnMouseEnterDelegate OnMouseEnter;

        /// <summary>
        /// Delegate invoked when the user's mouse leaves this form.
        /// </summary>
        public OnMouseLeaveDelegate OnMouseLeave;

        /// <summary>
        /// Delegate invoked while the user's mouse is in this form.
        /// 
        /// This is called the frame *after* OnMouseEnter is invoked.
        /// </summary>
        public OnMouseHoverDelegate OnMouseHover;

        /// <summary>
        /// Delegate invoked when the user has clicked this form.
        /// </summary>
        public OnClickedDelegate OnClicked;

        /// <summary>
        /// Delegate invoked while the user is clicking this form.
        /// </summary>
        public OnHeldDelegate OnHeld;

        /// <summary>
        /// Delegate invoked when the user has stopped clicking this form.
        /// </summary>
        public OnReleasedDelegate OnReleased;

        /// <summary>
        /// Delegate invoked when this form has gained focus by a MenuNavigator.
        /// 
        /// This is only invoked if Focusable is true.
        /// </summary>
        public OnFocusGainedDelegate OnFocusGained; // CURRENTLY NOT USED

        /// <summary>
        /// Delegate invoked when this form has lost focus.
        /// 
        /// This is only invoked if Focusable is true.
        /// </summary>
        public OnFocusLostDelegate OnFocusLost; // CURRENTLY NOT USED

        /// <summary>
        /// The number of frames each mouse button has been pressing this form for.
        /// </summary>
        private int[] framesMouseuttonPressed = { 0, 0, 0 };

        internal BaseMenuForm() : base() { }

        internal BaseMenuForm(Body body) : this(null, body) { }

        internal BaseMenuForm(string name) : base(name) { }

        internal BaseMenuForm(BodyConstructor constructor)
            : this(constructor.Construct()) { }

        internal BaseMenuForm(string name, BodyConstructor constructor)
            : this(name, constructor.Construct()) { }

        internal BaseMenuForm(string name, Body body)
            : base(name)
        {
            Body = body;

            RequiredUpdater += MainUpdate;
        }

        private void MainUpdate()
        {
            if (Body == null)
                return;
            var mousePos = ConvertUnits.ToSimUnits(ArtemisEngine.Mouse.PositionVector);
            if (Body.CollidingWithMouse())
            {
                if (MouseColliding)
                {
                    if (OnMouseHover != null)
                        OnMouseHover();      
                }
                else
                {
                    if (OnMouseEnter != null)
                        OnMouseEnter();
                    MouseColliding = true;
                }

                foreach (var button in MouseInput.MouseButtons)
                {
                    var buttonID = (int)button;
                    var frames = ArtemisEngine.Mouse.FramesSinceButtonPressed[buttonID];
                    if (frames == 1)
                    {
                        framesMouseuttonPressed[buttonID] = 1;
                        if (OnClicked != null)
                            OnClicked(button);
                    }
                    else if (frames > 1)
                    {
                        if (framesMouseuttonPressed[buttonID] > 0)
                        {
                            framesMouseuttonPressed[buttonID]++;
                            if (OnHeld != null)
                                OnHeld(button);
                        }
                    }
                    else
                    {
                        if (framesMouseuttonPressed[buttonID] > 0)
                        {
                            if (OnReleased != null)
                                OnReleased(button);
                            framesMouseuttonPressed[buttonID] = 0;
                        }
                    }
                }

                FramesMouseHovered++;
                TimeMouseHovered += ArtemisEngine.GameTimer.DeltaTime;
            }
            else
            {
                if (MouseColliding)
                {
                    MouseColliding = false;
#if C_SHARP_6
                    OnMouseLeave?.Invoke();
#else
                    if (OnMouseLeave != null)
                        OnMouseLeave();
#endif              
                    FramesMouseHovered = 0;
                    TimeMouseHovered = 0;
                }
            }
        }
    }
}
