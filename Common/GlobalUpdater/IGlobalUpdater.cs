namespace Bloodthirst.Core.Updater
{
    public interface IGlobalUpdater
    {
        void Initialize();
        void Register(IUpdater updater);
        void Unregister(IUpdater updater);
    }
}
