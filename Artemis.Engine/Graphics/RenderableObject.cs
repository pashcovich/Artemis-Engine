#region Using Statements

using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

#endregion

namespace Artemis.Engine.Graphics
{

    /// <summary>
    /// A delegate fired when the LayerTarget of a Layer changes.
    /// </summary>
    /// <param name="previousTarget"></param>
    /// <param name="currentTarget"></param>
    public delegate void LayerTargetChangedDelegate(RenderTarget2D previousTarget, RenderTarget2D currentTarget);

    public class RenderableObject : ResolutionRelativeObject
    {

        /// <summary>
        /// The Layer this object belongs to.
        /// </summary>
        public AbstractRenderLayer Layer { get; internal set; }

        /// <summary>
        /// The Camera this physical object is being viewed by.
        /// </summary>
        public AbstractCamera Camera
        {
            get
            {
                if (Layer == null)
                    return null;
                var worldLayer = Layer as WorldRenderLayer;
                return worldLayer == null ? null : worldLayer.Camera;
            }
        }

        /// <summary>
        /// Whether or not this object is visible.
        /// </summary>
        public bool Visible;

        /// <summary>
        /// Whether or not this object can be safely rendered multiple times in a single game tick.
        /// </summary>
        public bool DisallowMultipleRenders;

        /// <summary>
        /// The components that specify how this object is to be rendered.
        /// </summary>
        public SpriteProperties SpriteProperties;

        /// <summary>
        /// The event fired when the target of the layer is changed.
        /// </summary>
        public LayerTargetChangedDelegate OnLayerTargetChanged;

        // Info for killing the object.
        internal RenderableGroup _renderableGroup;
        internal string _renderableName;

        /// <summary>
        /// The renderer action.
        /// </summary>
        private Renderer Renderer;

        private Renderer _requiredRenderer;
        protected internal Renderer RequiredRenderer
        {
            get { return _requiredRenderer; }
            set
            {
                _requiredRenderer = value;
                Renderer = value;
            }
        }

        /// <summary>
        /// An abstract method for rendering this object.
        /// </summary>
        public void Render()
        {
            if (Renderer != null)
                Renderer();
        }

        
        /// <summary>
        /// Set the renderer for this object.
        /// </summary>
        /// <param name="renderer"></param>
        public void SetRenderer(Renderer renderer)
        {
            Renderer = null;
            Renderer += RequiredRenderer;
            Renderer += renderer;
        }

        /// <summary>
        /// Add a renderer to this object.
        /// </summary>
        /// <param name="renderer"></param>
        public void AddRenderer(Renderer renderer)
        {
            Renderer += renderer;
        }

        /// <summary>
        /// Remove a renderer from this object.
        /// </summary>
        /// <param name="renderer"></param>
        public void RemoveRenderer(Renderer renderer)
        {
            Renderer -= renderer;
        }

        /// <summary>
        /// Remove all renderers from this object.
        /// </summary>
        public void ClearRenderer()
        {
            Renderer = null;
            Renderer += RequiredRenderer;
        }

        internal void InternalRender(HashSet<RenderableObject> seenObjects)
        {
            if (Visible)
            {
                if (DisallowMultipleRenders && seenObjects.Contains(this))
                {
                    throw new RenderOrderException(
                        String.Format(
                            "Renderable object '{0}' was rendered multiple times. If this is desired behaviour, " +
                            "change the object's `DisallowMultipleRenders` property to false.", this));
                }
                Render();
                seenObjects.Add(this);
            }
        }

        public RenderableObject() : base() { }

        public override void Kill()
        {
            base.Kill();
            if (Layer != null)
            {
                Layer.targetChangeListeners.Remove(this);
                if (_renderableGroup == null)
                    return;
                if (_renderableName == null)
                    _renderableGroup.RemoveAnonymousItem(this, false);
                else
                    _renderableGroup.RemoveItem(_renderableName);
            }
        }
    }
}
