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
            get { return "Unlock Road Types, all Areas, etc. to be purchasable at Start"; }
        }

        public string Name
        {
            get { return "Unlock at Start"; }
        }
    }

    public class Base : MilestonesExtensionBase
    {
        internal static void SetPrivateVariable<T>(object obj, string fieldName, T value)
        {
            obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(obj, value);
        }

        internal static T GetPrivateVariable<T>(object obj, string fieldName)
        {
            return (T)obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);
        }


        private static void Unlock()
        {
            Configuration config = Configuration.Deserialize() ?? new Configuration();
            Configuration.Serialize(config);

            Unlock(config, typeof(RoadTypes), typeof(Areas));
        }

        private static void Unlock(Configuration config, params Type[] types)
        {
            foreach (Type t in types)
            {
                IUnlockable unlockable = Activator.CreateInstance(t) as IUnlockable;
                if (unlockable != null && unlockable.ShouldUnlock(config))
                    unlockable.Unlock();
            }
        }
        public override void OnRefreshMilestones()
        {
            milestonesManager.UnlockMilestone("Basic Road Created");
            Unlock();
        }
    }
}
