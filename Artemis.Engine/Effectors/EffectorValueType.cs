
namespace Artemis.Engine.Effectors
{

    /// <summary>
    /// An enum representing how the effector will behave with respect
    /// to the initial value of the effected field.
    /// </summary>
    public enum EffectorValueType
    {
        /// <summary>
        /// Absolute indicates that the values returned by the effector will
        /// simply override the initial value of the effected field.
        /// </summary>
        Absolute,

        /// <summary>
        /// RelativeToStart indicates that the values returned by the effector will
        /// be relative to the initial value of the effected field. For example, if
        /// the EffectorOperatorType is Additive, and if x0 is the initial value of
        /// the effected field, then the next value returned by the effector will be
        /// added to x0, and the next value added to that, etc.
        /// 
        /// NOTE: If an effector's operator is InPlace, then RelativeToStart will
        /// cause the initial value of the effected field to be added to the value
        /// returned by the effector.
        /// </summary>
        RelativeToStart
    }
}
