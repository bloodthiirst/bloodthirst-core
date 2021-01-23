using Bloodthirst.Socket.Models;

namespace Bloodthirst.Socket.Core
{
    /// <summary>
    /// <para>An interface that triggers on the server side</para>
    /// <para>This triggers when the servers gets informed by the client that the spawn happend succesfully on the client part</para> 
    /// </summary>
    public interface IPlayerSpawnSuccess
    {
        void OnPlayerSpawnSuccess(GUIDPlayerSpawnSuccess spawnInfo);
    }
}
