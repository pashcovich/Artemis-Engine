#region Using Statements

using Artemis.Engine.Effectors;
using Artemis.Engine.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion

namespace Artemis.Engine
{

    /// <summary>
    /// An ArtemisObject is the most generic form of "game object" found in the Artemis Engine.
    /// </summary>
    public class ArtemisObject : EffectableObject
    {
        protected ArtemisObject() : base() { }
    }
}