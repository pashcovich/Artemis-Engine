
namespace Artemis.Engine.Fixins
{
    /// <summary>
    /// The base type of fixin that can be added to PhysicalForms (and subclasses).
    /// </summary>
    public abstract class BasePhysicalFixin : AbstractGenericFixin<PhysicalForm>
    {
        public BasePhysicalFixin(string name) : base(name) { }

        public BasePhysicalFixin(string name, PhysicalForm form) : base(name, form){}
    }
}
