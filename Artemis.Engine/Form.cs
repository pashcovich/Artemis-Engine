#region Using Statements

using Artemis.Engine.Fixins;
using Artemis.Engine.Graphics;
using Artemis.Engine.Multiforms;

using System;
using System.Collections.Generic;

#endregion

namespace Artemis.Engine
{
    public delegate void FormDelegate(Form form);

    public class Form : RenderableObject, IAttachableToFixin<BaseFixin>
    {
        private bool _midUpdateFixins;
        private bool _midRenderFixins;

        private List<AbstractFixin> _fixinsToAdd = new List<AbstractFixin>();
        private List<string> _fixinsToRemove = new List<string>();

        internal FormGroup _formGroup;
        internal string _formName;

        /// <summary>
        /// The name of this form.
        /// </summary>
        public string Name;

        /// <summary>
        /// Whether or not this form has a name.
        /// </summary>
        public bool Anonymous { get { return Name != null; } }

        /// <summary>
        /// The parent of this form.
        /// </summary>
        public Multiform Parent { get; internal set; }

        /// <summary>
        /// Whether or not this form is managed by a Multiform (i.e. if Parent != null).
        /// </summary>
        public bool Managed { get { return Parent != null; } }

        /// <summary>
        /// The dictionary of fixins whose FixinType is FixinType.Render.
        /// </summary>
        public Dictionary<string, AbstractFixin> RenderFixins { get; private set; }

        /// <summary>
        /// The dictionary of fixins whose FixinType is FixinType.Update.
        /// </summary>
        public Dictionary<string, AbstractFixin> UpdateFixins { get; private set; }

        /// <summary>
        /// The delegate invoked when this form has been added to a multiform.
        /// </summary>
        public Action OnAddedToMultiform { get; protected set; }

        public Form() : this(null) { }

        public Form(string name)
            : base() 
        { 
            Name = name;
            RenderFixins = new Dictionary<string, AbstractFixin>();
            UpdateFixins = new Dictionary<string, AbstractFixin>();

            RequiredUpdater += UpdateAllFixins;
            RequiredRenderer += RenderAllFixins;
        }

        /// <summary>
        /// Attach a given fixin to this form.
        /// </summary>
        /// <param name="fixin"></param>
        public void AttachFixin(BaseFixin fixin)
        {
            AttachFixin((AbstractFixin)fixin);
        }

        /// <summary>
        /// Attach a given fixin to this form.
        /// </summary>
        /// <param name="fixin"></param>
        protected internal void AttachFixin(AbstractFixin fixin)
        {
            if (fixin == null)
                throw new ArgumentNullException(
                    String.Format("Cannot attach null fixin to form '{0}'.", Anonymous ? (object)this : Name));
            bool attached = false;
            switch (fixin.FixinType)
            {
                case FixinType.Render:
                    if (_midRenderFixins)
                        _fixinsToAdd.Add(fixin);
                    else
                    {
                        RenderFixins.Add(fixin.Name, fixin);
                        fixin._form = this;
                        attached = true;
                    }
                    break;
                case FixinType.Update:
                    if (_midRenderFixins)
                        _fixinsToAdd.Add(fixin);
                    else
                    {
                        UpdateFixins.Add(fixin.Name, fixin);
                        fixin._form = this;
                        attached = true;
                    }
                    break;
                case FixinType.Render | FixinType.Update:
                    if (_midUpdateFixins | _midRenderFixins)
                        _fixinsToAdd.Add(fixin);
                    else
                    {
                        RenderFixins.Add(fixin.Name, fixin);
                        UpdateFixins.Add(fixin.Name, fixin);
                        fixin._form = this;
                        attached = true;
                    }
                    break;
            }
            
            if (attached && fixin.OnAttachToForm != null)
            {
                fixin.OnAttachToForm();
            }
        }

        /// <summary>
        /// Detach the given fixin.
        /// </summary>
        /// <param name="fixin"></param>
        public void DetachFixin(AbstractFixin fixin)
        {
            DetachFixin(fixin, null);
        }

