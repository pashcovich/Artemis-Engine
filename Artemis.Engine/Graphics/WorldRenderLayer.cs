#region Using Statements

using Artemis.Engine.Maths.Geometry;
using Artemis.Engine.Utilities.UriTree;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Artemis.Engine.Graphics
{
    public class WorldRenderLayer : ResolutionRelativeRenderLayer
    {
        private RenderPipeline rp; // the global render pipeline,
        private World _world;
        private AbstractCamera _camera;
        private Predicate<RenderableObject> isVisibleToCameraPredicate;

        /// <summary>
        /// The camera attached to this layer.
        /// </summary>
        public AbstractCamera Camera
        {
            get { return _camera; }
            set
            {
                if (MidRender)
                    throw new CameraException(
                        String.Format(
                            "Cannot set Camera on render layer with name '{0}' " +
                            "until after the render cycle is complete.", tempFullName
                            )
                        );
                if (value == null)
                    value = new NullCamera();
                _camera = value;
                _camera.Layer = this;
            }
        }

        /// <summary>
        /// The world this layer is attached to.
        /// </summary>
        public World World
        {
            get { return _world; }
        }

        public WorldRenderLayer(string fullName)
            : this(fullName, new NullCamera()) { }

        public WorldRenderLayer(string fullName, AbstractCamera camera)
            : this(fullName, camera, GlobalLayerScaleType.Dynamic, UniformLayerScaleType.Stretch, null) { }

        public WorldRenderLayer(string fullName, AbstractCamera camera, World world)
            : this(fullName, camera, GlobalLayerScaleType.Dynamic, UniformLayerScaleType.Stretch, world) { }

        public WorldRenderLayer( string fullName
                          , AbstractCamera camera
                          , GlobalLayerScaleType layerScaleType    = GlobalLayerScaleType.Dynamic
                          , UniformLayerScaleType uniformScaleType = UniformLayerScaleType.Stretch
                          , World world = null )
            : base(UriUtilities.GetLastPart(fullName), layerScaleType, uniformScaleType)
        {
            rp = ArtemisEngine.RenderPipeline; // for convenience

            Camera = camera;
            camera.Layer = this;

            _world = world;
        }

        /// <summary>
        /// Attach this layer to a world.
        /// </summary>
        /// <param name="world"></param>
        public void AttachToWorld(World world)
        {
            _world = world;
        }

        /// <summary>
        /// Get the RenderAction. The Layer RenderAction determines how it prepares and calls
        /// each RenderableObject's Render method it encounters.
        /// 
        /// For RenderLayer, the render action returned depends on the LayerScaleType.
        /// </summary>
        /// <returns></returns>
        protected override RenderableHandler GetRenderableHandler()
        {
            switch (LayerScaleType)
            {
                case GlobalLayerScaleType.Uniform:
                    if (isVisibleToCameraPredicate == null)
                        return obj => obj.InternalRender(SeenRenderables);
                    else
                        return obj =>
                        {
                            if (isVisibleToCameraPredicate(obj))
                                obj.InternalRender(SeenRenderables);
                        };
                case GlobalLayerScaleType.Dynamic:
                    var isBaseRes = ArtemisEngine.DisplayManager.IsBaseResolution;
                    var crntRes   = ArtemisEngine.DisplayManager.WindowResolution;
                    var resScale  = ArtemisEngine.DisplayManager.ResolutionScale;

                    if (isVisibleToCameraPredicate == null)
                        return obj => ProcessDynamicallyScaledRenderable(obj, isBaseRes, crntRes, resScale);
                    else
                        return obj =>
                        {
                            if (isVisibleToCameraPredicate(obj))
                                ProcessDynamicallyScaledRenderable(obj, isBaseRes, crntRes, resScale);
                        };
                default:
                    throw new RenderLayerException(
                        String.Format(
                            "Unknown GlobalLayerScaleType '{0}' supplied to layer '{1}'.",
                            LayerScaleType, tempFullName));
            }
        }

        /// <summary>
        /// Get the list of all RenderableObjects visible to the Camera.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<RenderableObject> GetCameraVisibleRenderables()
        {
            if (Camera is NullCamera)
                return AllRenderables.RetrieveAll();

            var renderables = new HashSet<RenderableObject>();
            var aabb = Camera.ViewAABB;
            _world.QueryAABB(f =>
            {
                var obj = (RenderableObject)f.Body.UserData;
                renderables.Add(obj); // A renderable object might have multiple fixtures, but
                // we only want to render the object itself once.
                return true;
            }, ref aabb);

            return renderables;
        }

        protected override void PreRender()
        {
            base.PreRender();

            // Get all the renderable objects.
            var renderables = GetCameraVisibleRenderables();

            // Create the predicate that checks if a given renderable is visible.
            isVisibleToCameraPredicate = null; // if null, then every renderable is visible.
            if (!(Camera is NullCamera))
            {
                var hashSet = (HashSet<RenderableObject>)renderables;
                isVisibleToCameraPredicate = obj => hashSet.Contains(obj);
            }
        }

        /// <summary>
        /// Called directly after "SetupLayerTarget" and directly before any rendering.
        /// </summary>
        protected override void PostSetupLayerTarget()
        {
            rp.SetRenderProperties(m: Camera.WorldToTargetTransform);
            rp.LockMatrix();
        }
    }
}
