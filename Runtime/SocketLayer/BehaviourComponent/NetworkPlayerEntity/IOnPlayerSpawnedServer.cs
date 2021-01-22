namespace Bloodthirst.Socket.Core
{
    public interface IOnPlayerSpawnedServer
    {
        bool IsDoneServerSpawn { get; set; }
        void OnPlayerSpawnedServer();
    }
}
