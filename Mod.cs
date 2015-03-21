using ColossalFramework.UI;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NewGamePlus
{
    public class ModInfo : IUserMod
    {
        public ModInfo()
        {
            try
            {
                NewGamePanel newGamePanel = UIView.library.Get<NewGamePanel>("NewGamePanel");
                if (newGamePanel != null)
                    new Options(newGamePanel);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

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
        private static Configuration config = null;

        internal static Configuration Config
        {
            get
            {
                if(config == null)
                {
                    config = Configuration.Deserialize() ?? new Configuration();
                    Configuration.Serialize(config);
                }
                return config;
            }
        }
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
            Unlock(typeof(RoadTypes), typeof(Areas), typeof(Service));
        }

        private static void Unlock(params Type[] types)
        {
            foreach (Type t in types)
            {
                IUnlockable unlockable = Activator.CreateInstance(t) as IUnlockable;
                if (unlockable != null)
                {
                    if(unlockable.ShouldUnlock(Config))
                        unlockable.Unlock();
                }
            }
        }
        public override void OnRefreshMilestones()
        {
            milestonesManager.UnlockMilestone("Basic Road Created");
            Unlock();
        }
    }
}
