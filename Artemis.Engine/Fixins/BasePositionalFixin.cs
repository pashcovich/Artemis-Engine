
namespace Artemis.Engine.Fixins
{
    /// <summary>
    /// The base type of fixin that can be added to PositionalForms (and subclasses).
    /// </summary>
    public abstract class BasePositionalFixin : AbstractGenericFixin<PositionalForm>
    {
        public BasePositionalFixin(string name) : base(name) { }

        public BasePositionalFixin(string name, PositionalForm form) : base(name, form) { }
    }
}
