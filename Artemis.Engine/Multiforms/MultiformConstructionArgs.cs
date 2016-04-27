#region Using Statements

using Artemis.Engine.Utilities.Dynamics;

#endregion

namespace Artemis.Engine.Multiforms
{
    public class MultiformConstructionArgs : DynamicPropertyCollection
    {
        public Multiform Sender { get; private set; }

        public string SenderName { get { return Sender == null ? null : Sender.Name; } }

        public MultiformConstructionArgs(Multiform sender)
        {
            Sender = sender;
        }
    }
}
