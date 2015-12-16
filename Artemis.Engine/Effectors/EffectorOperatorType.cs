
namespace Artemis.Engine.Effectors
{

    /// <summary>
    /// An enum representing the different operations that can be used by an
    /// Effector to combine the previous and next returned values. It is
    /// equivalent to an operation O(x, y), where x is the previous value
    /// and y is the next value.
    /// </summary>
    public enum EffectorOperatorType
    {
        /// <summary>
        /// An in-place operation is given by O(x, y) = y. Basically this means
        /// that the next value returned by the effector will simply override the
        /// previous value before assigning it to the effected field.
        /// </summary>
        InPlace,

        /// <summary>
        /// An additive operation is given by O(x, y) = x + y. Basically this means
        /// that the next value returned by the effector will be added to the previous
        /// value returned before assigning it to the effected field.
        /// </summary>
        Additive,

        /// <summary>
        /// A multiplicative operation is given by O(x, y) = xy. This means that
        /// the next value returned by the effector will be multiplied with the
        /// previous value returned before assigning it to the effected field.
        /// </summary>
        Multiplicative,

        /// <summary>
        /// An arithmetic averaging operation is given by O(x, y) = (x + y)/2.
        /// </summary>
        ArithmeticAveraging
    }
}
