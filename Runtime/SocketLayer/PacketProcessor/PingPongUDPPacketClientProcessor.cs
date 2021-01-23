using Bloodthirst.Models;
using System;

namespace Bloodthirst.Socket.PacketParser
{
    public class PingPongUDPPacketClientProcessor : PacketClientProcessor<PingPongUDP, Guid>
    {
        public PingPongUDPPacketClientProcessor() : base()
        {
        }

        public override bool Validate(ref PingPongUDP data)
        {
            return true;
        }
    }
}
