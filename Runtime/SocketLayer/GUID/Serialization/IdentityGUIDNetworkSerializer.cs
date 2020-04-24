using Assets.Scripts.NetworkCommand;
using Bloodthirst.Socket.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.SocketLayer.Serialization.Data
{
    public class IdentityGUIDNetworkSerializer : INetworkSerializer<Guid>
    {
        public int StartIndex => 4;

        public int Length => 16;

        public Guid Deserialize(byte[] packet)
        {
            byte[] from = packet.SubArray(StartIndex, Length);

            return new Guid(from);
        }

        public byte[] Serialize(Guid identifier)
        {
            return identifier.ToByteArray();
        }
    }
}
