using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NewGamePlus
{
    public class ModInfo : IUserMod
    {
        private Options options = null;

        public ModInfo()
        {
            try
            {
                NewGamePanel newGamePanel = UIView.library.Get<NewGamePanel>("NewGamePanel");
                if (newGamePanel != null)
                {
                    options = new Options(newGamePanel);
                    SimulationManager.RegisterSimulationManager(new NewGamePlusSimManager());

                    pluginsChanged();
                    PluginManager.instance.eventPluginsChanged += pluginsChanged;
                    PluginManager.instance.eventPluginsStateChanged += pluginsChanged;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void pluginsChanged()
        {
            foreach (var plugin in PluginManager.instance.GetPluginsInfo())
            {
                // Name is only applicable if deployed as local file, otherwise it'll be the string representation of the steam workshop ID
                if(plugin.publishedFileID.AsUInt64 == 411769510L || plugin.name == "mbkNewGamePlus")
                {
                    options.Show(plugin.isEnabled);
                }
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

        // NewGame/LoadGame usually.
        internal static SimulationManager.UpdateMode mode = SimulationManager.UpdateMode.Undefined;

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
            set
            {
                config = value;
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

        /// <summary>
        /// Unlock all unlockables if configured to.
        /// </summary>
        internal static void Unlock()
        {
            Unlock(typeof(RoadTypes), typeof(Areas), typeof(Service));
        }

        /// <summary>
        /// Unlocks all given parameter types if they should.
        /// </summary>
        /// <param name="types"></param>
        internal static void Unlock(params Type[] types)
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

        /// <summary>
        /// Please note we could -technically- use our SimulationManager instance to store this data.
        /// I have no clue what would happen, most likely removing the mod would break your savegame.
        /// </summary>
        /// <returns></returns>
        private bool LoadData()
        {
            config = new Configuration();

            byte[] data;
            if(SimulationManager.instance.m_serializableDataStorage.TryGetValue("NewGamePlus/Storage", out data))
            {
                if (data != null && data.Length > 0)
                {
                    switch(data[0])
                    {
                        case 0x1:
                            {
                                if(data.Length != 2)
                                    return false;

                                // Savegame version 1.
                                byte x = data[1];
                                config.Airplanes = (x & 0x1) != 0;
                                config.AllAreas = (x & 0x2) != 0;
                                config.AllRoads = (x & 0x4) != 0;
                                config.Buses = (x & 0x8) != 0;
                                config.FreeAreas = (x & 0x10) != 0;
                                config.Ships = (x & 0x20) != 0;
                                config.Subways = (x & 0x40) != 0;
                                config.Trains = (x & 0x80) != 0;

                                return true;
                            }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Please note we could -technically- use our SimulationManager instance to store this data.
        /// I have no clue what would happen, most likely removing the mod would break your savegame.
        /// </summary>
        internal static void SaveData()
        {
            byte[] data = new byte[2];
            data[0] = 0x1;

            data[1] |= ((byte)(Config.Airplanes ? 0x1 : 0));
            data[1] |= ((byte)(Config.AllAreas ? 0x2 : 0));
            data[1] |= ((byte)(Config.AllRoads ? 0x4 : 0));
            data[1] |= ((byte)(Config.Buses ? 0x8 : 0));
            data[1] |= ((byte)(Config.FreeAreas ? 0x10 : 0));
            data[1] |= ((byte)(Config.Ships ? 0x20 : 0));
            data[1] |= ((byte)(Config.Subways ? 0x40 : 0));
            data[1] |= ((byte)(Config.Trains ? 0x80 : 0));

            SimulationManager.instance.m_serializableDataStorage["NewGamePlus/Storage"] = data;
        }

        /// <summary>
        /// Note that this function is executed -before- LoadingExtension's OnLevelLoaded is.
        /// </summary>
        public override void OnRefreshMilestones()
        {
            milestonesManager.UnlockMilestone("Basic Road Created");

            // DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Message, string.Format("Yo, we're refreshing your milestones in {0} mode", mode));

            switch(mode)
            {
                case SimulationManager.UpdateMode.NewGame:
                    SaveData();
                    Unlock();
                    break;

                case SimulationManager.UpdateMode.LoadGame:
                    LoadData();
                    Unlock();
                    break;
            }
        }
    }
}
