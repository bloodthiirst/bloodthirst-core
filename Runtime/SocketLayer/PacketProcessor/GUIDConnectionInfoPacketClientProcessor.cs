using Assets.Models;
using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket.Serializer;
using System;

namespace Assets.SocketLayer.PacketParser
{
    public class GUIDConnectionInfoPacketClientProcessor : PacketClientProcessor<GUIDConnectionInfo, Guid>
    {
        public override INetworkSerializer<GUIDConnectionInfo> DataSerializer { get; }

        public override INetworkSerializer<Guid> IdentifierSerializer { get; }

        public GUIDConnectionInfoPacketClientProcessor() : base()
        {
            IdentifierSerializer = new IdentityGUIDNetworkSerializer();

            DataSerializer = new BaseNetworkSerializer<GUIDConnectionInfo>();
        }


        public override bool Validate(ref GUIDConnectionInfo data)
        {
            return true;
        }
    }
}
