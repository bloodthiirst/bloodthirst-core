using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Models
{
    public struct GUIDConnectionInfo
    {
        public Guid PlayerNetworkID { get; set; }

        public List<Guid> ExistingPlayers { get; set; }
    }
}
