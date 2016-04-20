
namespace Artemis.Engine.Utilities.Dynamics
{
    /// <summary>
    /// The IDynamicProperty interface is implemented by SimpleDynamicProperty and
    /// ComplexDynamicProperty and provides simply a value that can be set and retrieved.
    /// </summary>
    internal interface IDynamicProperty
    {
        object Value { get; set; }
    }
}
