namespace Bloodthirst.Scripts.Core.GamePassInitiator
{
    public interface IAfterAllScenesIntializationPass : IGamePass
    {
        void Execute();
    }
}
