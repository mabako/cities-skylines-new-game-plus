using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnlockAllRoadTypes
{
    internal class RoadTypes : IUnlockable
    {
        public void Unlock()
        {
            for (int index = 0; index < PrefabCollection<NetInfo>.LoadedCount(); ++index)
            {
                NetInfo loaded = PrefabCollection<NetInfo>.GetLoaded((uint)index);

                if (UnlockClass(loaded.m_class))
                    loaded.m_UnlockMilestone = null;
            }

            for (int index = 0; index < PrefabCollection<BuildingInfo>.LoadedCount(); ++index)
            {
                BuildingInfo loaded = PrefabCollection<BuildingInfo>.GetLoaded((uint)index);
                if (UnlockClass(loaded.m_class))
                {
                    loaded.m_UnlockMilestone = null;

                    var intersectionAI = loaded.m_buildingAI as IntersectionAI;
                    if (intersectionAI != null)
                    {
                        Base.SetPrivateVariable<MilestoneInfo>(intersectionAI, "m_cachedUnlockMilestone", null);
                    }
                }
            }
        }

        private bool UnlockClass(ItemClass itemClass)
        {
            var name = itemClass.name;
            return name.Contains("Road") || name == "Highway";
        }

        public bool ShouldUnlock(Configuration config)
        {
            return config.AllRoads;
        }
    }
}
