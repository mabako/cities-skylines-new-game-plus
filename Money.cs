using ICities;

namespace NewGamePlus
{
    public class Money : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode == LoadMode.NewGame && Base.Config.StartMoney >= 0)
            {
                Base.SetPrivateVariable<long>(EconomyManager.instance, "m_cashAmount", Base.Config.StartMoney * 100L);

                // Base.SaveData();
                // This has no effect whatsoever.
                // Base.Unlock();
            }
        }

        public override void OnLevelUnloading()
        {
            // restore config
            Base.Config = Configuration.Deserialize() ?? new Configuration();
        }
    }
}
