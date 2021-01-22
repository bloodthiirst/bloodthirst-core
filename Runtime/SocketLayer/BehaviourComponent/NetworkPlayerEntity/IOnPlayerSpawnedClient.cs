namespace Bloodthirst.Socket.Core
{
    public interface IOnPlayerSpawnedClient
    {
        bool IsDoneClientSpawn { get; set; }
        void OnPlayerSpawnedClient();
    }
}
