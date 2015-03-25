﻿namespace NewGamePlus
{
    internal class RoadTypes : IUnlockable
    {
        /// <summary>
        /// Unlocks all road types and all intersections
        /// </summary>
        public void Unlock()
        {
            // Normal road types, easy.
            for (int index = 0; index < PrefabCollection<NetInfo>.LoadedCount(); ++index)
            {
                NetInfo loaded = PrefabCollection<NetInfo>.GetLoaded((uint)index);

                if (loaded == null || loaded.m_class == null || loaded.m_class.name == null)
                    continue;

                if (UnlockClass(loaded.m_class))
                    loaded.m_UnlockMilestone = null;
            }

            // Intersections.
            for (int index = 0; index < PrefabCollection<BuildingInfo>.LoadedCount(); ++index)
            {
                BuildingInfo loaded = PrefabCollection<BuildingInfo>.GetLoaded((uint)index);

                if (loaded == null || loaded.m_class == null || loaded.m_class.name == null)
                    continue;

                if (UnlockClass(loaded.m_class))
                {
                    loaded.m_UnlockMilestone = null;

                    var intersectionAI = loaded.m_buildingAI as IntersectionAI;
                    if (intersectionAI != null)
                    {
                        // The cached milestone here is generally the "highest" road type used.
                        Base.SetPrivateVariable<MilestoneInfo>(intersectionAI, "m_cachedUnlockMilestone", null);
                    }
                }
            }
        }
        public void Lock()
        {
            // The game handles all of this already.
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
