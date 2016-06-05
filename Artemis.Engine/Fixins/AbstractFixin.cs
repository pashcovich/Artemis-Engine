#region Using Statements

using System;

#endregion

namespace Artemis.Engine
{
    [Flags]
    public enum FixinType
    {
        None = 0,
        Update = 1 << 0,
        Render = 1 << 1
    }

    [ManualUpdate]
    public abstract class AbstractFixin : ArtemisObject
    {
        /// <summary>
        /// The fixin type. This determines how the Form it's attached to handles it.
        /// </summary>
        public abstract FixinType FixinType { get; }

        /// <summary>
        /// The form this fixin is attached to.
        /// </summary>
        internal Form _form;

        /// <summary>
        /// Whether or not this fixin is attached to a form.
        /// </summary>
        public bool Attached { get { return _form != null; } }

        /// <summary>
        /// Whether or not this fixin is active (i.e. effects the form it's attached to).
        /// </summary>
        public bool Active;

        /// <summary>
        /// The name of this fixin (also the key of this fixin in the
        /// attached form's specific fixins dictionary).
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Delegate fired after this fixin has been attached to a form.
        /// </summary>
        public Action OnAttachToForm { get; protected set; }

        public AbstractFixin(string name) : this(name, null) { }

        public AbstractFixin(string name, Form form)
            : base()
        {
            Name = name;
            if (form != null)
            {
                form.AttachFixin(this);
            }
        }

        #region Directly Copied from RenderableObject

        /// <summary>
        /// The renderer action.
        /// </summary>
        internal Renderer Renderer;

        private Renderer _requiredRenderer;
        protected Renderer RequiredRenderer
        {
            get { return _requiredRenderer; }
            set
            {
                _requiredRenderer = value;
                Renderer = value;
            }
        }

        /// <summary>
        /// Set the renderer for this object.
        /// </summary>
        /// <param name="renderer"></param>
        public void SetRenderer(Renderer renderer)
        {
            Renderer = null;
            Renderer += RequiredRenderer;
            Renderer += renderer;
        }

        /// <summary>
        /// Add a renderer to this object.
        /// </summary>
        /// <param name="renderer"></param>
        public void AddRenderer(Renderer renderer)
        {
            Renderer += renderer;
        }

        /// <summary>
        /// Remove a renderer from this object.
        /// </summary>
        /// <param name="renderer"></param>
        public void RemoveRenderer(Renderer renderer)
        {
            Renderer -= renderer;
        }

        /// <summary>
        /// Remove all renderers from this object.
        /// </summary>
        public void ClearRenderer()
        {
            Renderer = null;
            Renderer += RequiredRenderer;
        }

        #endregion

        internal void Kill(bool detach)
        {
            if (detach)
            {
                _form.DetachFixin(Name);
                _form = null;
            }

            Kill();
        }
    }
}
