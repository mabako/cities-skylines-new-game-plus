using ICities;

namespace NewGamePlus
{
    public class Money : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Message, "Level Loaded");

            if (mode == LoadMode.NewGame && Base.Config.StartMoney >= 0)
            {
                Base.SetPrivateVariable<long>(EconomyManager.instance, "m_cashAmount", Base.Config.StartMoney * 100L);
            }
        }
    }
}
