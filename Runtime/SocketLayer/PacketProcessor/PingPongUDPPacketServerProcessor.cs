using Assets.Models;
using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket.Serializer;
using System;
using UnityEngine;

namespace Assets.SocketLayer.PacketParser
{
    public class PingPongUDPPacketServerProcessor : PacketServerProcessor<PingPongUDP, Guid>
    {
        public override INetworkSerializer<PingPongUDP> DataSerializer { get; }

        public override INetworkSerializer<Guid> IdentifierSerializer { get; }

        public PingPongUDPPacketServerProcessor() : base()
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
