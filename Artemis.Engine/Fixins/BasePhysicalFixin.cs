
namespace Artemis.Engine.Fixins
{
    public abstract class BasePhysicalFixin : AbstractGenericFixin<PhysicalForm>
    {
        public BasePhysicalFixin(string name) : base(name) { }

        public BasePhysicalFixin(string name, PhysicalForm form) : base(name, form){}
    }
}
