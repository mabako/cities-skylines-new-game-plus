using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewGamePlus
{
    /// <summary>
    /// You may rightfully wonder why this class exists.
    /// 
    /// A short course:
    ///   There is absolutely no way to obtain a UpdateMode other than in a SimulationManager instance -or- in a ILoadingExtension because it's only passed through, not stored anywhere.
    ///   Unfortunately for us, ILoadingExtension is called when the game seems to be fully loaded.
    ///   This means, however, that we cannot react before that to whether it's a new or existing game. 
    ///   And we have to, as this way of unlocking does NOT work in ILoadingExtension.
    /// </summary>
    public class NewGamePlusSimManager : ISimulationManager
    {
        private ThreadProfiler tp = new ThreadProfiler();

        public void GetData(FastList<ColossalFramework.IO.IDataContainer> data)
        {
        }

        public string GetName()
        {
            return "NewGamePlusSimMgr";
        }

        public ThreadProfiler GetSimulationProfiler()
        {
            return tp;
        }

        public void LateUpdateData(SimulationManager.UpdateMode mode)
        {
            // This is where RefreshMilestones is called, in another manager.
        }

        public void SimulationStep(int subStep)
        {
        }

        public void UpdateData(SimulationManager.UpdateMode mode)
        {
            // we -only- want this. ffs.
            Base.mode = mode;
        }
    }
}
