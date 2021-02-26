using System;
using System.Collections.Generic;

namespace Bloodthirst.Models
{
    public struct GUIDConnectionInfo
    {
        public GUIDAndPrefabPath PlayerNetworkID { get; set; }

        public List<GUIDAndPrefabPath> ExistingPlayers { get; set; }
    }
}
