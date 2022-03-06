namespace Bloodthirst.Scripts.Core.GamePassInitiator
{
    public interface IPostSceneInitializationPass : IGamePass
    {
        void Execute();
    }
}
