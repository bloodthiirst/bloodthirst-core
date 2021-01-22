namespace Assets.Scripts.Core.GamePassInitiator
{
    public interface IPostSceneLoadedPass : IGamePass
    {
        void DoScenePass();
    }
}
