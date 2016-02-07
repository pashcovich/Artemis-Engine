#region Using Statements

using Artemis.Engine.Utilities;
using Artemis.Engine.Utilities.UriTree;

using Microsoft.Xna.Framework.Graphics;

using System;

#endregion

namespace Artemis.Engine.Graphics
{
    public abstract class AbstractLayer : UriTreeObserverNode<AbstractLayer, RenderablesGroup>
    {
        // The name of the top level RenderablesGroup.
        private const string TOP_LEVEL = "ALL";

        /// <summary>
        /// The top level observed RenderablesGroup.
        /// </summary>
        protected RenderablesGroup AllSprites;

        protected RenderTarget2D target;

        // We have to store a temporary full name until the render layer get's
        // added to a LayerManager, then it's actual FullName property gets
        // set.
        internal string tempFullName;

        public AbstractLayer(string fullName)
            : base(UriUtilities.GetLastPart(fullName))
        {
            tempFullName = fullName;

            AllSprites = new RenderablesGroup(TOP_LEVEL);
            AddObservedNode(TOP_LEVEL, AllSprites);

            target = ArtemisEngine.RenderPipeline.CreateRenderTarget();
        }

        /// <summary>
        /// Add an IRenderable item to be rendered on this layer.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        public void Add(string name, IRenderable item)
        {
            AllSprites.InsertItem(name, item);
        }

        /// <summary>
        /// Add an anonymous IRenderable item to be rendered on this layer.
        /// </summary>
        /// <param name="item"></param>
        public void AddAnonymous(IRenderable item)
        {
            AllSprites.AddAnonymousItem(item);
        }

        /// <summary>
        /// Add an anoymous IRenderable to the subgroup of IRenderables on this
        /// layer with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        public void AddAnonymous(string name, IRenderable item)
        {
            AllSprites.InsertAnonymousItem(name, item);
        }

        /// <summary>
        /// Function called before rendering.
        /// </summary>
        public Action PreRender { get; set; }

        /// <summary>
        /// Function called after rendering.
        /// </summary>
        public Action PostRender { get; set; }

        /// <summary>
        /// Render this layer and every sublayer as well.
        /// </summary>
        public abstract void Render();

        /// <summary>
        /// Render a single IRenderable item.
        /// </summary>
        /// <param name="item"></param>
        public abstract void RenderIRenderable(IRenderable item);
    }
}
