using Bloodthirst.Socket.Serializer;
using System;

namespace Bloodthirst.Socket.Serialization.Data
{
    public class IdentityGUIDNetworkSerializer : INetworkSerializer<Guid>
    {
        public Guid Deserialize(byte[] packet)
        {
            return new Guid(packet);
        }

        public byte[] Serialize(Guid identifier)
        {
            return identifier.ToByteArray();
        }
    }
}
