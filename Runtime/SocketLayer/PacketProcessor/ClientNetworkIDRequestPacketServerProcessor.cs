using Assets.Models;
using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket.Serializer;
using System;

namespace Assets.SocketLayer.PacketParser
{
    public class ClientNetworkIDRequestPacketServerProcessor : PacketServerProcessor<ClientNetworkIDRequest, Guid>
    {
        public override INetworkSerializer<ClientNetworkIDRequest> DataSerializer { get; }

        public override INetworkSerializer<Guid> IdentifierSerializer { get; }

        public ClientNetworkIDRequestPacketServerProcessor() : base()
        {
            IdentifierSerializer = new IdentityGUIDNetworkSerializer();

            DataSerializer = new BaseNetworkSerializer<ClientNetworkIDRequest>();
        }


        public override bool Validate(ref ClientNetworkIDRequest data)
        {
            return true;
        }
    }
}
