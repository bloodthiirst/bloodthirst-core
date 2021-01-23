using System;
using System.Collections.Generic;

namespace Bloodthirst.Models
{
    public struct GUIDConnectionInfo
    {
        public Guid PlayerNetworkID { get; set; }

        public List<Guid> ExistingPlayers { get; set; }
    }
}
