using Bloodthirst.Socket.Models;
using System;

namespace Bloodthirst.Socket.PacketParser
{
    public class GUIDPlayerSpawnSuccessPacketServerProcessor : PacketServerProcessor<GUIDPlayerSpawnSuccess, Guid>
    {
        public GUIDPlayerSpawnSuccessPacketServerProcessor() : base()
        {
        }

        public override bool Validate(ref GUIDPlayerSpawnSuccess data)
        {
            return true;
        }
    }
}
