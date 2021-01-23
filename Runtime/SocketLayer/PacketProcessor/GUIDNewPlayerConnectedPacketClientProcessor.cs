using Bloodthirst.Models;
using System;

namespace Bloodthirst.Socket.PacketParser
{
    public class GUIDNewPlayerConnectedPacketClientProcessor : PacketClientProcessor<GUIDNewPlayerConnected, Guid>
    {
        public GUIDNewPlayerConnectedPacketClientProcessor() : base()
        {
        }

        public override bool Validate(ref GUIDNewPlayerConnected data)
        {
            return true;
        }
    }
}
