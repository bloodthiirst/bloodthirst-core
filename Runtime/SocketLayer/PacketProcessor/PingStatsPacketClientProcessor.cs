using Bloodthirst.Models;
using System;

namespace Bloodthirst.Socket.PacketParser
{
    public class PingStatsPacketClientProcessor : PacketClientProcessor<PingStats, Guid>
    {
        public PingStatsPacketClientProcessor() : base()
        {
        }

        public override bool Validate(ref PingStats data)
        {
            return true;
        }
    }
}
