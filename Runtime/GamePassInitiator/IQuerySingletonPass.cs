namespace Bloodthirst.Scripts.Core.GamePassInitiator
{
    public interface IQuerySingletonPass : IGamePass
    {
        new void Execute();
    }
}
