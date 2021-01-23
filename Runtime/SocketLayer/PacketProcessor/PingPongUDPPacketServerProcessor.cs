using Bloodthirst.Models;
using System;

namespace Bloodthirst.Socket.PacketParser
{
    public class PingPongUDPPacketServerProcessor : PacketServerProcessor<PingPongUDP, Guid>
    {
        public PingPongUDPPacketServerProcessor() : base()
        {
        }

        public override bool Validate(ref PingPongUDP data)
        {
            return true;
        }
    }
}
