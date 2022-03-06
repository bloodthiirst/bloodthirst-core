namespace Bloodthirst.Scripts.Core.GamePassInitiator
{
    public interface IInjectPass : IGamePass
    {
        new void Execute();
    }
}
