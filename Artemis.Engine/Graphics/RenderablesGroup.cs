#region Using Statements

using Artemis.Engine.Utilities;
using Artemis.Engine.Utilities.UriTree;

#endregion

namespace Artemis.Engine.Graphics
{
    public sealed class RenderablesGroup : UriTreeMutableGroup<RenderablesGroup, IRenderable>
    {
        public RenderablesGroup(string name) : base(name) { }

        public void Render()
        {
            Render(TreeTraversalOrder.Pre);
        }

        public void Render(TreeTraversalOrder order)
        {
            switch (order)
            {
                case TreeTraversalOrder.Pre:
                    RenderSubgroups(order);
                    RenderItems(order);
                    break;
                case TreeTraversalOrder.Post:
                    RenderItems(order);
                    RenderSubgroups(order);
                    break;
                default:
                    throw new TreeTraversalOrderException(order);
            }
        }

        private void RenderSubgroups(TreeTraversalOrder order)
        {
            foreach (var subgroup in Subnodes)
            {
                subgroup.Value.Render(order);
            }
        }

        private void RenderItems(TreeTraversalOrder order)
        {
            foreach (var item in Items)
            {
                // item.Render();   
            }
        }
    }
}
