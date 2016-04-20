
namespace Artemis.Engine.Effectors
{
    /// <summary>
    /// The base non-generic interface for an effector.
    /// </summary>
    public interface IEffector
    {
        bool Anonymous { get; }

        string EffectorName { get; set; }

        void InternalInitialize(EffectableObject obj);

        void UpdateEffector(EffectableObject obj);
    }
}
