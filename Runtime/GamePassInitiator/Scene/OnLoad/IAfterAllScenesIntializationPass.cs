namespace Bloodthirst.Scripts.Core.GamePassInitiator
{
    public interface IAfterAllScenesIntializationPass : IGamePass
    {
        new void Execute();
    }
}
