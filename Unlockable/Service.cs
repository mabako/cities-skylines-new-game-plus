using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewGamePlus
{
    class Service : IUnlockable
    {
        private static ItemClass.SubService[] relevantSubservices = new ItemClass.SubService[] { ItemClass.SubService.PublicTransportBus, ItemClass.SubService.PublicTransportMetro, ItemClass.SubService.PublicTransportPlane, ItemClass.SubService.PublicTransportShip, ItemClass.SubService.PublicTransportTrain };
        private static Dictionary<ItemClass.SubService, MilestoneInfo> unlockMilestonesSubservice = new Dictionary<ItemClass.SubService, MilestoneInfo>();

        public void Unlock()
        {
            // Disable the milestones for opening the GUI
            UnlockManager.instance.m_properties.m_ServiceMilestones[(int)ItemClass.Service.PublicTransport] = null;
            
            // Let all panels be viewed, to prevent some rather odd bug with an empty panel showing upon opening the menu
            foreach (var service in relevantSubservices)
            {
                if (!unlockMilestonesSubservice.ContainsKey(service))
                    unlockMilestonesSubservice[service] = UnlockManager.instance.m_properties.m_SubServiceMilestones[(int)service];

                UnlockManager.instance.m_properties.m_SubServiceMilestones[(int)service] = null;
            }

            // Enable all things we so desired.
            for (int index = 0; index < PrefabCollection<BuildingInfo>.LoadedCount(); ++index)
            {
                BuildingInfo loaded = PrefabCollection<BuildingInfo>.GetLoaded((uint)index);

                bool show = false;
                show = show || (Base.Config.Buses && loaded.category == "PublicTransportBus");
                show = show || (Base.Config.Subways && loaded.category == "PublicTransportMetro");
                show = show || (Base.Config.Ships && loaded.category == "PublicTransportShip");
                show = show || (Base.Config.Trains && loaded.category == "PublicTransportTrain");
                show = show || (Base.Config.Airplanes && loaded.category == "PublicTransportPlane");

                if (show)
                    loaded.m_UnlockMilestone = null;
                else
                    loaded.m_UnlockMilestone = loaded.m_UnlockMilestone ?? GetDefaultMilestone(loaded.GetSubService());
            }

            for (int index = 0; index < PrefabCollection<TransportInfo>.LoadedCount(); ++index)
            {
                TransportInfo loaded = PrefabCollection<TransportInfo>.GetLoaded((uint)index);

                bool show = false;
                show = show || (Base.Config.Buses && loaded.GetSubService() == ItemClass.SubService.PublicTransportBus);
                show = show || (Base.Config.Subways && loaded.GetSubService() == ItemClass.SubService.PublicTransportMetro);
                show = show || (Base.Config.Ships && loaded.GetSubService() == ItemClass.SubService.PublicTransportShip);
                show = show || (Base.Config.Trains && loaded.GetSubService() == ItemClass.SubService.PublicTransportTrain);
                show = show || (Base.Config.Airplanes && loaded.GetSubService() == ItemClass.SubService.PublicTransportPlane);

                if(show)
                    loaded.m_UnlockMilestone = null;
                else
                    loaded.m_UnlockMilestone = loaded.m_UnlockMilestone ?? GetDefaultMilestone(loaded.GetSubService());
            }
        }

        public void Lock()
        {
            for (int index = 0; index < PrefabCollection<TransportInfo>.LoadedCount(); ++index)
            {
                TransportInfo loaded = PrefabCollection<TransportInfo>.GetLoaded((uint)index);

                loaded.m_UnlockMilestone = GetDefaultMilestone(loaded.GetSubService());
            }
        }

        private MilestoneInfo GetDefaultMilestone(ItemClass.SubService subService)
        {
            MilestoneInfo milestone;
            if (unlockMilestonesSubservice.TryGetValue(subService, out milestone))
            {
                return milestone;
            }
            return null;
        }

        public bool ShouldUnlock(Configuration config)
        {
            return config.Airplanes || config.Buses || config.Ships || config.Subways || config.Trains;
        }
    }
}
