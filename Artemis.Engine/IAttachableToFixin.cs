using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Artemis.Engine
{
    public interface IAttachableToFixin<T> where T : AbstractFixin
    {
        void AttachFixin(T fixin);
    }
}
