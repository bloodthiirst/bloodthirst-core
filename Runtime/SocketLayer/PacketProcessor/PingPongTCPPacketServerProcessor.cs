using Bloodthirst.Models;
using System;

namespace Bloodthirst.Socket.PacketParser
{
    public class PingPongTCPPacketServerProcessor : PacketServerProcessor<PingPongTCP, Guid>
    {
        public PingPongTCPPacketServerProcessor() : base()
        {
        }

        public override bool Validate(ref PingPongTCP data)
        {
            return true;
        }
    }
}
