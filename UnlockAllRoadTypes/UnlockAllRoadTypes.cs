using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnlockAllRoadTypes
{
    public class ModInfo : IUserMod
    {
        public string Description
        {
            get { return "Start with all Road Types"; }
        }

        public string Name
        {
            get { return "Unlock all Road Types"; }
        }
    }

    public class Unlocker : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
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
                        SetPrivateVariable<MilestoneInfo>(intersectionAI, "m_cachedUnlockMilestone", null);
                    }
                }
            }

            managers.milestones.UnlockMilestone("Basic Road Created");
        }

        private bool UnlockClass(ItemClass itemClass)
        {
            var name = itemClass.name;
            return name.Contains("Road") || name == "Highway";
        }


        private void SetPrivateVariable<T>(object obj, string fieldName, T value)
        {
            obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(obj, value);
        }
    }
}
