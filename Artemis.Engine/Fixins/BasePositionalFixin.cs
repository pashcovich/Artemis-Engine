
namespace Artemis.Engine.Fixins
{
    public abstract class BasePositionalFixin : AbstractGenericFixin<PositionalForm>
    {
        public BasePositionalFixin(string name) : base(name) { }

        public BasePositionalFixin(string name, PositionalForm form) : base(name, form) { }
    }
}
