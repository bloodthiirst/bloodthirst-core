using Bloodthirst.Models;
using System;

namespace Bloodthirst.Socket.PacketParser
{
    public class ServerNetworkIDRequestPacketServerProcessor : PacketServerProcessor<ServerNetworkIDRequest, Guid>
    {
        public ServerNetworkIDRequestPacketServerProcessor() : base()
        {
        }

        public override bool Validate(ref ServerNetworkIDRequest data)
        {
            return true;
        }
    }
}
