namespace Bloodthirst.Scripts.Core.GamePassInitiator
{
    public interface IPostSceneLoadedPass : IGamePass
    {
        void DoScenePass();
    }
}
