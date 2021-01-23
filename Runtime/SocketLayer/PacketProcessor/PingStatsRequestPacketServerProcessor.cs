using Bloodthirst.Models;
using System;

namespace Bloodthirst.Socket.PacketParser
{
    public class PingStatsRequestPacketServerProcessor : PacketServerProcessor<PingStatsRequest, Guid>
    {
        public PingStatsRequestPacketServerProcessor() : base()
        {
        }

        public override bool Validate(ref PingStatsRequest data)
        {
            return true;
        }
    }
}
