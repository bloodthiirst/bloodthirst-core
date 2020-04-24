using Assets.Models;
using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket.Serializer;
using System;
using UnityEngine;

namespace Assets.SocketLayer.PacketParser
{
    public class GUIDNewPlayerConnectedPacketClientProcessor : PacketClientProcessor<GUIDNewPlayerConnected, Guid>
    {
        public override INetworkSerializer<GUIDNewPlayerConnected> DataSerializer { get; }

        public override INetworkSerializer<Guid> IdentifierSerializer { get; }

        public GUIDNewPlayerConnectedPacketClientProcessor() : base()
        {
            IdentifierSerializer = new IdentityGUIDNetworkSerializer();

            DataSerializer = new BaseNetworkSerializer<GUIDNewPlayerConnected>();
        }


        public override bool Validate(ref GUIDNewPlayerConnected data)
        {
            return true;
        }
    }
}
