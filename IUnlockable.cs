using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnlockAtStart
{
    internal interface IUnlockable
    {
        void Unlock();
        bool ShouldUnlock(Configuration config);
    }
}
