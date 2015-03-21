using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewGamePlus
{
    class Service : IUnlockable
    {
        public void Unlock()
        {
            // Disable the milestones for opening the GUI
            UnlockManager.instance.m_properties.m_ServiceMilestones[(int)ItemClass.Service.PublicTransport] = null;
            
            // Let all panels be viewed, to prevent some rather odd bug with an empty panel to show
            UnlockManager.instance.m_properties.m_SubServiceMilestones[(int)ItemClass.SubService.PublicTransportBus] = null;
            UnlockManager.instance.m_properties.m_SubServiceMilestones[(int)ItemClass.SubService.PublicTransportMetro] = null;
            UnlockManager.instance.m_properties.m_SubServiceMilestones[(int)ItemClass.SubService.PublicTransportPlane] = null;
            UnlockManager.instance.m_properties.m_SubServiceMilestones[(int)ItemClass.SubService.PublicTransportShip] = null;
            UnlockManager.instance.m_properties.m_SubServiceMilestones[(int)ItemClass.SubService.PublicTransportTrain] = null;

            // Enable all things we so desired.
            for (int index = 0; index < PrefabCollection<BuildingInfo>.LoadedCount(); ++index)
            {
                BuildingInfo loaded = PrefabCollection<BuildingInfo>.GetLoaded((uint)index);

                bool show = false;
                show = show || (Base.Config.Buses && loaded.category == "PublicTransportBus");
                show = show || (Base.Config.Subways && loaded.category == "PublicTransportMetro");
                show = show || (Base.Config.Ships && loaded.category == "PublicTransportShip");
                show = show || (Base.Config.Trains && loaded.category == "PublicTransportTrain"); // || loaded.m_class.name == "Train Track"));
                show = show || (Base.Config.Airplanes && loaded.category == "PublicTransportPlane");

                if(show)
                    loaded.m_UnlockMilestone = null;
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
            }
        }

        public bool ShouldUnlock(Configuration config)
        {
            return config.Airplanes || config.Buses || config.Ships || config.Subways || config.Trains;
        }
    }
}
