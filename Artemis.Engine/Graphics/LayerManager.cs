#region Using Statements

using Artemis.Engine.Graphics.LMRenderOrderActions;
using Artemis.Engine.Utilities.UriTree;

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Artemis.Engine.Graphics
{
    public class LayerManager : UriTreeObserver<RenderLayer>
    {

        static class DefaultRenderOrderActions
        {
            public static Action<LayerManager> Render(string name)
            {
                return lm => lm.GetObservedNode(name).Render();
            }

            public static Action<LayerManager> RenderTop(string name)
            {
                return lm => lm.GetObservedNode(name).RenderTop();
            }

            public static Action<LayerManager> SetRenderProperties(RenderPropertiesPacket packet)
            {
                return lm => ArtemisEngine.RenderPipeline.SetRenderProperties(packet);
            }

            public static Action<LayerManager> ClearRenderProperties()
            {
                return lm => ArtemisEngine.RenderPipeline.ClearRenderProperties();
            }
        }

        private List<Action<LayerManager>> RenderOrderActions;

        public LayerManager()
        {
            RenderOrderActions = new List<Action<LayerManager>>();
        }

        public void AddLayer(RenderLayer layer)
        {
            AddObservedNode(layer.tempFullName, layer);
        }

        public void SetRenderOrder(params string[] names)
        {
            var renderOrder = from name in names 
                              select DefaultRenderOrderActions.Render(name);
            RenderOrderActions = renderOrder.ToList();
        }

        public void SetRenderOrder(params Action<LayerManager>[] actions)
        {
            RenderOrderActions = actions.ToList();
        }

        public void SetRenderOrder(params object[] actions)
        {
            RenderOrderActions.Clear();

            int index = 0;
            Action<LayerManager> currentRenderAction;
            foreach (var obj in actions)
            {
                var type = obj.GetType();
                if (type == typeof(string))
                {
                    currentRenderAction = DefaultRenderOrderActions.Render((string)obj);
                }
                else if (type == typeof(RenderPropertiesPacket))
                {
                    currentRenderAction = DefaultRenderOrderActions.SetRenderProperties(
                        (RenderPropertiesPacket)obj);
                }
                else if (type == typeof(Action<LayerManager>))
                {
                    currentRenderAction = (Action<LayerManager>)obj;
                }
                else
                {
                    throw new RenderOrderException(
                        String.Format(
                            "Don't know what to do with render order action object of type '{0}' at index '{1}'. " +
                            "The supplied objects must be either a 'string', a 'RenderPropertiesPacket', or an " +
                            "'Action<LayerManager>'.", type, index
                            )
                        );
                }
                RenderOrderActions.Add(currentRenderAction);
                index++;
            }
        }

        internal void Render()
        {
            foreach (var action in RenderOrderActions)
            {
                action(this);
            }
        }
    }
}
