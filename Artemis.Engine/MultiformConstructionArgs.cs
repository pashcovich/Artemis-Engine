#region Using Statements

using Artemis.Engine.Utilities.Dynamics;

#endregion

namespace Artemis.Engine
{
    public class MultiformConstructionArgs : DynamicPropertyCollection
    {
        public Multiform Sender { get; private set; }

        public string SenderName { get { return Sender.Name; } }

        public MultiformConstructionArgs(Multiform sender)
        {
            Sender = sender;
        }
    }
}
