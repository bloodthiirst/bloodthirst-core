using Assets.Scripts.SocketLayer.Game.Player.Models;
using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket.Serializer;
using System;

namespace Assets.SocketLayer.PacketParser
{
    public class PlayerRotationPacketClientProcessor : PacketClientProcessor<PlayerRotation, Guid>
    {
        public override INetworkSerializer<PlayerRotation> DataSerializer { get; }

        public override INetworkSerializer<Guid> IdentifierSerializer { get; }

        public PlayerRotationPacketClientProcessor() : base()
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
