using Assets.Models;
using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket.Serializer;
using System;

namespace Assets.SocketLayer.PacketParser
{
    public class PingStatsPacketClientProcessor : PacketClientProcessor<PingStats, Guid>
    {
        public override INetworkSerializer<PingStats> DataSerializer { get; }

        public override INetworkSerializer<Guid> IdentifierSerializer { get; }

        public PingStatsPacketClientProcessor() : base()
        {
            IdentifierSerializer = new IdentityGUIDNetworkSerializer();

            DataSerializer = new BaseNetworkSerializer<PingStats>();
        }


        public override bool Validate(ref PingStats data)
        {
            return true;
        }
    }
}
