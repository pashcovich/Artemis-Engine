#region Using Statements

using System;

#endregion

namespace Artemis.Engine.Graphics.LMRenderOrderActions
{
    public class ImplementedLMRenderOrderAction : AbstractLMRenderOrderAction
    {
        private Action<LayerManager> action;

        public ImplementedLMRenderOrderAction(Action<LayerManager> action)
        {
            this.action = action;
        }

        public override void Perform(LayerManager manager)
        {
            action(manager);
        }
    }
}