        /// <summary>
        /// Detach the fixin with the given name.
        /// </summary>
        /// <param name="fixinName"></param>
        public void DetachFixin(string fixinName)
        {
            DetachFixin(null, fixinName);
        }

        private void DetachFixin(AbstractFixin fixin, string fixinName)
        {
            if (fixin == null && fixinName == null)
                throw new ArgumentNullException(
                    String.Format(
                        "When detaching a fixin from form '{0}', both parameters " +
                        "('fixin' and 'fixinName') can't be null.", Anonymous ? (object)this : Name));
            if (fixin == null)
            {
                fixin = UpdateFixins.ContainsKey(fixinName) ? UpdateFixins[fixinName]
                  : RenderFixins.ContainsKey(fixinName) ? RenderFixins[fixinName]
                  : null;
                if (fixin == null)
                    return;
            }
            else if (fixinName == null)
            {
                fixinName = fixin.Name;
            }

            var type = fixin.FixinType;

            // There's probably a simpler if statement for this...
            if (type == FixinType.Update && _midUpdateFixins ||
                type == FixinType.Render && _midRenderFixins ||
                type == (FixinType.Update | FixinType.Render) && (_midRenderFixins || _midUpdateFixins))
            {
                _fixinsToRemove.Add(fixinName);
            }
            else
            {
                switch (type)
                {
                    case FixinType.Update:
                        UpdateFixins.Remove(fixinName);
                        break;
                    case FixinType.Render:
                        RenderFixins.Remove(fixinName);
                        break;
                    case FixinType.Update | FixinType.Render:
                        UpdateFixins.Remove(fixinName);
                        RenderFixins.Remove(fixinName);
                        break;
                }
                fixin.Kill(false);
            }
        }

        /// <summary>
        /// Update the form.
        /// 
        /// This will simply call all the active fixins Updaters.
        /// </summary>
        private void UpdateAllFixins()
        {
            _midUpdateFixins = true;

            foreach (var fixin in UpdateFixins.Values)
            {
                if (fixin.Active)
                    fixin.Update();
            }

            _midUpdateFixins = false;

            if (_fixinsToAdd.Count > 0)
            {
                foreach (var fixin in _fixinsToAdd)
                    AttachFixin(fixin);

                _fixinsToAdd.Clear();
            }

            AddNewFixins();
            RemoveDeadFixins();
        }

        /// <summary>
        /// Render the fixins attached to this form.
        /// </summary>
        private void RenderAllFixins()
        {
            _midRenderFixins = true;

            foreach (var fixin in RenderFixins.Values)
            {
                if (fixin.Active && fixin.Renderer != null)
                    fixin.Renderer();
            }

            _midRenderFixins = false;

            AddNewFixins();
            RemoveDeadFixins();
        }

        private void AddNewFixins()
        {
            if (_fixinsToAdd.Count > 0)
            {
                foreach (var fixin in _fixinsToAdd)
                    AttachFixin(fixin);

                _fixinsToAdd.Clear();
            }
        }

        private void RemoveDeadFixins()
        {
            if (_fixinsToRemove.Count > 0)
            {
                foreach (var fixinName in _fixinsToRemove)
                {
                    DetachFixin(null, fixinName);
                }

                _fixinsToRemove.Clear();
            }
        }

        public override void Kill()
        {
            base.Kill();

            if (Anonymous)
                _formGroup.RemoveAnonymousItem(this, false);
            else
                _formGroup.RemoveItem(_formName);

            // Calling Kill(false) will prevent attempts to remove the fixin from the dictionaries while iterating
            foreach (var fixin in UpdateFixins.Values)
                fixin.Kill(false); 
            foreach (var fixin in RenderFixins.Values)
                fixin.Kill(false);

            UpdateFixins.Clear();
            RenderFixins.Clear();
        }
    }
}
