#region Using Statements

using Artemis.Engine.Utilities;
using Artemis.Engine.Utilities.UriTree;

using System;

#endregion

namespace Artemis.Engine.Graphics
{
    public class RenderLayer : UriTreeObserverNode<RenderLayer, RenderablesGroup>
    {

        // The name of the top level RenderablesGroup.
        private const string TOP_LEVEL = "ALL";

        // We have to store a temporary full name until the render layer get's
        // added to a LayerManager, then it's actual FullName property gets
        // set.
        internal string tempFullName;

        RenderablesGroup AllSprites;

        public RenderLayer(string fullName)
            : base(UriUtilities.GetLastPart(fullName))
        {
            tempFullName = fullName;

            AllSprites = new RenderablesGroup(TOP_LEVEL);
            AddObservedNode(TOP_LEVEL, AllSprites);
        }

        public void Add(string name, IRenderable item)
        {
            AllSprites.InsertItem(name, item);
        }

        public void AddAnonymous(IRenderable item)
        {
            AllSprites.AddAnonymousItem(item);
        }

        public void AddAnonymous(string name, IRenderable item)
        {
            AllSprites.InsertAnonymousItem(name, item);
        }

        /// <summary>
        /// Render this layer and every sublayer as well.
        /// 
        /// This is equivalent to calling Render(TreeTraversalOrder.Pre).
        /// </summary>
        public void Render()
        {
            Render(TreeTraversalOrder.Pre);
        }

        /// <summary>
        /// Render this layer and all the sublayers in the given order.
        /// If the order is TreeTraversalOrder.Pre, then the sublayers will be
        /// rendered first, then the IRenderables. This is reversed when the order
        /// is TreeTraversalOrder.Post.
        /// </summary>
        /// <param name="order"></param>
        public void Render(TreeTraversalOrder order)
        {
            switch (order)
            {
                case TreeTraversalOrder.Pre:
                    RenderSublayers(order);
                    RenderRenderables(order);
                    break;
                case TreeTraversalOrder.Post:
                    RenderRenderables(order);
                    RenderSublayers(order);
                    break;
                default:
                    throw new TreeTraversalOrderException(order);
            }
        }

        private void RenderSublayers(TreeTraversalOrder order)
        {
            foreach (var subLayer in Subnodes)
            {
                subLayer.Value.Render(order);
            }
        }

        private void RenderRenderables(TreeTraversalOrder order)
        {
            AllSprites.Render(order);
        }

        public void RenderTop()
        {
            AllSprites.Render();
        }
    }
}
