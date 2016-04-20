
namespace Artemis.Engine.Utilities.Dynamics
{

    /// <summary>
    /// A DynamicProperty with only a value (in contrast to a ComplexDynamicProperty).
    /// </summary>
    internal class SimpleDynamicProperty : IDynamicProperty
    {
        public object Value { get; set; }

        public SimpleDynamicProperty(object value)
        {
            Value = value;
        }
    }
}
