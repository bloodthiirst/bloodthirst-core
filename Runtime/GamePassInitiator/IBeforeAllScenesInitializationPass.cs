namespace Bloodthirst.Scripts.Core.GamePassInitiator
{
    public interface IBeforeAllScenesInitializationPass : IGamePass
    {
        void Execute();
    }
}
