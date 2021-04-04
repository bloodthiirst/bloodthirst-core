namespace Bloodthirst.Scripts.Core.GamePassInitiator
{
    public interface ISceneInitializationPass : IGamePass
    {
        int SceneOrder { get; }
        void Execute();
    }
}
