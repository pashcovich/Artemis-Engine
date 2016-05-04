#region Using Statements

using Artemis.Engine.Assets;
using Artemis.Engine.Graphics;
using Artemis.Engine.Graphics.AnimationStepActions;
using Artemis.Engine.Utilities.UriTree;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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

        public XmlElement AnimationMap { get; private set; }
        public Dictionary<string, AnimationState> States { get; private set; }
        public string InitState { get; private set; }

        public AnimationMapReader(XmlElement sheet)
        {
            AnimationMap = sheet;
            States = new Dictionary<string, AnimationState>();
        }

        public Dictionary<string, AnimationState> Load()
        {
            ReadElementChildNodes(AnimationMap);
            ReadElementAttributes(AnimationMap);

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

                switch (element.Name)
                {
                    case ANIMATION_STATE:
                        AnimationStateReader stateReader = new AnimationStateReader(element);
                        stateReader.Read();
                        AnimationState state = new AnimationState(stateReader.stateName, stateReader.StepActions, AnimationStateLoopType.Cycle);
                        States.Add(stateReader.stateName, state);
                        break;

                    default:
                        break;
                }
            }
        }

        private void ReadElementAttributes(XmlElement element)
        {
            foreach (XmlAttribute attrib in element.Attributes)
            {
                switch (attrib.Name)
                {
                    case INIT_STATE:
                        InitState = attrib.Value;
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
