namespace Bloodthirst.Scripts.Core.GamePassInitiator
{
    public interface IPostSceneInitializationPass : IGamePass
    {
        int SceneOrder { get; }
        void Execute();
    }
}
