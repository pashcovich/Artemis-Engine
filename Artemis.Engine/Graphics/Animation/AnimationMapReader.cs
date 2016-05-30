#region Using Statements

using System.Collections.Generic;
using System.Xml;

#endregion

namespace Artemis.Engine.Graphics.Animation
{
    internal class AnimationMapReader
    {
        #region Xml Constants

        // Xml Tags
        public const string ANIMATION_STATE = "AnimationState";

        // Xml Attribute Names
        public const string INIT_STATE = "InitState";

        #endregion

        public XmlElement AnimationMapElement { get; private set; }
        public Dictionary<string, AnimationState> States { get; private set; }
        public string InitState { get; private set; }

        public AnimationMapReader(XmlElement element)
        {
            AnimationMapElement = element;
            States = new Dictionary<string, AnimationState>();
        }

        public Dictionary<string, AnimationState> Load()
        {
            ReadElementChildNodes(AnimationMapElement);
            ReadElementAttributes(AnimationMapElement);

            return States;
        }

        private void ReadElementChildNodes(XmlElement animationMapElement)
        {
            foreach (var node in animationMapElement.ChildNodes)
            {
                var element = node as XmlElement;

                if (element == null)
                {
                    continue;
                }

                if (element.Name == ANIMATION_STATE)
                {
                    AnimationStateReader stateReader = new AnimationStateReader(element);
                    stateReader.Read();

                    AnimationState state = new AnimationState(
                        stateReader.StateName, 
                        stateReader.StepActions, 
                        AnimationStateLoopType.Cycle);
                    States.Add(stateReader.StateName, state);
                }
            }
        }

        private void ReadElementAttributes(XmlElement element)
        {
            foreach (XmlAttribute attrib in element.Attributes)
            {
                if (attrib.Name == INIT_STATE)
                    InitState = attrib.Value + ".";
            }
        }
    }
}
