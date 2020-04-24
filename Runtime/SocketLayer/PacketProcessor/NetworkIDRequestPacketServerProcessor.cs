using Assets.Models;
using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket.Serializer;
using System;
using UnityEngine;

namespace Assets.SocketLayer.PacketParser
{
    public class NetworkIDRequestPacketServerProcessor : PacketServerProcessor<NetworkIDRequest, Guid>
    {
        public override INetworkSerializer<NetworkIDRequest> DataSerializer { get; }

        public override INetworkSerializer<Guid> IdentifierSerializer { get; }

        public NetworkIDRequestPacketServerProcessor() : base()
        {
            IdentifierSerializer = new IdentityGUIDNetworkSerializer();

            DataSerializer = new BaseNetworkSerializer<NetworkIDRequest>();
        }


        public override bool Validate(ref NetworkIDRequest data)
        {
            return true;
        }
    }
}
