
namespace Artemis.Engine.Effectors
{
    /// <summary>
    /// A base interface used to represent an arbitrary effector with no
    /// generic parameter.
    /// </summary>
    interface IEffector
    {
        /// <summary>
        /// Update a given EffectableArtemisObject using this effector.
        /// </summary>
        /// <param name="obj"></param>
        void UpdateEffector(EffectableArtemisObject obj);
    }
}
