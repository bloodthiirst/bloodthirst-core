using System;

namespace Bloodthirst.Socket.Models
{
    public struct GUIDPlayerSpawnSuccess
    {
        public Guid SpawnedPlayerID { get; set; }
        public Guid ClientThePlayerSpawnedIn { get; set; }
    }
}
