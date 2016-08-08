#region Using Statements

using Artemis.Engine.Utilities;
using Artemis.Engine.Utilities.UriTree;

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Artemis.Engine.Multiforms
{

    public enum FormNameType
    {
        Named,
        Anonymous,
        Both
    }

    public sealed class FormGroup : UriTreeMutableGroup<FormGroup, Form>
    {
        public FormGroup(string name) : base(name)
        {
            OnItemAdded += OnFormAdded;
            OnItemRemoved += OnFormRemoved;
        } 

        private void OnFormAdded(string name, Form form)
        {
            form._formGroup = this;
            form._formName = name;
        }

        private void OnFormRemoved(string name, Form form)
        {
            form._formGroup = null;
            form._formName = null;
        }

        /// <summary>
        /// Update all the Forms in this form group and it's subgroups, in the order 
        /// specified by the TraversalOptions.
        /// 
        /// You can specify to render only named forms, only anonymous forms, or both 
        /// using the FormType parameter.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="formType"></param>
        public void Update(TraversalOptions order = TraversalOptions.Pre, FormNameType formType = FormNameType.Both)
        {
            switch (order)
            {
                case TraversalOptions.Pre:
                    UpdateSubgroups(order, formType);
                    UpdateTop(formType);
                    break;
                case TraversalOptions.Post:
                    UpdateTop(formType);
                    UpdateSubgroups(order, formType);
                    break;
                case TraversalOptions.Top:
                    UpdateTop(formType);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Update the subgroups of this form group.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="formType"></param>
        public void UpdateSubgroups(TraversalOptions order = TraversalOptions.Pre, FormNameType formType = FormNameType.Both)
        {
            foreach (var subnode in Subnodes.Values)
            {
                subnode.Update(order, formType);
            }
        }

        /// <summary>
        /// Update the forms in this form group.
        /// </summary>
        /// <param name="formType"></param>
        public void UpdateTop(FormNameType formType = FormNameType.Both)
        {
            switch (formType)
            {
                case FormNameType.Named:
                    foreach (var item in Items.Values)
                    {
                        item.Update();
                    }
                    break;
                case FormNameType.Anonymous:
                    foreach (var item in AnonymousItems)
                    {
                        item.Update();
                    }
                    break;
                case FormNameType.Both:
                    foreach (var item in Items.Values)
                    {
                        item.Update();
                    }
                    foreach (var item in AnonymousItems)
                    {
                        item.Update();
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// A Multiform represents a specific part of a game with a specific
    /// update loop and a specific render loop.
    /// </summary>
    [ManualUpdate] // Multiform updating is handled by the MultiformManager
    public abstract class Multiform : ArtemisObject
    {

        private static AttributeMemoService<Multiform> attrMemoService
            = new AttributeMemoService<Multiform>();

        static Multiform()
        {
            attrMemoService.RegisterHandler<ReconstructMultiformAttribute>(m => { m.reconstructable = true; });
        }

        private const string TOP_FORM_GROUP_NAME = "ALL"; // The name of _allForms.
        private FormGroup _allForms; // The root FormGroup.
        private bool reconstructable; // Whether or not the multiform uses reconstruction upon multiple activation.

        /// <summary>
        /// The name of the multiform instance.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The MultiformManager this multiform is registered to.
        /// </summary>
        public MultiformManager Manager { get; private set; }

        /// <summary>
        /// Whether or not this multiform has been registered to a multiform manager.
        /// </summary>
        public bool Registered { get { return Manager != null; } }

        /// <summary>
        /// The number of times this multiform has been activated.
        /// </summary>
        public int TimesActivated { get; private set; }

        /// <summary>
        /// The transition constraints on this multiform.
        /// </summary>
        public TransitionConstraintsAttribute TransitionConstraints { get; private set; }

        public Multiform() : this(null) { }

        public Multiform(string name)
        {
            if (name == null)
            {
                var type = GetType();
                Name = Reflection.HasAttribute<NamedMultiformAttribute>(type)
                    ? Reflection.GetFirstAttribute<NamedMultiformAttribute>(type).Name
                    : type.Name;
            }
            else
            {
                Name = name;
            }

            attrMemoService.Handle(this);

            _allForms = new FormGroup(TOP_FORM_GROUP_NAME);
        }

        private void HandleTransitionConstraints()
        {
            TransitionConstraints = Reflection.GetFirstAttribute<TransitionConstraintsAttribute>(GetType());
        }

        /// <summary>
        /// Called after the multiform is registered to a manager.
        /// </summary>
        /// <param name="manager"></param>
        internal void PostRegister(MultiformManager manager)
        {
            Manager = manager;
        }

        internal void InternalConstruct(MultiformConstructionArgs args)
        {
            TimesActivated++;
            if (reconstructable && TimesActivated > 1)
            {
                Reconstruct(args);
            }
            else
            {
                Construct(args);
            }
        }

        /// <summary>
        /// The main constructor for the multiform. This is called every time this multiform
        /// instance is switched to by the MultiformManager.
        /// </summary>
        public abstract void Construct(MultiformConstructionArgs args);

        /// <summary>
        /// The auxiliary constructor called every time after the first time the multiform is
        /// activated. This is only used if the multiform is decorated with a ReconstructMultiform
        /// attribute.
        /// </summary>
        public virtual void Reconstruct(MultiformConstructionArgs args) { }

        /// <summary>
        /// The deconstructor for the multiform. This is called when the multiform is deactivated (after
        /// Deactivate is called).
        /// </summary>
        public virtual void Deconstruct() { }

        /// <summary>
        /// Deactivate this multiform.
        /// </summary>
        public void Deactivate()
        {
            Manager.Deactivate(this);
        }

        /// <summary>
        /// Add a form to this multiform.
        /// </summary>
        /// <param name="form"></param>
        public void AddForm(Form form, bool disallowDuplicates = true)
        {
            if (form.Name == null)
            {
                _allForms.AddAnonymousItem(form);
            }
            else
            {
                _allForms.InsertItem(form.Name, form, disallowDuplicates);
            }
            form.Parent = this;
            if (form.OnAddedToMultiform != null)
            {
                form.OnAddedToMultiform();
            }
        }

        /// <summary>
        /// Add an anonymous form to the group with the given name.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="form"></param>
        public void AddAnonymousForm(string groupName, Form form)
        {
            _allForms.InsertAnonymousItem(groupName, form);
        }

        /// <summary>
        /// Add the given forms to this multiform.
        /// </summary>
        /// <param name="disallowDuplicates"></param>
        /// <param name="forms"></param>
        public void AddForms(bool disallowDuplicates = true, params Form[] forms)
        {
            AddForms(disallowDuplicates, forms);
        }

        /// <summary>
        /// Add the given forms to this multiform.
        /// </summary>
        /// <param name="forms"></param>
        public void AddForms(IEnumerable<Form> forms, bool disallowDuplicates = true)
        {
            foreach (var form in forms)
                AddForm(form, disallowDuplicates);
        }

        /// <summary>
        /// Add the given forms anonymously to the group with the given name.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="forms"></param>
        public void AddAnonymousForms(string groupName, params Form[] forms)
        {
            AddAnonymousForms(groupName, forms);
        }

        /// <summary>
        /// Add the given forms anonymously to the group with the given name.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="forms"></param>
        public void AddAnonymousForms(string groupName, IEnumerable<Form> forms)
        {
            foreach (var form in forms)
            {
                AddAnonymousForm(groupName, form);
            }
        }

        /// <summary>
        /// Get the form with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Form GetForm(string name)
        {
            return _allForms.GetItem(name);
        }

        public Form GetForm<T>(string name) where T : Form
        {
            return (T)_allForms.GetItem(name);
        }

        /// <summary>
        /// Get the anonymous forms from the group with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<Form> GetAnonymousForms(string name)
        {
            return _allForms.GetSubnode(name).AnonymousItems;
        }

        public IEnumerable<T> GetAnonymousForms<T>(string name) where T : Form
        {
            return _allForms.GetSubnode(name).AnonymousItems.Cast<T>();
        }

        /// <summary>
        /// Get the forms with the given names.
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public IEnumerable<Form> GetForms(params string[] names)
        {
            return GetForms(names);
        }

        public IEnumerable<T> GetForms<T>(params string[] names) where T : Form
        {
            return GetForms<T>(names);
        }

        /// <summary>
        /// Get the forms with the given names.
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public IEnumerable<Form> GetForms(IEnumerable<string> names)
        {
            return from name in names select _allForms.GetItem(name);
        }

        public IEnumerable<T> GetForms<T>(IEnumerable<string> names) where T : Form
        {
            return from name in names select (T)_allForms.GetItem(name);
        }

        /// <summary>
        /// Remove the form with the given name.
        /// </summary>
        /// <param name="name"></param>
        public void RemoveForm(string name)
        {
            _allForms.RemoveItem(name);
        }

        /// <summary>
        /// Remove the given form.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="searchRecursive"></param>
        public void RemoveForm(Form form, bool searchRecursive = true)
        {
            if (form.Anonymous)
                RemoveAnonymousForm(form, searchRecursive);
            else
                RemoveForm(form.Name);
        }

        /// <summary>
        /// Remove the given anonymous form.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="searchRecursive"></param>
        public void RemoveAnonymousForm(Form form, bool searchRecursive = true)
        {
            _allForms.RemoveAnonymousItem(form, searchRecursive);
        }

        /// <summary>
        /// Remove the anonymous form in the group with the given name.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="form"></param>
        public void RemoveAnonymousForm(string groupName, Form form)
        {
            _allForms.RemoveAnonymousItem(groupName, form);
        }

        /// <summary>
        /// Remove the forms with the given names.
        /// </summary>
        /// <param name="names"></param>
        public void RemoveForms(params string[] names)
        {
            RemoveForms(names);
        }

        /// <summary>
        /// Remove the forms with the given names.
        /// </summary>
        /// <param name="names"></param>
        public void RemoveForms(IEnumerable<string> names)
        {
            foreach (var name in names)
                RemoveForm(name);
        }

        /// <summary>
        /// Remove the given forms.
        /// </summary>
        /// <param name="searchRecursive"></param>
        /// <param name="forms"></param>
        public void RemoveForms(bool searchRecursive = true, params Form[] forms)
        {
            RemoveForms(forms, searchRecursive);
        }

        /// <summary>
        /// Remove the given forms.
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="searchRecursive"></param>
        public void RemoveForms(IEnumerable<Form> forms, bool searchRecursive = true)
        {
            foreach (var form in forms)
                RemoveForm(form, searchRecursive);
        }

        /// <summary>
        /// Remove the given anonymous forms.
        /// </summary>
        /// <param name="searchRecursive"></param>
        /// <param name="forms"></param>
        public void RemoveAnonymousForms(bool searchRecursive = true, params Form[] forms)
        {
            RemoveAnonymousForms(forms, searchRecursive);
        }

        /// <summary>
        /// Remove the given anonymous forms.
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="searchRecursive"></param>
        public void RemoveAnonymousForms(IEnumerable<Form> forms, bool searchRecursive = true)
        {
            foreach (var form in forms)
                RemoveAnonymousForm(form, searchRecursive);
        }

        /// <summary>
        /// Remove all the forms.
        /// </summary>
        /// <param name="recursive"></param>
        public void ClearForms(bool recursive = false)
        {
            _allForms.ClearItems(recursive);
        }

        /// <summary>
        /// Remove all the forms in the given group.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="recursive"></param>
        public void ClearForms(string groupName, bool recursive = false)
        {
            _allForms.GetSubnode(groupName).ClearItems(recursive);
        }

        /// <summary>
        /// Remove all the named forms (leaving only the anonymous ones).
        /// </summary>
        /// <param name="recursive"></param>
        public void ClearNamedForms(bool recursive = false)
        {
            _allForms.ClearNamedItems(recursive);
        }

        /// <summary>
        /// Remove all the named forms that match the given regex.
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="recursive"></param>
        public void ClearNamedForms(string regex, bool recursive = false)
        {
            _allForms.ClearNamedItems(regex, recursive);
        }

        /// <summary>
        /// Remove all the named forms in the given group that match the given regex.
        /// 
        /// NOTE: The `regex` parameter can be null.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="regex"></param>
        /// <param name="recursive"></param>
        public void ClearNamedForms(string groupName, string regex, bool recursive = false)
        {
            var subnode = _allForms.GetSubnode(groupName);
            if (regex == null)
                subnode.ClearNamedItems(recursive);
            else
                subnode.ClearNamedItems(regex, recursive);
        }

        /// <summary>
        /// Remove all the anonymous forms (leaving only the named ones).
        /// </summary>
        /// <param name="recursive"></param>
        public void ClearAnonymousForms(bool recursive = false)
        {
            _allForms.ClearAnonymousItems(recursive);
        }

        /// <summary>
        /// Remove all the anonymous forms from the group with the given name (leaving only the named ones).
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="recursive"></param>
        public void ClearAnonymousForms(string groupName, bool recursive = false)
        {
            _allForms.GetSubnode(groupName).ClearAnonymousItems(recursive);
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

        public void UpdateForms(TraversalOptions order = TraversalOptions.Pre)
        {
            _allForms.Update(order);
        }

        public void UdpateForms(string groupName, TraversalOptions order = TraversalOptions.Pre)
        {
            _allForms.GetSubnode(groupName).Update(order);
        }

        public void UpdateForm(string name)
        {
            _allForms.GetItem(name).Update();
        }

        public void UpdateForms(params string[] names)
        {
            UpdateForms(names);
        }

        public void UpdateForms(IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                _allForms.GetItem(name).Update();
            }
        }

        #endregion

        internal void Render()
        {
            Renderer();
        }
    }
}
