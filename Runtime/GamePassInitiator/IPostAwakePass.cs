namespace Bloodthirst.Scripts.Core.GamePassInitiator
{
    public interface IPostAwakePass : IGamePass
    {
        new void Execute();
    }
}
