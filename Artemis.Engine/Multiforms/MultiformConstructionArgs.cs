#region Using Statements

using Artemis.Engine.Utilities.Dynamics;

#endregion

namespace Artemis.Engine.Multiforms
{
    public class MultiformConstructionArgs : DynamicPropertyCollection
    {
        /// <summary>
        /// The Multiform that called Activate.
        /// </summary>
        public Multiform Sender { get; private set; }

        /// <summary>
        /// The name of the activator.
        /// </summary>
        public string SenderName { get { return Sender == null ? null : Sender.Name; } }

        public MultiformConstructionArgs(Multiform sender)
        {
            Sender = sender;
        }
    }
}
