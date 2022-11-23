namespace Bloodthirst.Core.Updater
{
    public interface IUpdater
    {
        void Register(IUpdatable updater);

        void Unregister(IUpdatable updater);

        void Tick(float deltaTime);
    }
}
