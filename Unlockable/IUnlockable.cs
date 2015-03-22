namespace NewGamePlus
{
    internal interface IUnlockable
    {
        void Unlock();

        void Lock();

        bool ShouldUnlock(Configuration config);
    }
}
