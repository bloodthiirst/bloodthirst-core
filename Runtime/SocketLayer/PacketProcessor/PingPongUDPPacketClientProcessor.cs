using Assets.Models;
using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket.Serializer;
using System;

namespace Assets.SocketLayer.PacketParser
{
    public class PingPongUDPPacketClientProcessor : PacketClientProcessor<PingPongUDP, Guid>
    {
        public override INetworkSerializer<PingPongUDP> DataSerializer { get; }

        public override INetworkSerializer<Guid> IdentifierSerializer { get; }

        public PingPongUDPPacketClientProcessor() : base()
        {
            IdentifierSerializer = new IdentityGUIDNetworkSerializer();

            DataSerializer = new BaseNetworkSerializer<PingPongUDP>();
        }


        public override bool Validate(ref PingPongUDP data)
        {
            return true;
        }
    }
}
