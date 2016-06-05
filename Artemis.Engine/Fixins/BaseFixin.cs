
namespace Artemis.Engine.Fixins
{
    public abstract class BaseFixin : AbstractGenericFixin<Form>
    {
        public BaseFixin(string name) : base(name) { }

        public BaseFixin(string name, Form form) : base(name, form) { }
    }
}
