using Assets.Models;
using Assets.Scripts.SocketLayer.Game.Player.Models;
using Assets.Scripts.SocketLayer.Models;
using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket.Serializer;
using System;
using UnityEngine;

namespace Assets.SocketLayer.PacketParser
{
    public class PlayerRotationPacketServerProcessor : PacketServerProcessor<PlayerRotation, Guid>
    {
        public override INetworkSerializer<PlayerRotation> DataSerializer { get; }

        public override INetworkSerializer<Guid> IdentifierSerializer { get; }

        public PlayerRotationPacketServerProcessor() : base()
        {
            IdentifierSerializer = new IdentityGUIDNetworkSerializer();

            DataSerializer = new BaseNetworkSerializer<PlayerRotation>();
        }


        public override bool Validate(ref PlayerRotation data)
        {
            return true;
        }
    }
}
