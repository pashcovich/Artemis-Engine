
namespace Artemis.Engine.Fixins
{
    /// <summary>
    /// A generic Fixin. The generic parameter determines the subclass of Form this
    /// Fixin type can be added. It follows that this type of fixin can also be added
    /// to subclasses of said generic parameter as well.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractGenericFixin<T> : AbstractFixin where T : Form
    {
        public T Form { get { return (T)_form; } }

        public AbstractGenericFixin(string name) : base(name) { }

        public AbstractGenericFixin(string name, T form) : base(name, form) { }
    }
}
