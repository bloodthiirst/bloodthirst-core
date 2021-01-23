using Bloodthirst.Models;
using System;

namespace Bloodthirst.Socket.PacketParser
{
    public class ClientNetworkIDRequestPacketServerProcessor : PacketServerProcessor<ClientNetworkIDRequest, Guid>
    {
        public ClientNetworkIDRequestPacketServerProcessor() : base()
        {
        }

        public override bool Validate(ref ClientNetworkIDRequest data)
        {
            return true;
        }
    }
}
