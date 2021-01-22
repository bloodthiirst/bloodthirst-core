using Assets.Models;
using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket.Serializer;
using System;

namespace Assets.SocketLayer.PacketParser
{
    public class GUIDPlayerSpawnSuccessPacketServerProcessor : PacketServerProcessor<GUIDPlayerSpawnSuccess, Guid>
    {
        public override INetworkSerializer<GUIDPlayerSpawnSuccess> DataSerializer { get; }

        public override INetworkSerializer<Guid> IdentifierSerializer { get; }

        public GUIDPlayerSpawnSuccessPacketServerProcessor() : base()
        {
            IdentifierSerializer = new IdentityGUIDNetworkSerializer();

            DataSerializer = new GUIDPlayerSpawnSuccessNetworkSerializer();
        }


        public override bool Validate(ref GUIDPlayerSpawnSuccess data)
        {
            return true;
        }
    }
}
