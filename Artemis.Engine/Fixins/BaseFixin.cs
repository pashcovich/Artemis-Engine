
namespace Artemis.Engine.Fixins
{
    /// <summary>
    /// The base type of fixin that can be added to Forms (and subclasses).
    /// </summary>
    public abstract class BaseFixin : AbstractGenericFixin<Form>
    {
        public BaseFixin(string name) : base(name) { }

        public BaseFixin(string name, Form form) : base(name, form) { }
    }
}
