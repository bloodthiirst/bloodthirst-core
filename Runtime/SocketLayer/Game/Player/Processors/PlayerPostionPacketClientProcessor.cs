using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket.Serializer;
using System;
using UnityEngine;

namespace Assets.SocketLayer.PacketParser
{
    public class PlayerPostionPacketClientProcessor : PacketClientProcessor<Vector3, Guid>
    {
        public override INetworkSerializer<Vector3> DataSerializer { get; }

        public override INetworkSerializer<Guid> IdentifierSerializer { get; }

        public PlayerPostionPacketClientProcessor() : base()
        {
            IdentifierSerializer = new IdentityGUIDNetworkSerializer();

            DataSerializer = Vector3NetworkSerializer.Instance;
        }


        public override bool Validate(ref Vector3 data)
        {
            return true;
        }
    }
}
