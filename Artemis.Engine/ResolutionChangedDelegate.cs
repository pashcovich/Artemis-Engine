using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Artemis.Engine
{
    /// <summary>
    /// A delegate fired when the Resolution of the display changes.
    /// </summary>
    /// <param name="previousResolution">The previous value of ArtemisEngine.DisplayManager.WindowResolution.</param>
    /// <param name="currentResolution">The current value of ArtemisEngine.DisplayManager.WindowResolution.</param>
    /// <param name="baseScaleFactors">The ratio of the current resolution to the game's base resolution.</param>
    public delegate void ResolutionChangedDelegate(
        Resolution previousResolution,
        Resolution currentResolution,
        Vector2 baseScaleFactors
    );
}
