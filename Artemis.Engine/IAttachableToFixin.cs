
namespace Artemis.Engine
{
    public interface IAttachableToFixin<T> where T : AbstractFixin
    {
        void AttachFixin(T fixin);
    }
}
