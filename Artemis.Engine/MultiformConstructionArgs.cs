
namespace Artemis.Engine
{
    public class MultiformConstructionArgs : DynamicFieldContainer
    {
        public Multiform Sender { get; private set; }

        public string SenderName { get { return Sender.Name; } }

        public MultiformConstructionArgs(Multiform sender)
        {
            Sender = sender;
        }
    }
}
