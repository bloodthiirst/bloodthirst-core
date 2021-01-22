using System;

namespace Assets.Models
{
    public struct GUIDPlayerSpawnSuccess
    {
        public Guid SpawnedPlayerID { get; set; }
        public Guid ClientThePlayerSpawnedIn { get; set; }
    }
}
