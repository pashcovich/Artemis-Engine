#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

using Artemis.Engine.Assets;
using Artemis.Engine.Graphics.AnimationStepActions;
using Artemis.Engine.Utilities.UriTree;

#endregion

namespace Artemis.Engine.Graphics.Animation
{
    internal class AnimationStateReader
    {
        #region Xml Constants

        // Xml Tags
        public const string FRAMES       = "Frames";
        public const string STEP_ACTIONS = "StepActions";

        // Xml Attributes
        public const string NAME     = "Name";
        public const string DURATION = "Duration";
        
        // StepAction Inner Tags
        public const string FRAME         = "Frame";
        public const string WAIT          = "Wait";
        public const string SOUND         = "Sound";
        public const string CALL_FUNCTION = "CallFunction";
        public const string REVERSE       = "Reverse";
        public const string REPEAT        = "Repeat";

        // Optional Inner Tags
        public const string FRAME_DURATION = "FrameDuration";
        public const string LOOP_TYPE      = "LoopType";
        public const string LOOP_COUNT     = "LoopCount";

        // Timing Inner Tags
        public const string WHEN_FINISHED = "WhenFinished";
        public const string WHEN_AFTER    = "WhenAfter";

        // Inner Timing Tags
        public const string ENTER   = "Enter";
        public const string JUMP_TO = "JumpTo";

        // Repeat Attributes
        public const string START = "Start";
        public const string END   = "End";
        public const string STEP  = "Step";

        // Xml Inner Text Regex
        public const string INT_REGEX             = @"\s*[+-]?(?<!\.)\b[0-9]+\b(?!\.[0-9])\s*$";
        public const string POS_INT_REGEX         = @"\s*[+]?(?<!\.)\b[0-9]+\b(?!\.[0-9])\s*$";
        public const string FRAME_SEPARATOR_REGEX = @"(\s*)?,?(\s*)?";
        public const string TIME_REGEX            = @"\s*(([+\-]?\s*[0-9]+(\.[0-9]+)?\s*(s|ms))|([+\-]?\s*[0-9]+))\s*$";

        #endregion

        public XmlElement AnimationStateTag { get; private set; }
        public List<AbstractAnimationStepAction> StepActions { get; private set; }
        public string StateName { get; private set; }

        private bool hasFrameTags = false;

        private int repeatStart = 0;
        private int repeatEnd;
        private int repeatStep = 1;

        public AnimationStateReader(XmlElement tag)
        {
            AnimationStateTag = tag;
            StepActions = new List<AbstractAnimationStepAction>();
            StateName = string.Empty;
        }

        public void Read()
        {
            ParseElementChildNodes(AnimationStateTag);
            ParseElementAttributes(AnimationStateTag);
        }

        private void ParseElementChildNodes(XmlElement parentElement)
        {
            foreach (var node in parentElement.ChildNodes)
            {
                var element = node as XmlElement;

                if (element == null)
                {
                    continue;
                }

                switch (element.Name)
                {
                    case FRAMES:
                        hasFrameTags = !hasFrameTags;
                        if (!hasFrameTags)
                        {
                            throw new AAMLConfigurationException();
                        }

                        foreach (var frame in Regex.Split(element.InnerText, FRAME_SEPARATOR_REGEX))
                        {
                            StepActions.Add(new FrameAnimationStepAction(StateName + frame));
                        }
                        break;

                    case STEP_ACTIONS:
                        hasFrameTags = !hasFrameTags;
                        if (!hasFrameTags)
                        {
                            throw new AAMLConfigurationException();
                        }
                        ParseElementChildNodes(element);
                        break;

                    // Step Action Inner Tags
                    case FRAME:
                        StepActions.Add(
                            new FrameAnimationStepAction(element.InnerText));
                        break;

                    case WAIT:
                        int waitFrame = Convert.ToInt32(
                            Regex.Match(element.InnerText, INT_REGEX).Groups[1].Value);
                        StepActions.Add(new WaitAnimationStepAction(waitFrame));
                        break;

                    case SOUND:
                        throw new NotImplementedException();
                        // Not done
                        /*
                        if (element.InnerText.Contains(UriUtilities.URI_SEPARATOR))
                        {
                            AssetLoader.Load<SoundEffect>(element.InnerText);
                        }
                        else
                        {
                            AssetLoader.LoadUsingExtension(element.InnerText);
                        }
                        */

                    case CALL_FUNCTION:
                        throw new NotImplementedException();

                    case REVERSE:
                        StepActions.Add(new ReverseAnimationStepAction());
                        break;

                    case REPEAT:
                        ParseElementAttributes(element);
                        
                        for (int i = repeatStart; i < repeatEnd + 1; i += repeatStep)
                        {
                            foreach (var repeatNode in element.ChildNodes)
                            {
                                var repeatElement = repeatNode as XmlElement;

                                if (repeatElement == null)
                                {
                                    continue;
                                }

                                switch (repeatElement.Name)
                                {
                                    case FRAME:
                                        StepActions.Add(new FrameAnimationStepAction(StateName + i));
                                        break;

                                    case WAIT:
                                        int repWaitFrame = Convert.ToInt32(Regex.Match(repeatElement.InnerText, INT_REGEX).Groups[1].Value);
                                        StepActions.Add(new WaitAnimationStepAction(repWaitFrame));
                                        break;

                                    default:
                                        break;
                                }
                            }
                        }
                        break;

                    // Optional Inner Tags
                    case FRAME_DURATION:
                        StepActions.Add(new WaitAnimationStepAction(Convert.ToInt32(Regex.Match(element.InnerText, POS_INT_REGEX))));
                        break;

                    case LOOP_TYPE:
                        string type = element.InnerText.Trim().ToLower();

                        if (type.Equals("cycle"))
                        {
                            StepActions.Add(new FrameAnimationStepAction(StateName + "0"));
                        }
                        else if (type.Equals("reverse"))
                        {
                            StepActions.Add(new ReverseAnimationStepAction());
                        }

                        break;

                    case LOOP_COUNT:
                        break;

                    // Timing elements
                    case WHEN_FINISHED:
                        ParseElementChildNodes(element);
                        break;

                    case WHEN_AFTER:
                        // Check for time attribute then childNodes
                        break;

                    // Inner timing elements
                    case ENTER:
                        StepActions.Add(new EnterAnimationStepAction(element.InnerText.Trim()));
                        break;

                    case JUMP_TO:
                        if (Regex.IsMatch(element.InnerText, POS_INT_REGEX))
                        {
                            StepActions.Add(new JumpToAnimationStepAction(Convert.ToInt32(Regex.Match(element.InnerText, POS_INT_REGEX).Groups[1])));
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        private void ParseElementAttributes(XmlElement element)
        {
            foreach (XmlAttribute attrib in element.Attributes)
            {
                switch (attrib.Name)
                {
                    case NAME:
                        if (!attrib.Value.Equals(string.Empty))
                        {
                            StateName = attrib.Value;
                        }
                        break;

                    case START:
                        repeatStart = Convert.ToInt32(attrib.Value.Trim());
                        break;

                    case END:
                        repeatEnd = Convert.ToInt32(attrib.Value.Trim());
                        break;

                    case STEP:
                        repeatStep = Convert.ToInt32(attrib.Value.Trim());
                        break;

                    default:
                        break;
                }
            }
        }
    }
}