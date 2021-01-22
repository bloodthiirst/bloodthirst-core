using Assets.Models;
using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket.Serializer;
using System;

namespace Assets.SocketLayer.PacketParser
{
    public class PingPongTCPPacketServerProcessor : PacketServerProcessor<PingPongTCP, Guid>
    {
        public override INetworkSerializer<PingPongTCP> DataSerializer { get; }

        public override INetworkSerializer<Guid> IdentifierSerializer { get; }

        public PingPongTCPPacketServerProcessor() : base()
        {
            IdentifierSerializer = new IdentityGUIDNetworkSerializer();

            DataSerializer = new BaseNetworkSerializer<PingPongTCP>();
        }


        public override bool Validate(ref PingPongTCP data)
        {
            return true;
        }
    }
}
