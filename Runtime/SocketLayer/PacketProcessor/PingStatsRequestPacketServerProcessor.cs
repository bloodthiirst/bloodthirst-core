using Assets.Models;
using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket.Serializer;
using System;

namespace Assets.SocketLayer.PacketParser
{
    public class PingStatsRequestPacketServerProcessor : PacketServerProcessor<PingStatsRequest, Guid>
    {
        public override INetworkSerializer<PingStatsRequest> DataSerializer { get; }

        public override INetworkSerializer<Guid> IdentifierSerializer { get; }

        public PingStatsRequestPacketServerProcessor() : base()
        {
            IdentifierSerializer = new IdentityGUIDNetworkSerializer();

            DataSerializer = new BaseNetworkSerializer<PingStatsRequest>();
        }


        public override bool Validate(ref PingStatsRequest data)
        {
            return true;
        }
    }
}
