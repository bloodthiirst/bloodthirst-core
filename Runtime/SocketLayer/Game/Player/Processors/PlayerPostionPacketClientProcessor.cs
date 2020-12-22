using Assets.Models;
using Assets.Scripts.SocketLayer.Models;
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

            DataSerializer = new BaseNetworkSerializer<Vector3>();
        }


        public override bool Validate(ref Vector3 data)
        {
            return true;
        }
    }
}
