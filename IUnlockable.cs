namespace NewGamePlus
{
    internal interface IUnlockable
    {
        void Unlock();
        bool ShouldUnlock(Configuration config);
    }
}
