
namespace Artemis.Engine.Fixins
{
    public abstract class AbstractGenericFixin<T> : AbstractFixin where T : Form
    {
        public T Form { get { return (T)_form; } }

        public AbstractGenericFixin(string name) : base(name) { }

        public AbstractGenericFixin(string name, T form) : base(name, form) { }
    }
}
