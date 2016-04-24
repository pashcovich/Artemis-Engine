
namespace Artemis.Engine.Utilities.Dynamics
{

    /// <summary>
    /// A DynamicProperty with getter and setter.
    /// </summary>
    internal class ComplexDynamicProperty : IDynamicProperty
    {

        private Getter getter { get; set; }
        private Setter setter { get; set; }

        public object Value
        {
            get
            {
                if (getter != null)
                    return getter();
                throw new DynamicPropertyException();
            }
            set
            {
                if (setter != null)
                    setter(value);
                else
                    throw new DynamicPropertyException();
            }
        }

        public ComplexDynamicProperty(
            Getter getter, Setter setter, bool useInitialValue = false, object initialValue = null)
        {
            this.getter = getter;
            this.setter = setter;

            if (useInitialValue)
            {
                setter(initialValue);
            }
        }
    }
}
