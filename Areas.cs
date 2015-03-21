using ICities;

namespace NewGamePlus
{
    internal class Areas : IUnlockable
    {
        public void Unlock()
        {
            // We only have 1 element for area milestones, and UnlockManager.Unlock(null) always returns true.

            // needs 1 element or we'll get an IndexOutOfBoundsException
            UnlockManager.instance.m_properties.m_AreaMilestones = new MilestoneInfo[] { null };
        }

        public bool ShouldUnlock(Configuration config)
        {
            return config.AllAreas;
        }
    }

    public class FreeAreas : AreasExtensionBase
    {
        public override void OnCreated(IAreas areas)
        {
        }

        public override int OnGetAreaPrice(uint ore, uint oil, uint forest, uint fertility, uint water, bool road, bool train, bool ship, bool plane, float landFlatness, int originalPrice)
        {
            if (Base.Config.FreeAreas)
                return 0;

            return base.OnGetAreaPrice(ore, oil, forest, fertility, water, road, train, ship, plane, landFlatness, originalPrice);
        }
    }
}
