#region Using Statements

using Artemis.Engine.Utilities.UriTree;

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Artemis.Engine.Graphics
{
    public class LayerManager : UriTreeObserver<RenderLayer>
    {

        private List<AbstractRenderOrderAction> RenderOrder;

        public LayerManager()
        {
            RenderOrder = new List<AbstractRenderOrderAction>();
        }

        public void AddLayer(RenderLayer layer)
        {
            AddObservedNode(layer.tempFullName, layer);
        }

        public void SetRenderOrder(params string[] names)
        {
            var renderOrder = from name in names 
                              select (AbstractRenderOrderAction)new RenderLayerAction(name);
            RenderOrder = renderOrder.ToList();
        }

        public void SetRenderOrder(params AbstractRenderOrderAction[] actions)
        {
            RenderOrder = actions.ToList();
        }

        public void SetRenderOrder(params object[] actions)
        {
            RenderOrder.Clear();

            int index = 0;
            AbstractRenderOrderAction currentRenderAction;
            foreach (var obj in actions)
            {
                var type = obj.GetType();
                if (type == typeof(string))
                {
                    currentRenderAction = new RenderLayerAction((string)obj);
                }
                else if (type == typeof(RenderPropertiesPacket))
                {
                    currentRenderAction = new SetRenderPropertiesAction((RenderPropertiesPacket)obj);
                }
                else if (type == typeof(Action<LayerManager>))
                {
                    currentRenderAction = new RenderOrderAction((Action<LayerManager>)obj);
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
                RenderOrder.Add(currentRenderAction);
                index++;
            }
        }

        internal void Render()
        {
            foreach (var action in RenderOrder)
            {
                action.Perform(this);
            }
        }
    }
}
