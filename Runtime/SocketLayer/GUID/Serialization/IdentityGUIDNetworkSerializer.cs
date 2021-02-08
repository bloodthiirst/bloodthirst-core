using Bloodthirst.Socket.Serializer;
using System;

namespace Bloodthirst.Socket.Serialization.Data
{
    public class IdentityGUIDNetworkSerializer : BaseNetworkSerializer<Guid>
    {
        public override Guid Deserialize(byte[] packet)
        {
            return new Guid(packet);
        }

        public override byte[] Serialize(Guid identifier)
        {
            return identifier.ToByteArray();
        }
    }
}
