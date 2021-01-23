using Bloodthirst.Models;
using System;

namespace Bloodthirst.Socket.PacketParser
{
    public class PingPongTCPPacketClientProcessor : PacketClientProcessor<PingPongTCP, Guid>
    {
        public PingPongTCPPacketClientProcessor() : base()
        {
        }

        public override bool Validate(ref PingPongTCP data)
        {
            return true;
        }
    }
}
