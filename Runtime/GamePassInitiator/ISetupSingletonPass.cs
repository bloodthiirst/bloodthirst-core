namespace Bloodthirst.Scripts.Core.GamePassInitiator
{
    public interface ISetupSingletonPass : IGamePass
    {
        new void Execute();
    }
}
