using Assets.Models;
using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket.Serializer;
using System;

namespace Assets.SocketLayer.PacketParser
{
    public class ServerNetworkIDRequestPacketServerProcessor : PacketServerProcessor<ServerNetworkIDRequest, Guid>
    {
        public override INetworkSerializer<ServerNetworkIDRequest> DataSerializer { get; }

        public override INetworkSerializer<Guid> IdentifierSerializer { get; }

        public ServerNetworkIDRequestPacketServerProcessor() : base()
        {
            IdentifierSerializer = new IdentityGUIDNetworkSerializer();

            DataSerializer = new BaseNetworkSerializer<ServerNetworkIDRequest>();
        }


        public override bool Validate(ref ServerNetworkIDRequest data)
        {
            return true;
        }
    }
}
