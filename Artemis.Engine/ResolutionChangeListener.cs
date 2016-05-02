using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Artemis.Engine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ResolutionChangeListener : Attribute { }
}
