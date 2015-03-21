using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewGamePlus
{
    internal interface IUnlockable
    {
        void Unlock();
        bool ShouldUnlock(Configuration config);
    }
}
