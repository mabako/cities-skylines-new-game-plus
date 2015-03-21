using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnlockAllRoadTypes
{
    internal class Areas : IUnlockable
    {
        public void Unlock()
        {
            // needs 1 element or we'll get an IndexOutOfBoundsException
            UnlockManager.instance.m_properties.m_AreaMilestones = new MilestoneInfo[] { null };
        }

        public bool ShouldUnlock(Configuration config)
        {
            return config.AllAreas;
        }
    }
}
