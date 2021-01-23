using Bloodthirst.Models;
using System;

namespace Bloodthirst.Socket.PacketParser
{
    public class GUIDConnectionInfoPacketClientProcessor : PacketClientProcessor<GUIDConnectionInfo, Guid>
    {
        public GUIDConnectionInfoPacketClientProcessor() : base()
        {
        }

        public override bool Validate(ref GUIDConnectionInfo data)
        {
            return true;
        }
    }
}
