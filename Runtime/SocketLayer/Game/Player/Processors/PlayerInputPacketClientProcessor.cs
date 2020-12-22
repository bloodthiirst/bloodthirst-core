using Assets.Models;
using Assets.Scripts.SocketLayer.Models;
using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket.Serializer;
using System;
using UnityEngine;

namespace Assets.SocketLayer.PacketParser
{
    public class PlayerInputPacketClientProcessor : PacketClientProcessor<PlayerInput, Guid>
    {
        public override INetworkSerializer<PlayerInput> DataSerializer { get; }

        public override INetworkSerializer<Guid> IdentifierSerializer { get; }

        public PlayerInputPacketClientProcessor() : base()
        {
            IdentifierSerializer = new IdentityGUIDNetworkSerializer();

            DataSerializer = new BaseNetworkSerializer<PlayerInput>();
        }


        public override bool Validate(ref PlayerInput data)
        {
            return true;
        }
    }
}
